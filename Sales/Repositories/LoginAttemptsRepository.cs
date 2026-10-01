// ============================================================
// الملف    : LoginAttemptsRepository.cs
// الغرض    : مستودع مراقبة وإدارة محاولات تسجيل الدخول الفاشلة
// ============================================================

using System;
using System.Data.SQLite;
using Sales.Database;

namespace Sales.Repositories
{
    public class LoginAttemptsRepository
    {
        public bool IsLocked(string userName, out DateTime? lockedUntil)
        {
            lockedUntil = null;
            if (string.IsNullOrWhiteSpace(userName)) return false;

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                const string sql = "SELECT failed_count, locked_until FROM LoginAttempts WHERE user_name=@u LIMIT 1";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@u", userName.Trim());
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return false;

                        string untilStr = r["locked_until"] == DBNull.Value ? null : r["locked_until"].ToString();
                        if (!string.IsNullOrWhiteSpace(untilStr) && DateTime.TryParse(untilStr, out var until))
                        {
                            if (until > DateTime.Now)
                            {
                                lockedUntil = until;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public int RecordFailure(string userName, int maxAttempts)
        {
            if (string.IsNullOrWhiteSpace(userName)) return 0;
            userName = userName.Trim();

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                int count = 1;
                using (var sel = new SQLiteCommand("SELECT failed_count FROM LoginAttempts WHERE user_name=@u", con))
                {
                    sel.Parameters.AddWithValue("@u", userName);
                    object v = sel.ExecuteScalar();
                    if (v != null && v != DBNull.Value)
                        count = Convert.ToInt32(v) + 1;
                }

                string lockedUntil = null;
                if (count >= maxAttempts)
                    lockedUntil = DateTime.Now.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss");

                using (var upsert = new SQLiteCommand(@"
INSERT INTO LoginAttempts(user_name, failed_count, locked_until)
VALUES(@u, @c, @lu)
ON CONFLICT(user_name) DO UPDATE SET
    failed_count = @c,
    locked_until = @lu;", con))
                {
                    upsert.Parameters.AddWithValue("@u", userName);
                    upsert.Parameters.AddWithValue("@c", count);
                    upsert.Parameters.AddWithValue("@lu", (object)lockedUntil ?? DBNull.Value);
                    upsert.ExecuteNonQuery();
                }
                return count;
            }
        }

        public void ClearFailures(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return;
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM LoginAttempts WHERE user_name=@u", con))
                {
                    cmd.Parameters.AddWithValue("@u", userName.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
