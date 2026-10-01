// ============================================================
// الملف    : Migration_002_OrderTaxAmount.cs
// الغرض    : تعديل جدول الطلبات لإضافة عمود نسبة وقيمة الضريبة
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Migrations
{
    public sealed class Migration_002_OrderTaxAmount : IMigration
    {
        public int Version => 2;
        public string Description => "Orders.tax_amount column";

        public void Apply(SQLiteConnection con)
        {
            SqlExecutor.AddColumnIfNotExists(con, "Orders", "tax_amount", "REAL NOT NULL DEFAULT 0");
        }
    }
}
