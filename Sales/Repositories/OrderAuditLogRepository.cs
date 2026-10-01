// ============================================================
// الملف    : OrderAuditLogRepository.cs
// الغرض    : مستودع جلب وعرض سجل العمليات (تعديل وحذف) على الفواتير
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;

namespace Sales.Repositories
{
    public class OrderAuditLogRepository
    {
        public List<OrderAuditLogEntry> Search(OrderAuditLogFilter filter)
        {
            filter = filter ?? new OrderAuditLogFilter();
            var list = new List<OrderAuditLogEntry>();
            var clauses = new List<string>();
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                string sql = @"SELECT id, order_id, action, user_name, action_date, details
                               FROM OrderAuditLog WHERE 1=1";

                if (filter.FromDate.HasValue)
                    clauses.Add(" datetime(action_date) >= datetime(@from) ");
                if (filter.ToDate.HasValue)
                    clauses.Add(" datetime(action_date) <= datetime(@to) ");
                if (!string.IsNullOrWhiteSpace(filter.UserName))
                    clauses.Add(" user_name LIKE @user ");
                if (!string.IsNullOrWhiteSpace(filter.Action))
                    clauses.Add(" action = @action ");
                if (filter.OrderId.HasValue)
                    clauses.Add(" order_id = @oid ");

                if (clauses.Count > 0)
                    sql += " AND " + string.Join(" AND ", clauses);
                sql += " ORDER BY id DESC LIMIT 5000";

                using (var cmd = new SQLiteCommand(sql, con))
                {
                    if (filter.FromDate.HasValue)
                        cmd.Parameters.AddWithValue("@from", filter.FromDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (filter.ToDate.HasValue)
                        cmd.Parameters.AddWithValue("@to", filter.ToDate.Value.ToString("yyyy-MM-dd 23:59:59"));
                    if (!string.IsNullOrWhiteSpace(filter.UserName))
                        cmd.Parameters.AddWithValue("@user", "%" + filter.UserName.Trim() + "%");
                    if (!string.IsNullOrWhiteSpace(filter.Action))
                        cmd.Parameters.AddWithValue("@action", filter.Action.Trim());
                    if (filter.OrderId.HasValue)
                        cmd.Parameters.AddWithValue("@oid", filter.OrderId.Value);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(Map(r));
                    }
                }
            }
            return list;
        }

        public static string[] GetKnownActions()
        {
            return new[] { "CREATED", "EDITED", "DELETED" };
        }

        private static OrderAuditLogEntry Map(SQLiteDataReader r)
        {
            DateTime? dt = null;
            if (r["action_date"] != DBNull.Value && DateTime.TryParse(r["action_date"].ToString(), out var parsed))
                dt = parsed;

            return new OrderAuditLogEntry
            {
                Id = Convert.ToInt32(r["id"]),
                OrderId = r["order_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["order_id"]),
                Action = r["action"]?.ToString(),
                UserName = r["user_name"]?.ToString(),
                ActionDate = dt,
                Details = r["details"]?.ToString()
            };
        }
    }

    public class OrderAuditLogFilter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public int? OrderId { get; set; }
    }
}
