// ============================================================
// الملف    : PaymentTriggers.cs
// الغرض    : تريغرز تلقائية لتحديث حالة الفاتورة عند إضافة دفعات
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Triggers
{
    public static class PaymentTriggers
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"CREATE TRIGGER IF NOT EXISTS trg_Payments_PreventOverpayment
                BEFORE INSERT ON Payments
                FOR EACH ROW
                BEGIN
                    SELECT CASE
                        WHEN (
                            (SELECT IFNULL(SUM(amount),0) FROM Payments WHERE order_id = NEW.order_id) + NEW.amount
                        ) > (
                            SELECT total FROM Orders WHERE id = NEW.order_id
                        )
                        THEN RAISE(ABORT, 'Overpayment')
                    END;
                END;", con);

            SqlExecutor.Execute(@"CREATE TRIGGER IF NOT EXISTS trg_Payments_PreventOverpayment_Update
                BEFORE UPDATE OF order_id, amount ON Payments
                FOR EACH ROW
                BEGIN
                    SELECT CASE
                        WHEN (
                            (SELECT IFNULL(SUM(amount),0) FROM Payments WHERE order_id = NEW.order_id AND id <> OLD.id) + NEW.amount
                        ) > (
                            SELECT total FROM Orders WHERE id = NEW.order_id
                        )
                        THEN RAISE(ABORT, 'Overpayment')
                    END;
                END;", con);

            SqlExecutor.Execute(@"CREATE TRIGGER IF NOT EXISTS trg_Payments_PreventOverpayment_Delete
                BEFORE DELETE ON Payments
                FOR EACH ROW
                BEGIN
                    SELECT CASE
                        WHEN OLD.order_id IS NULL THEN 0
                        WHEN (
                            SELECT IFNULL(SUM(amount),0) FROM Payments WHERE order_id = OLD.order_id AND id <> OLD.id
                        ) > (
                            SELECT total FROM Orders WHERE id = OLD.order_id
                        )
                        THEN RAISE(ABORT, 'OverpaymentAfterDelete')
                    END;
                END;", con);
        }
    }
}
