// ============================================================
// الملف    : UsersSchema.cs
// الغرض    : تعريف جداول الحسابات، كلمات المرور، وصلاحيات النظام
// ============================================================

using System.Data.SQLite;

namespace Sales.Database.Schemas
{
    public static class UsersSchema
    {
        public static void EnsureCreated(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS Users (
                id TEXT PRIMARY KEY,
                pwd_hash TEXT NOT NULL,
                full_name TEXT NOT NULL DEFAULT '',
                role TEXT NOT NULL DEFAULT 'user',
                CONSTRAINT ck_Users_Role CHECK (role IN ('admin','user'))
            );", con);

            SqlExecutor.Execute(@"
            CREATE TABLE IF NOT EXISTS AppSettings (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL DEFAULT ''
            );", con);
        }
    }
}
