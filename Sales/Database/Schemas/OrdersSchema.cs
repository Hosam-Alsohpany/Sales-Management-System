// ============================================================
// الملف    : OrdersSchema.cs
// الغرض    : تعريف جداول المبيعات، الفواتير، الدفعات، والعملاء
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Schemas
{
    public static class OrdersSchema
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS Orders (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                request_id TEXT,
                order_date TEXT NOT NULL,
                customer_id INTEGER,
                total REAL NOT NULL DEFAULT 0,
                discount REAL NOT NULL DEFAULT 0,
                tax_amount REAL NOT NULL DEFAULT 0,
                note TEXT,
                created_by TEXT,
                FOREIGN KEY(customer_id) REFERENCES Customers(id) ON DELETE SET NULL
            );", con);
            try { SqlExecutor.Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_orders_request_id_unique ON Orders(request_id);", con); } catch { }
            try { SqlExecutor.Execute("CREATE INDEX IF NOT EXISTS idx_Orders_OrderDate ON Orders(order_date);", con); } catch { }

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS Order_Details (
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
            try { SqlExecutor.Execute("CREATE INDEX IF NOT EXISTS idx_OrderDetails_OrderId ON Order_Details(id_order);", con); } catch { }

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS Payments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id INTEGER NOT NULL,
                amount REAL NOT NULL,
                payment_date TEXT DEFAULT CURRENT_TIMESTAMP,
                user_name TEXT,
                FOREIGN KEY(order_id) REFERENCES Orders(id) ON DELETE CASCADE
            );", con);
        }
    }
}
