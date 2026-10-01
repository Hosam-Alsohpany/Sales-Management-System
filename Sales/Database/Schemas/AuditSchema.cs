// ============================================================
// الملف    : AuditSchema.cs
// الغرض    : تعريف جداول التدقيق والمراقبة وسجلات دخول الأخطاء
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Schemas
{
    public static class AuditSchema
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS OrderAuditLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id INTEGER,
                action TEXT NOT NULL,
                user_name TEXT,
                action_date TEXT DEFAULT CURRENT_TIMESTAMP,
                details TEXT,
                FOREIGN KEY(order_id) REFERENCES Orders(id) ON DELETE SET NULL
            );", con);

            SqlExecutor.Execute(@"CREATE TABLE IF NOT EXISTS LoginAuditLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_name TEXT,
                login_time DATETIME,
                logout_time DATETIME,
                ip_address TEXT,
                success INTEGER,
                failure_reason TEXT
            );", con);

            SqlExecutor.Execute(@"CREATE TABLE IF NOT EXISTS LoginAttempts (
                user_name TEXT PRIMARY KEY,
                failed_count INTEGER NOT NULL DEFAULT 0,
                locked_until TEXT
            );", con);
        }
    }
}
