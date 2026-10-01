// ============================================================
// الملف    : DatabaseBootstrap.cs
// الغرض    : تهيئة وبدء تنفيذ إعدادات قاعدة البيانات والترحيلات
// ============================================================

using System.Data.SQLite;
using Sales.Database.Migrations;
using Sales.Database.Schemas;
using Sales.Database.Triggers;

namespace Sales.Database
{
    /// <summary>
    /// تنسيق إنشاء الجداول والترحيلات بعد التهيئة الأساسية.
    /// </summary>
    public static class DatabaseBootstrap
    {
        public static void RunPostSchemaMigrations(SQLiteConnection con)
        {
            PaymentTriggers.EnsureCreated(con);
            PriceSyncTriggers.EnsureCreated(con);
            MigrationRunner.RunPending(con, MigrationRunner.GetAllMigrations());
        }

        public static void EnsureCoreSchemas(SQLiteConnection con)
        {
            UsersSchema.EnsureCreated(con);
            ProductsSchema.EnsureCreated(con);
            OrdersSchema.EnsureCreated(con);
            StockSchema.EnsureCreated(con);
            BarcodeSchema.EnsureCreated(con);
            AuditSchema.EnsureCreated(con);
        }
    }
}
