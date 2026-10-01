using System.Data.SQLite;

namespace Sales.Database.Migrations
{
    /// <summary>
    /// ترحيل إزالة أعمدة legacy (product_date, qty) — idempotent عبر مفتاح AppSettings.
    /// </summary>
    public sealed class Migration_003_LegacyColumnsBatch1 : IMigration
    {
        public int Version => 3;
        public string Description => "Legacy column removal batch1 (product_date, qty, snapshots)";

        public void Apply(SQLiteConnection con)
        {
            LegacySchemaMigration.ApplyBatch1(con);
        }
    }
}
