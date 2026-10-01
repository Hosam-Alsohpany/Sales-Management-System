// ============================================================
// الملف    : ProductsSchema.cs
// الغرض    : تعريف جداول المنتجات، الأصناف، والوحدات التابعة
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Schemas
{
    public static class ProductsSchema
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS Products (
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
            try { SqlExecutor.Execute("CREATE INDEX IF NOT EXISTS idx_Products_Label ON Products(label);", con); } catch { }
        }
    }
}
