// ============================================================
// الملف    : StockSchema.cs
// الغرض    : تعريف جداول المخازن، حركات الجرد، وسجل المخزون
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Schemas
{
    public static class StockSchema
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS StockHistory (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_id INTEGER NOT NULL,
                qty_change INTEGER NOT NULL,
                qty_change_scaled INTEGER,
                qty_scale_pow10 INTEGER NOT NULL DEFAULT 0,
                reason TEXT,
                change_date TEXT DEFAULT CURRENT_TIMESTAMP,
                user_name TEXT,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE CASCADE
            );", con);

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS StockHistoryBatches (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                created_by TEXT,
                reason TEXT,
                reference TEXT,
                note TEXT
            );", con);

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS StockHistoryBatchItems (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                batch_id INTEGER NOT NULL,
                product_id INTEGER NOT NULL,
                qty_before INTEGER,
                qty_after INTEGER,
                qty_change INTEGER,
                FOREIGN KEY(batch_id) REFERENCES StockHistoryBatches(id) ON DELETE CASCADE,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE RESTRICT
            );", con);

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS Purchases (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                supplier_id INTEGER,
                purchase_date TEXT NOT NULL,
                note TEXT,
                created_by TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(supplier_id) REFERENCES Suppliers(id) ON DELETE SET NULL
            );", con);

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS PurchaseDetails (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                purchase_id INTEGER NOT NULL,
                product_id INTEGER NOT NULL,
                product_unit_id INTEGER,
                unit_name_snapshot TEXT,
                factor_snapshot REAL,
                qty REAL NOT NULL,
                base_qty REAL NOT NULL,
                cost_price REAL NOT NULL DEFAULT 0,
                sell_price REAL NOT NULL DEFAULT 0,
                expiry_date TEXT,
                note TEXT,
                qty_scaled_snapshot INTEGER,
                qty_scale_pow10_snapshot INTEGER,
                FOREIGN KEY(purchase_id) REFERENCES Purchases(id) ON DELETE CASCADE,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE RESTRICT,
                FOREIGN KEY(product_unit_id) REFERENCES ProductUnits(id) ON DELETE SET NULL
            );", con);
        }
    }
}
