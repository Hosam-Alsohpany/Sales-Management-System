using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using Sales.Database;

namespace Sales.Utilities
{
    public sealed class DashboardDataService
    {
        public sealed class DashboardKpis
        {
            public decimal SalesToday { get; set; }
            public int OrdersToday { get; set; }
            public decimal SalesYesterday { get; set; }
            public int OrdersYesterday { get; set; }
            public int LowStockCount { get; set; }
            public int OutOfStockCount { get; set; }
            public int ActiveCustomersToday { get; set; }
            public decimal OutstandingTotal { get; set; }
        }

        public sealed class ActivityItem
        {
            public DateTime When { get; set; }
            public string Title { get; set; }
            public string Details { get; set; }
            public string Type { get; set; }
        }

        public sealed class AlertItem
        {
            public string Severity { get; set; } // Critical/Warning/Info
            public string Title { get; set; }
            public string Details { get; set; }
        }

        public sealed class SeriesPoint
        {
            public DateTime Day { get; set; }
            public decimal Value { get; set; }
        }

        public DashboardKpis GetKpis(int lowStockThreshold)
        {
            var k = new DashboardKpis();

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();

                k.SalesToday = ExecuteDecimal(con, "SELECT IFNULL(SUM(total),0) FROM Orders WHERE date(order_date)=date('now','localtime')");
                k.OrdersToday = ExecuteInt(con, "SELECT IFNULL(COUNT(*),0) FROM Orders WHERE date(order_date)=date('now','localtime')");

                k.SalesYesterday = ExecuteDecimal(con, "SELECT IFNULL(SUM(total),0) FROM Orders WHERE date(order_date)=date('now','localtime','-1 day')");
                k.OrdersYesterday = ExecuteInt(con, "SELECT IFNULL(COUNT(*),0) FROM Orders WHERE date(order_date)=date('now','localtime','-1 day')");

                k.ActiveCustomersToday = ExecuteInt(con, "SELECT IFNULL(COUNT(DISTINCT customer_id),0) FROM Orders WHERE customer_id IS NOT NULL AND date(order_date)=date('now','localtime')");

                k.OutOfStockCount = ExecuteInt(con, "SELECT IFNULL(COUNT(*),0) FROM Products WHERE IFNULL(qty_scaled,0) <= 0 AND IFNULL(product_type,'physical') <> 'service'");

                using (var cmd = new SQLiteCommand(@"
SELECT IFNULL(COUNT(*),0) FROM Products
WHERE IFNULL(product_type,'physical') <> 'service'
  AND IFNULL(qty_scaled,0) > 0
  AND IFNULL(min_qty,0) > 0
  AND (CAST(qty_scaled AS REAL) / CASE IFNULL(qty_scale_pow10,0)
        WHEN 1 THEN 10.0 WHEN 2 THEN 100.0 WHEN 3 THEN 1000.0
        WHEN 4 THEN 10000.0 WHEN 5 THEN 100000.0 WHEN 6 THEN 1000000.0
        ELSE 1.0 END) <= @t", con))
                {
                    cmd.Parameters.AddWithValue("@t", lowStockThreshold);
                    k.LowStockCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Outstanding = sum(order.total - paid)
                const string outstandingSql = @"
SELECT IFNULL(SUM(o.total - IFNULL(p.paid,0)),0)
FROM Orders o
LEFT JOIN (
    SELECT order_id, SUM(amount) AS paid
    FROM Payments
    GROUP BY order_id
) p ON p.order_id = o.id
WHERE (o.total - IFNULL(p.paid,0)) > 0";
                k.OutstandingTotal = ExecuteDecimal(con, outstandingSql);
            }

            return k;
        }

        public List<SeriesPoint> GetSalesLastDays(int days)
        {
            if (days <= 0) days = 7;
            var points = new List<SeriesPoint>();

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();

                string sql = @"
SELECT date(order_date) AS d, IFNULL(SUM(total),0) AS s
FROM Orders
WHERE date(order_date) >= date('now','localtime', @from)
GROUP BY date(order_date)
ORDER BY date(order_date);";

                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@from", "-" + (days - 1).ToString(CultureInfo.InvariantCulture) + " day");
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DateTime d;
                            DateTime.TryParse(dr["d"].ToString(), out d);
                            points.Add(new SeriesPoint
                            {
                                Day = d,
                                Value = dr["s"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["s"]) 
                            });
                        }
                    }
                }
            }

