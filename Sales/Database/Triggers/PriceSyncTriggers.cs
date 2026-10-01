// ============================================================
// الملف    : PriceSyncTriggers.cs
// الغرض    : تريغرز لمزامنة الأسعار بين المنتجات ووحداتها التابعة
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Triggers
{
    public static class PriceSyncTriggers
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnits_SyncProductPrice_Insert
                AFTER INSERT ON ProductUnits
                FOR EACH ROW
                WHEN NEW.factor = 1
                BEGIN
                    UPDATE Products
                    SET price = NEW.sell_price,
                        cost_price = NEW.cost_price,
                        updated_at = CURRENT_TIMESTAMP
                    WHERE id = NEW.product_id;
                END;", con);

            SqlExecutor.Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnits_SyncProductPrice_Update
                AFTER UPDATE OF sell_price, cost_price, factor ON ProductUnits
                FOR EACH ROW
                WHEN NEW.factor = 1
                BEGIN
                    UPDATE Products
                    SET price = NEW.sell_price,
                        cost_price = NEW.cost_price,
                        updated_at = CURRENT_TIMESTAMP
                    WHERE id = NEW.product_id;
                END;", con);
        }
    }
}
