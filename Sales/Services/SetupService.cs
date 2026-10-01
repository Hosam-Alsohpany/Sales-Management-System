// ============================================================
// الملف    : SetupService.cs
// الغرض    : تهيئة وبناء هيكل قاعدة البيانات للمرة الأولى
// ============================================================

using System;
using System.Data.SQLite;
using System.Globalization;
using Sales.Database;
using Sales.Utilities;

namespace Sales.Services
{
    public sealed class SetupRequest
    {
        public string StoreName { get; set; }
        public string Currency { get; set; }
        public decimal? DefaultTax { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string AdminFullName { get; set; }
        public string DefaultUnitName { get; set; }
        public string DefaultWarehouseName { get; set; }
        public bool SeedDemoProduct { get; set; }
    }

    public sealed class SetupResult
    {
        public bool Success { get; set; }
        public string UserMessage { get; set; }
        public string DiagnosticMessage { get; set; }
        public int DefaultWarehouseId { get; set; }
    }

    public static class SetupService
    {
        public static SetupResult RunInitialSetup(SetupRequest req)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            var result = new SetupResult();

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();

                try
                {
                    using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", con))
                    {
                        pragma.ExecuteNonQuery();
                    }
                }
                catch
                {
                }

                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        ExecuteStep("EnsureNotInitialized", () => EnsureNotInitialized(con, tran));

                        ExecuteStep("EnsureUnit", () => EnsureUnit(con, tran, req.DefaultUnitName));
                        ExecuteStep("EnsureCategoryGeneral", () => EnsureCategoryGeneral(con, tran));

                        ExecuteStep("CreateAdminUser", () => CreateAdminUser(con, tran, req.AdminUsername, req.AdminPassword, req.AdminFullName));

                        ExecuteStep("SetSetting(StoreName)", () => SetSetting(con, tran, AppSettingsManager.Keys.StoreName, req.StoreName));
                        ExecuteStep("SetSetting(Currency)", () => SetSetting(con, tran, AppSettingsManager.Keys.Currency, req.Currency));
                        ExecuteStep("SetSetting(DefaultTax)", () => SetSetting(con, tran, AppSettingsManager.Keys.DefaultTax, req.DefaultTax.HasValue ? req.DefaultTax.Value.ToString(CultureInfo.InvariantCulture) : string.Empty));
                        ExecuteStep("SetSetting(DefaultUnit)", () => SetSetting(con, tran, AppSettingsManager.Keys.DefaultUnit, req.DefaultUnitName));
                        if (req.SeedDemoProduct)
                        {
                            ExecuteStep("SeedDemoProduct", () => SeedDemoProduct(con, tran));
                        }

                        ExecuteStep("SetSetting(IsInitialized)", () => SetSetting(con, tran, AppSettingsManager.Keys.IsInitialized, "1"));

                        tran.Commit();

                        result.Success = true;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        try { tran.Rollback(); }
                        catch (Exception rbEx)
                        {
                            Logger.LogError(nameof(SetupService), "RunInitialSetup.Rollback", rbEx);
                        }

                        try
                        {
                            Logger.LogError(nameof(SetupService), "RunInitialSetup", ex);
                            string runtimeVersion = string.Empty;
                            try
                            {
                                using (var vcmd = new SQLiteCommand("SELECT sqlite_version()", con))
                                {
                                    object vv = vcmd.ExecuteScalar();
                                    runtimeVersion = vv != null && vv != DBNull.Value ? vv.ToString() : string.Empty;
                                }
                            }
                            catch
                            {
                                runtimeVersion = string.Empty;
                            }

                            Logger.LogInfo(nameof(SetupService), "SQLite", "SQLiteConnection.SQLiteVersion=" + SQLiteConnection.SQLiteVersion + (string.IsNullOrWhiteSpace(runtimeVersion) ? string.Empty : ", sqlite_version()=" + runtimeVersion));
                        }
                        catch
                        {
                        }

