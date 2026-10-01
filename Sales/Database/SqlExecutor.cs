// ============================================================
// الملف    : SqlExecutor.cs
// الغرض    : تنفيذ الاستعلامات والأوامر المباشرة على قاعدة البيانات
// ============================================================

using System;
using System.Data.SQLite;

namespace Sales.Database
{
    /// <summary>
    /// تنفيذ أوامر SQL المشتركة (CREATE/ALTER/PRAGMA).
    /// </summary>
    public static class SqlExecutor
    {
        public static void Execute(string sql, SQLiteConnection con)
        {
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static bool TableExists(SQLiteConnection con, string table)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@t", con))
            {
                cmd.Parameters.AddWithValue("@t", table);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static bool ColumnExists(SQLiteConnection con, string table, string column)
        {
            using (var cmd = new SQLiteCommand($"PRAGMA table_info({table});", con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (string.Equals(reader["name"].ToString(), column, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        public static void AddColumnIfNotExists(SQLiteConnection con, string table, string column, string type)
        {
            if (ColumnExists(con, table, column)) return;
            Execute($"ALTER TABLE {table} ADD COLUMN {column} {type};", con);
        }
    }
}
