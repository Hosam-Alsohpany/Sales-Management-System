// ============================================================
// الملف    : LoginAuditLogRepository.cs
// الغرض    : مستودع جلب وعرض سجل عمليات الدخول والخروج للمستخدمين
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;

namespace Sales.Repositories
{
    public class LoginAuditLogRepository
    {
        public int Insert(string userName, string ipAddress, bool success, string failureReason)
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                const string sql = @"
INSERT INTO LoginAuditLog (user_name, login_time, ip_address, success, failure_reason)
VALUES (@u, CURRENT_TIMESTAMP, @ip, @ok, @reason);
SELECT last_insert_rowid();";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@u", userName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@ip", string.IsNullOrWhiteSpace(ipAddress) ? (object)DBNull.Value : ipAddress);
                    cmd.Parameters.AddWithValue("@ok", success ? 1 : 0);
                    cmd.Parameters.AddWithValue("@reason", string.IsNullOrWhiteSpace(failureReason) ? (object)DBNull.Value : failureReason);
                    return Convert.ToInt32((long)cmd.ExecuteScalar());
                }
            }
        }

        public void SetLogoutForOpenSession(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return;
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                const string sql = @"
UPDATE LoginAuditLog
SET logout_time = CURRENT_TIMESTAMP
WHERE id = (
    SELECT id FROM LoginAuditLog
    WHERE user_name = @u AND success = 1 AND logout_time IS NULL
    ORDER BY id DESC LIMIT 1
);";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@u", userName.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<LoginAuditLogEntry> Search(DateTime? from, DateTime? to, string userName, bool? successOnly)
        {
            var list = new List<LoginAuditLogEntry>();
            var where = new List<string>();
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                string sql = @"SELECT id, user_name, login_time, logout_time, ip_address, success, failure_reason
                               FROM LoginAuditLog WHERE 1=1";

                if (from.HasValue)
                {
                    where.Add(" datetime(login_time) >= datetime(@from) ");
                }
                if (to.HasValue)
                {
                    where.Add(" datetime(login_time) <= datetime(@to) ");
                }
                if (!string.IsNullOrWhiteSpace(userName))
                    where.Add(" user_name LIKE @u ");
                if (successOnly.HasValue)
                    where.Add(" success = @ok ");

                if (where.Count > 0)
                    sql += " AND " + string.Join(" AND ", where);
                sql += " ORDER BY id DESC LIMIT 5000";

                using (var cmd = new SQLiteCommand(sql, con))
                {
                    if (from.HasValue)
                        cmd.Parameters.AddWithValue("@from", from.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (to.HasValue)
                        cmd.Parameters.AddWithValue("@to", to.Value.ToString("yyyy-MM-dd 23:59:59"));
                    if (!string.IsNullOrWhiteSpace(userName))
                        cmd.Parameters.AddWithValue("@u", "%" + userName.Trim() + "%");
                    if (successOnly.HasValue)
                        cmd.Parameters.AddWithValue("@ok", successOnly.Value ? 1 : 0);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(Map(r));
                        }
                    }
                }
            }
            return list;
        }

        private static LoginAuditLogEntry Map(SQLiteDataReader r)
        {
            DateTime? login = null;
            DateTime? logout = null;
            if (r["login_time"] != DBNull.Value && DateTime.TryParse(r["login_time"].ToString(), out var lt))
                login = lt;
            if (r["logout_time"] != DBNull.Value && DateTime.TryParse(r["logout_time"].ToString(), out var lo))
                logout = lo;

            return new LoginAuditLogEntry
            {
                Id = Convert.ToInt32(r["id"]),
                UserName = r["user_name"]?.ToString(),
                LoginTime = login,
                LogoutTime = logout,
                IpAddress = r["ip_address"]?.ToString(),
                Success = r["success"] != DBNull.Value && Convert.ToInt32(r["success"]) == 1,
                FailureReason = r["failure_reason"]?.ToString()
            };
        }
    }
}