                        result.Success = false;
                        result.UserMessage = "فشل تنفيذ الإعداد. لم يتم حفظ أي تغييرات.";
                        result.DiagnosticMessage = ex.ToString();
                        return result;
                    }
                }
            }
        }

        private static void ExecuteStep(string stepName, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("InitialSetup failed at step: " + (stepName ?? string.Empty), ex);
            }
        }

        private static void EnsureNotInitialized(SQLiteConnection con, SQLiteTransaction tran)
        {
            using (var cmd = new SQLiteCommand("SELECT value FROM AppSettings WHERE key='IsInitialized' LIMIT 1", con, tran))
            {
                object v = cmd.ExecuteScalar();
                if (v != null && v != DBNull.Value)
                {
                    var s = v.ToString();
                    if (string.Equals(s, "1") || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "yes", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("System already initialized");
                }
            }
        }

        private static void SetSetting(SQLiteConnection con, SQLiteTransaction tran, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key is required", nameof(key));

            string k = key.Trim();
            string v = value ?? string.Empty;

            using (var insertIgnore = new SQLiteCommand("INSERT OR IGNORE INTO AppSettings(key, value) VALUES(@k, @v)", con, tran))
            {
                insertIgnore.Parameters.AddWithValue("@k", k);
                insertIgnore.Parameters.AddWithValue("@v", v);
                insertIgnore.ExecuteNonQuery();
            }

            using (var update = new SQLiteCommand("UPDATE AppSettings SET value=@v WHERE key=@k", con, tran))
            {
                update.Parameters.AddWithValue("@k", k);
                update.Parameters.AddWithValue("@v", v);
                update.ExecuteNonQuery();
            }
        }

        private static void CreateAdminUser(SQLiteConnection con, SQLiteTransaction tran, string username, string password, string fullName)
        {
            using (var cmd = new SQLiteCommand("INSERT INTO Users (id, pwd_hash, full_name, role) VALUES (@id, @pwd_hash, @name, 'admin')", con, tran))
            {
                cmd.Parameters.AddWithValue("@id", username);
                cmd.Parameters.AddWithValue("@pwd_hash", PasswordHasher.HashPassword(password));
                cmd.Parameters.AddWithValue("@name", string.IsNullOrWhiteSpace(fullName) ? username : fullName);
                cmd.ExecuteNonQuery();
            }
        }

        private static int EnsureWarehouse(SQLiteConnection con, SQLiteTransaction tran, string name)
        {
            using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Warehouses(name) VALUES(@name);", con, tran))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand("SELECT id FROM Warehouses WHERE name=@name LIMIT 1", con, tran))
            {
                cmd.Parameters.AddWithValue("@name", name);
                object v = cmd.ExecuteScalar();
                return Convert.ToInt32(v);
            }
        }

        private static void EnsureUnit(SQLiteConnection con, SQLiteTransaction tran, string name)
        {
            using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Units(name) VALUES(@name)", con, tran))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }

        private static void EnsureCategoryGeneral(SQLiteConnection con, SQLiteTransaction tran)
        {
            using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Categories (id, name) VALUES (1, 'عام');", con, tran))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static void SeedDemoProduct(SQLiteConnection con, SQLiteTransaction tran)
        {
            // Keep minimal: a single product linked to category_id=1.
            // Ensure sellable immediately: create ProductUnit (factor=1) + default ProductUnitBarcode.

            long productId;
            using (var cmd = new SQLiteCommand(@"INSERT INTO Products(label, qty_scaled, qty_scale_pow10, product_type, price, cost_price, min_qty, category_id, note, created_by, created_at)
VALUES('منتج تجريبي', 0, 0, 'physical', 0, 0, 0, 1, 'Seed', 'setup', CURRENT_TIMESTAMP);
SELECT last_insert_rowid();", con, tran))
            {
                productId = (long)cmd.ExecuteScalar();
            }

            int unitId;
            using (var cmd = new SQLiteCommand("SELECT id FROM Units WHERE name=@n LIMIT 1", con, tran))
            {
                cmd.Parameters.AddWithValue("@n", "قطعة");
                object v = cmd.ExecuteScalar();
                if (v != null && v != DBNull.Value)
                    unitId = Convert.ToInt32(v);
                else
                {
                    using (var ins = new SQLiteCommand("INSERT INTO Units(name) VALUES(@n); SELECT last_insert_rowid();", con, tran))
                    {
                        ins.Parameters.AddWithValue("@n", "قطعة");
                        unitId = Convert.ToInt32((long)ins.ExecuteScalar());
                    }
                }
            }

            long productUnitId;
            using (var cmd = new SQLiteCommand(@"INSERT INTO ProductUnits(product_id, unit_id, factor, sell_price, cost_price, display_order)
VALUES(@pid, @uid, 1, 0, 0, 1);
SELECT last_insert_rowid();", con, tran))
            {
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@uid", unitId);
                productUnitId = (long)cmd.ExecuteScalar();
            }

            using (var cmd = new SQLiteCommand(@"INSERT INTO ProductUnitBarcodes(product_unit_id, barcode, is_default)
VALUES(@puid, @b, 1);", con, tran))
            {
                cmd.Parameters.AddWithValue("@puid", productUnitId);
                cmd.Parameters.AddWithValue("@b", "DEMO-0001");
                cmd.ExecuteNonQuery();
            }
        }
    }
}
