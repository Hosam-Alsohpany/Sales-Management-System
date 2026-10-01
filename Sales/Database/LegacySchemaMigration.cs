// ============================================================
// الملف    : LegacySchemaMigration.cs
// الغرض    : ترحيل جداول قاعدة البيانات القديمة لتتوافق مع الهيكل الحديث
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Sales.Utilities;

namespace Sales.Database
{
    /// <summary>
    /// ترحيلات إزالة الأعمدة legacy (product_date, qty) وإضافة snapshots للفواتير.
    /// </summary>
    public static class LegacySchemaMigration
    {
        private const string MigrationKey = "SchemaMigration_LegacyColumnsRemoved_v1";

        /// <summary>
        /// يطبّق ترحيلات الدفعة الأولى (13–15) مرة واحدة مع نسخة احتياطية تلقائية.
        /// </summary>
        public static void ApplyBatch1(SQLiteConnection con)
        {
            if (con == null) throw new ArgumentNullException(nameof(con));

            if (IsMigrationApplied(con))
                return;

            BackupDatabaseFile();

            try { Execute("PRAGMA foreign_keys = OFF;", con); } catch { }

            try
            {
                MigrateProductDateToCreatedAt(con);
                RebuildProductsTableWithoutLegacyColumns(con);
                EnsureOrderDetailSnapshotColumns(con);
                RebuildOrderDetailsWithoutLegacyQty(con);

                MarkMigrationApplied(con);
            }
            finally
            {
                try { Execute("PRAGMA foreign_keys = ON;", con); } catch { }
            }
        }

        private static bool IsMigrationApplied(SQLiteConnection con)
        {
            try
            {
                CreateAppSettingsIfNeeded(con);
                using (var cmd = new SQLiteCommand("SELECT value FROM AppSettings WHERE key=@k LIMIT 1", con))
                {
                    cmd.Parameters.AddWithValue("@k", MigrationKey);
                    object v = cmd.ExecuteScalar();
                    return v != null && v != DBNull.Value && string.Equals(v.ToString(), "1", StringComparison.Ordinal);
                }
            }
            catch
            {
                return false;
            }
        }

        private static void MarkMigrationApplied(SQLiteConnection con)
        {
            CreateAppSettingsIfNeeded(con);
            using (var cmd = new SQLiteCommand(
                "INSERT OR REPLACE INTO AppSettings(key, value) VALUES (@k, '1')", con))
            {
                cmd.Parameters.AddWithValue("@k", MigrationKey);
                cmd.ExecuteNonQuery();
            }
        }

        private static void CreateAppSettingsIfNeeded(SQLiteConnection con)
        {
            Execute(@"CREATE TABLE IF NOT EXISTS AppSettings (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL DEFAULT ''
            );", con);
        }

        private static void BackupDatabaseFile()
        {
            try
            {
                string dbPath = DatabaseInitializer.DbPath;
                if (!File.Exists(dbPath)) return;

                string backupDir = Path.Combine(Path.GetDirectoryName(dbPath) ?? dbPath, "Backups");
                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string dest = Path.Combine(backupDir, "DataBase_pre_batch1_" + stamp + ".db");
                File.Copy(dbPath, dest, overwrite: false);
            }
            catch (Exception ex)
            {
                try { Logger.LogError("LegacySchemaMigration backup failed", ex); } catch { }
            }
        }

        private static void MigrateProductDateToCreatedAt(SQLiteConnection con)
        {
            if (!ColumnExists(con, "Products", "product_date")) return;

            Execute(@"UPDATE Products
                     SET created_at = COALESCE(NULLIF(TRIM(created_at), ''), NULLIF(TRIM(product_date), ''), CURRENT_TIMESTAMP)
                     WHERE created_at IS NULL OR TRIM(created_at) = '';", con);
        }

        private static void RebuildProductsTableWithoutLegacyColumns(SQLiteConnection con)
        {
            if (!ColumnExists(con, "Products", "qty") && !ColumnExists(con, "Products", "product_date"))
                return;

            Execute(@"CREATE TABLE IF NOT EXISTS Products_new (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                sku TEXT,
                label TEXT NOT NULL,
                qty_scaled INTEGER NOT NULL DEFAULT 0,
                qty_scale_pow10 INTEGER NOT NULL DEFAULT 0,
                product_type TEXT NOT NULL DEFAULT 'physical',
                price REAL NOT NULL DEFAULT 0,
                cost_price REAL NOT NULL DEFAULT 0,
                min_qty REAL NOT NULL DEFAULT 0,
                category_id INTEGER,
                note TEXT,
                created_by TEXT,
                expiry_date TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT,
                image_path TEXT,
                FOREIGN KEY(category_id) REFERENCES Categories(id) ON DELETE SET NULL,
                CONSTRAINT ck_Products_ProductType CHECK (product_type IN ('physical','weighted','service')),
                CONSTRAINT ck_Products_QtyScalePow10 CHECK (qty_scale_pow10 >= 0 AND qty_scale_pow10 <= 6)
            );", con);

            string insert = @"INSERT INTO Products_new (
                id, sku, label, qty_scaled, qty_scale_pow10, product_type, price, cost_price, min_qty,
                category_id, note, created_by, expiry_date, created_at, updated_at, image_path)
            SELECT
                id, sku, label,
                CASE WHEN IFNULL(qty_scaled, 0) <> 0 THEN qty_scaled
                ELSE IFNULL(qty, 0) * (
                    CASE IFNULL(qty_scale_pow10, 0)
                        WHEN 1 THEN 10 WHEN 2 THEN 100 WHEN 3 THEN 1000
                        WHEN 4 THEN 10000 WHEN 5 THEN 100000 WHEN 6 THEN 1000000
                        ELSE 1
                    END
                )
                END,
                IFNULL(qty_scale_pow10, 0),
                IFNULL(product_type, 'physical'),
                IFNULL(price, 0), IFNULL(cost_price, 0), IFNULL(min_qty, 0),
                category_id, note, created_by, expiry_date,
                COALESCE(NULLIF(TRIM(created_at), ''), NULLIF(TRIM(product_date), ''), CURRENT_TIMESTAMP),
                updated_at, image_path
            FROM Products;";

            Execute(insert, con);
            Execute("DROP TABLE Products;", con);
            Execute("ALTER TABLE Products_new RENAME TO Products;", con);

            try { Execute("CREATE INDEX IF NOT EXISTS idx_Products_Label ON Products(label);", con); } catch { }
            RecreateProductTriggers(con);
        }

