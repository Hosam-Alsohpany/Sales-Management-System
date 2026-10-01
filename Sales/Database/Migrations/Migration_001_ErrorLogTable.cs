// ============================================================
// الملف    : Migration_001_ErrorLogTable.cs
// الغرض    : إنشاء جدول سجل الأخطاء (ErrorLog) لتتبع الأعطال بالنظام
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Migrations
{
    public sealed class Migration_001_ErrorLogTable : IMigration
    {
        public int Version => 1;
        public string Description => "ErrorLog table for critical errors";

        public void Apply(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"CREATE TABLE IF NOT EXISTS ErrorLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                level TEXT NOT NULL DEFAULT 'ERROR',
                source TEXT,
                message TEXT NOT NULL,
                exception TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP
            );", con);
            try
            {
                SqlExecutor.Execute("CREATE INDEX IF NOT EXISTS idx_ErrorLog_CreatedAt ON ErrorLog(created_at);", con);
            }
            catch { }
        }
    }
}