            // fill missing days to make chart stable
            var map = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in points)
                map[p.Day.ToString("yyyy-MM-dd")] = p.Value;

            var normalized = new List<SeriesPoint>();
            for (int i = days - 1; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);
                map.TryGetValue(day.ToString("yyyy-MM-dd"), out var v);
                normalized.Add(new SeriesPoint { Day = day, Value = v });
            }

            return normalized;
        }

        public List<ActivityItem> GetLatestActivities(int maxItems)
        {
            if (maxItems <= 0) maxItems = 10;
            var items = new List<ActivityItem>();

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();

                // Orders
                const string sqlOrders = @"
SELECT o.id, o.total, o.order_date,
       IFNULL(c.name,'عميل غير محدد') AS customer_name
FROM Orders o
LEFT JOIN Customers c ON c.id = o.customer_id
ORDER BY datetime(o.order_date) DESC
LIMIT 10";

                using (var cmd = new SQLiteCommand(sqlOrders, con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        items.Add(new ActivityItem
                        {
                            Type = "Order",
                            When = ParseSqliteDate(dr["order_date"].ToString()),
                            Title = "فاتورة جديدة #" + dr["id"].ToString(),
                            Details = dr["customer_name"].ToString() + " | " + SalesNumberFormat.FormatPrice(Convert.ToDecimal(dr["total"]))
                        });
                    }
                }

                // Payments
                const string sqlPayments = @"
SELECT id, order_id, amount, payment_date
FROM Payments
ORDER BY datetime(payment_date) DESC
LIMIT 10";

                using (var cmd = new SQLiteCommand(sqlPayments, con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        items.Add(new ActivityItem
                        {
                            Type = "Payment",
                            When = ParseSqliteDate(dr["payment_date"].ToString()),
                            Title = "دفعة مستلمة (فاتورة #" + dr["order_id"].ToString() + ")",
                            Details = SalesNumberFormat.FormatPrice(Convert.ToDecimal(dr["amount"]))
                        });
                    }
                }

                // Stock history
                const string sqlStock = @"
SELECT sh.qty_change, sh.reason, sh.change_date, IFNULL(p.label,'') AS product_name
FROM StockHistory sh
LEFT JOIN Products p ON p.id = sh.product_id
ORDER BY datetime(sh.change_date) DESC
LIMIT 10";

                using (var cmd = new SQLiteCommand(sqlStock, con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int qty = dr["qty_change"] == DBNull.Value ? 0 : Convert.ToInt32(dr["qty_change"]);
                        string reason = dr["reason"] == DBNull.Value ? string.Empty : dr["reason"].ToString();
                        string product = dr["product_name"] == DBNull.Value ? string.Empty : dr["product_name"].ToString();

                        items.Add(new ActivityItem
                        {
                            Type = "Stock",
                            When = ParseSqliteDate(dr["change_date"].ToString()),
                            Title = "حركة مخزون" + (string.IsNullOrWhiteSpace(product) ? "" : (" - " + product)),
                            Details = (qty >= 0 ? "+" : "") + qty + (string.IsNullOrWhiteSpace(reason) ? "" : (" | " + reason))
                        });
                    }
                }
            }

            items.Sort((a, b) => b.When.CompareTo(a.When));
            if (items.Count > maxItems)
                items = items.GetRange(0, maxItems);

            return items;
        }

        public List<AlertItem> GetAlerts(int lowStockThreshold)
        {
            var alerts = new List<AlertItem>();

            var k = GetKpis(lowStockThreshold);
            if (k.OutOfStockCount > 0)
            {
                alerts.Add(new AlertItem
                {
                    Severity = "Critical",
                    Title = "منتجات نفدت من المخزون",
                    Details = "عدد المنتجات: " + k.OutOfStockCount
                });
            }

            if (k.LowStockCount > 0)
            {
                alerts.Add(new AlertItem
                {
                    Severity = "Warning",
                    Title = "منتجات منخفضة المخزون",
                    Details = "عدد المنتجات: " + k.LowStockCount + " | الحد: " + lowStockThreshold
                });
            }

            if (k.OutstandingTotal > 0)
            {
                alerts.Add(new AlertItem
                {
                    Severity = "Info",
                    Title = "مستحقات (آجل)",
                    Details = "إجمالي المستحق: " + SalesNumberFormat.FormatPrice(k.OutstandingTotal)
                });
            }

            return alerts;
        }

        public decimal GetSalesForPeriod(DateTime from, DateTime to)
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT IFNULL(SUM(total),0) FROM Orders WHERE date(order_date) BETWEEN date(@f) AND date(@t)", con))
                {
                    cmd.Parameters.AddWithValue("@f", from.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@t", to.ToString("yyyy-MM-dd"));
                    object v = cmd.ExecuteScalar();
                    return v == null || v == DBNull.Value ? 0m : Convert.ToDecimal(v);
                }
            }
        }

        public List<ProductSalesRow> GetTopProducts(int limit)
        {
            if (limit <= 0) limit = 10;
            var list = new List<ProductSalesRow>();
            const string sql = @"
SELECT od.id_product AS pid,
       COALESCE(od.product_name_snapshot, p.label) AS pname,
       IFNULL(SUM(COALESCE(od.base_qty, od.qty_unit, 0)), 0) AS qty,
       IFNULL(SUM(od.total), 0) AS rev
FROM Order_Details od
LEFT JOIN Products p ON p.id = od.id_product
GROUP BY od.id_product
ORDER BY rev DESC
LIMIT @lim";
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@lim", limit);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new ProductSalesRow
                            {
                                ProductId = Convert.ToInt32(r["pid"]),
                                ProductName = r["pname"]?.ToString(),
                                QtySold = r["qty"] == DBNull.Value ? 0m : Convert.ToDecimal(r["qty"]),
                                Revenue = r["rev"] == DBNull.Value ? 0m : Convert.ToDecimal(r["rev"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<LowStockProduct> GetLowStockProducts(int threshold)
        {
            var list = new List<LowStockProduct>();
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                const string sql = @"
SELECT id, label,
       CAST(qty_scaled AS REAL) / CASE IFNULL(qty_scale_pow10,0)
         WHEN 1 THEN 10.0 WHEN 2 THEN 100.0 WHEN 3 THEN 1000.0
         WHEN 4 THEN 10000.0 WHEN 5 THEN 100000.0 WHEN 6 THEN 1000000.0 ELSE 1.0 END AS dq,
       IFNULL(min_qty,0) AS minq
FROM Products
WHERE IFNULL(product_type,'physical') <> 'service'
  AND (IFNULL(qty_scaled,0) <= 0 OR (IFNULL(min_qty,0) > 0 AND dq <= @t))
ORDER BY dq ASC
LIMIT 20";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@t", threshold);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new LowStockProduct
                            {
                                ProductId = Convert.ToInt32(r["id"]),
                                ProductName = r["label"]?.ToString(),
                                DisplayQty = r["dq"] == DBNull.Value ? 0m : Convert.ToDecimal(r["dq"]),
                                MinQty = r["minq"] == DBNull.Value ? 0m : Convert.ToDecimal(r["minq"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public sealed class ProductSalesRow
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal QtySold { get; set; }
            public decimal Revenue { get; set; }
        }

        public sealed class LowStockProduct
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal DisplayQty { get; set; }
            public decimal MinQty { get; set; }
        }

        private static int ExecuteInt(SQLiteConnection con, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, con))
            {
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return 0;
                return Convert.ToInt32(result);
            }
        }

        private static decimal ExecuteDecimal(SQLiteConnection con, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, con))
            {
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return 0m;
                return Convert.ToDecimal(result);
            }
        }

        private static DateTime ParseSqliteDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return DateTime.MinValue;
            DateTime dt;
            if (DateTime.TryParse(s, out dt)) return dt;
            return DateTime.MinValue;
        }
    }
}
