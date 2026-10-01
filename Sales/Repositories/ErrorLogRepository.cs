// ============================================================
// الملف    : ErrorLogRepository.cs
// الغرض    : مستودع تسجيل وحفظ وعرض سجل الأخطاء التقنية الموثقة
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;

namespace Sales.Repositories
{
    public class ErrorLogRepository
    {
        public void Insert(string level, string source, string message, string exceptionText)
        {
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = @"INSERT INTO ErrorLog (level, source, message, exception)
                                         VALUES (@l, @s, @m, @e)";
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@l", level ?? "ERROR");
                        cmd.Parameters.AddWithValue("@s", (object)source ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@m", message ?? string.Empty);
                        cmd.Parameters.AddWithValue("@e", string.IsNullOrWhiteSpace(exceptionText) ? (object)DBNull.Value : exceptionText);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }
        }

        public List<ErrorLogEntry> GetRecent(int limit = 500)
        {
            if (limit <= 0) limit = 500;
            var list = new List<ErrorLogEntry>();
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM ErrorLog ORDER BY id DESC LIMIT @n", con))
                {
                    cmd.Parameters.AddWithValue("@n", limit);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(Map(r));
                    }
                }
            }
            return list;
        }

        private static ErrorLogEntry Map(SQLiteDataReader r)
        {
            DateTime? dt = null;
            if (r["created_at"] != DBNull.Value && DateTime.TryParse(r["created_at"].ToString(), out var parsed))
                dt = parsed;
            return new ErrorLogEntry
            {
                Id = Convert.ToInt32(r["id"]),
                Level = r["level"]?.ToString(),
                Source = r["source"]?.ToString(),
                Message = r["message"]?.ToString(),
                Exception = r["exception"]?.ToString(),
                CreatedAt = dt
            };
        }
    }
}
