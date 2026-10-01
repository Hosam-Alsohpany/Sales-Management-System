// ============================================================
// الملف    : BarcodeSchema.cs
// الغرض    : تعريف إعدادات وجداول باركود الموازين والمنتجات
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Schemas
{
    public static class BarcodeSchema
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS Units (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE
            );", con);

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS ProductUnits (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_id INTEGER NOT NULL,
                unit_id INTEGER NOT NULL,
                factor REAL NOT NULL,
                sell_price REAL NOT NULL DEFAULT 0,
                cost_price REAL NOT NULL DEFAULT 0,
                display_order INTEGER NOT NULL DEFAULT 1,
                parent_product_unit_id INTEGER,
                pack_size REAL,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE CASCADE,
                FOREIGN KEY(unit_id) REFERENCES Units(id) ON DELETE RESTRICT,
                CONSTRAINT ck_ProductUnits_Factor_Positive CHECK (factor > 0)
            );", con);

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS ProductUnitBarcodes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_unit_id INTEGER NOT NULL,
                barcode TEXT NOT NULL,
                barcode_type TEXT NOT NULL DEFAULT 'NORMAL',
                is_default INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY(product_unit_id) REFERENCES ProductUnits(id) ON DELETE CASCADE
            );", con);
        }
    }
}