        private static void RecreateProductTriggers(SQLiteConnection con)
        {
            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_Products_DefaultScale_OnInsert
                AFTER INSERT ON Products
                FOR EACH ROW
                WHEN NEW.product_type = 'weighted' AND (NEW.qty_scale_pow10 IS NULL OR NEW.qty_scale_pow10 = 0)
                BEGIN
                    UPDATE Products SET qty_scale_pow10 = 3 WHERE id = NEW.id;
                END;", con);

            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_Products_DefaultScale_OnProductTypeUpdate
                AFTER UPDATE OF product_type ON Products
                FOR EACH ROW
                WHEN NEW.product_type = 'weighted' AND (NEW.qty_scale_pow10 IS NULL OR NEW.qty_scale_pow10 = 0)
                BEGIN
                    UPDATE Products SET qty_scale_pow10 = 3 WHERE id = NEW.id;
                END;", con);
        }

        private static void EnsureOrderDetailSnapshotColumns(SQLiteConnection con)
        {
            AddColumnIfNotExists(con, "Order_Details", "product_name_snapshot", "TEXT");
            AddColumnIfNotExists(con, "Order_Details", "barcode_snapshot", "TEXT");
            AddColumnIfNotExists(con, "Order_Details", "sell_price_snapshot", "REAL");

            try
            {
                Execute(@"UPDATE Order_Details
                         SET sell_price_snapshot = price
                         WHERE sell_price_snapshot IS NULL;", con);

                Execute(@"UPDATE Order_Details
                         SET product_name_snapshot = (
                             SELECT p.label FROM Products p WHERE p.id = Order_Details.id_product
                         )
                         WHERE product_name_snapshot IS NULL OR TRIM(product_name_snapshot) = '';", con);

                Execute(@"UPDATE Order_Details
                         SET base_qty = COALESCE(base_qty, qty, qty_unit, 0)
                         WHERE base_qty IS NULL OR base_qty = 0;", con);
            }
            catch { }
        }

        private static void RebuildOrderDetailsWithoutLegacyQty(SQLiteConnection con)
        {
            if (!ColumnExists(con, "Order_Details", "qty"))
                return;

            Execute(@"CREATE TABLE IF NOT EXISTS Order_Details_new (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                id_order INTEGER NOT NULL,
                id_product INTEGER NOT NULL,
                price REAL NOT NULL,
                total REAL NOT NULL,
                product_unit_id INTEGER,
                unit_name_snapshot TEXT,
                factor_snapshot REAL,
                qty_unit REAL,
                base_qty REAL NOT NULL DEFAULT 0,
                cost_price REAL,
                qty_scaled_snapshot INTEGER,
                qty_scale_pow10_snapshot INTEGER,
                product_name_snapshot TEXT,
                barcode_snapshot TEXT,
                sell_price_snapshot REAL,
                FOREIGN KEY(id_order) REFERENCES Orders(id) ON DELETE CASCADE,
                FOREIGN KEY(id_product) REFERENCES Products(id) ON DELETE RESTRICT
            );", con);

            Execute(@"INSERT INTO Order_Details_new (
                id, id_order, id_product, price, total, product_unit_id, unit_name_snapshot, factor_snapshot,
                qty_unit, base_qty, cost_price, qty_scaled_snapshot, qty_scale_pow10_snapshot,
                product_name_snapshot, barcode_snapshot, sell_price_snapshot)
            SELECT
                od.id, od.id_order, od.id_product, od.price, od.total, od.product_unit_id, od.unit_name_snapshot, od.factor_snapshot,
                od.qty_unit,
                COALESCE(od.base_qty, od.qty, od.qty_unit, 0),
                od.cost_price, od.qty_scaled_snapshot, od.qty_scale_pow10_snapshot,
                COALESCE(od.product_name_snapshot, p.label),
                od.barcode_snapshot,
                COALESCE(od.sell_price_snapshot, od.price)
            FROM Order_Details od
            LEFT JOIN Products p ON p.id = od.id_product;", con);

            Execute("DROP TABLE Order_Details;", con);
            Execute("ALTER TABLE Order_Details_new RENAME TO Order_Details;", con);
            try { Execute("CREATE INDEX IF NOT EXISTS idx_OrderDetails_OrderId ON Order_Details(id_order);", con); } catch { }
        }

        private static bool ColumnExists(SQLiteConnection con, string table, string column)
        {
            using (var cmd = new SQLiteCommand($"PRAGMA table_info({table});", con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (string.Equals(reader["name"].ToString(), column, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        private static void AddColumnIfNotExists(SQLiteConnection con, string table, string column, string type)
        {
            if (ColumnExists(con, table, column)) return;
            Execute($"ALTER TABLE {table} ADD COLUMN {column} {type};", con);
        }

        private static void Execute(string sql, SQLiteConnection con)
        {
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
