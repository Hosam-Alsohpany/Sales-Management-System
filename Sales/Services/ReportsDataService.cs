// ============================================================
// الملف    : ReportsDataService.cs
// الغرض    : خدمة تجميع وحساب البيانات والتقارير المالية والأرباح والمبيعات
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using Sales.Database;
using Sales.Utilities;

namespace Sales.Services
{
    public sealed class ReportsDataService
    {
        public sealed class SalesRow
        {
            public string Period { get; set; }
            public int OrderCount { get; set; }
            public decimal Total { get; set; }
            public decimal Discount { get; set; }
            public decimal Tax { get; set; }
        }

        public sealed class ProductSalesRow
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal QtySold { get; set; }
            public decimal Revenue { get; set; }
        }

        public sealed class StockRow
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal DisplayQty { get; set; }
            public decimal MinQty { get; set; }
            public decimal CostPrice { get; set; }
            public decimal SellPrice { get; set; }
        }

        public sealed class ProfitRow
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal QtySold { get; set; }
            public decimal Revenue { get; set; }
            public decimal Cost { get; set; }
            public decimal Profit { get; set; }
        }

        public sealed class DebtRow
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalOrders { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal Balance { get; set; }
        }

        public List<SalesRow> GetSalesByDay(DateTime from, DateTime to)
        {
            const string sql = @"
SELECT date(order_date) AS p, COUNT(*) AS cnt,
       IFNULL(SUM(total),0) AS t, IFNULL(SUM(discount),0) AS d, IFNULL(SUM(tax_amount),0) AS tax
FROM Orders
WHERE date(order_date) BETWEEN date(@from) AND date(@to)
GROUP BY date(order_date)
ORDER BY date(order_date)";
            return ReadSalesRows(sql, from, to, r => r["p"]?.ToString());
        }

        public List<SalesRow> GetSalesByMonth(DateTime from, DateTime to)
        {
            const string sql = @"
SELECT strftime('%Y-%m', order_date) AS p, COUNT(*) AS cnt,
       IFNULL(SUM(total),0) AS t, IFNULL(SUM(discount),0) AS d, IFNULL(SUM(tax_amount),0) AS tax
FROM Orders
WHERE date(order_date) BETWEEN date(@from) AND date(@to)
GROUP BY strftime('%Y-%m', order_date)
ORDER BY p";
            return ReadSalesRows(sql, from, to, r => r["p"]?.ToString());
        }

        public decimal GetSalesTotal(DateTime from, DateTime to)
        {
            using (var con = Open())
            using (var cmd = new SQLiteCommand(
                "SELECT IFNULL(SUM(total),0) FROM Orders WHERE date(order_date) BETWEEN date(@from) AND date(@to)", con))
            {
                cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd"));
                return ToDecimal(cmd.ExecuteScalar());
            }
        }

        public List<ProductSalesRow> GetTopProducts(DateTime from, DateTime to, int limit, bool ascending)
        {
            if (limit <= 0) limit = 10;
            string sql = @"
SELECT od.id_product AS pid,
       COALESCE(od.product_name_snapshot, p.label) AS pname,
       IFNULL(SUM(COALESCE(od.base_qty, od.qty_unit, 0)), 0) AS qty,
       IFNULL(SUM(od.total), 0) AS rev
FROM Order_Details od
INNER JOIN Orders o ON o.id = od.id_order
LEFT JOIN Products p ON p.id = od.id_product
WHERE date(o.order_date) BETWEEN date(@from) AND date(@to)
GROUP BY od.id_product
ORDER BY rev " + (ascending ? "ASC" : "DESC") + @"
LIMIT @lim";
            var list = new List<ProductSalesRow>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@lim", limit);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new ProductSalesRow
                        {
                            ProductId = Convert.ToInt32(r["pid"]),
                            ProductName = r["pname"]?.ToString(),
                            QtySold = ToDecimal(r["qty"]),
                            Revenue = ToDecimal(r["rev"])
                        });
                    }
                }
            }
            return list;
        }

        public List<StockRow> GetStockReport(bool lowOnly, int lowThreshold)
        {
            var list = new List<StockRow>();
            string sql = @"
SELECT id, label,
       CAST(qty_scaled AS REAL) / CASE IFNULL(qty_scale_pow10,0)
         WHEN 1 THEN 10.0 WHEN 2 THEN 100.0 WHEN 3 THEN 1000.0
         WHEN 4 THEN 10000.0 WHEN 5 THEN 100000.0 WHEN 6 THEN 1000000.0 ELSE 1.0 END AS dq,
       IFNULL(min_qty,0) AS minq, IFNULL(cost_price,0) AS cost, IFNULL(price,0) AS sell, product_type
FROM Products
WHERE IFNULL(product_type,'physical') <> 'service'";
            if (lowOnly)
                sql += @" AND IFNULL(min_qty,0) > 0
AND (CAST(qty_scaled AS REAL) / CASE IFNULL(qty_scale_pow10,0)
     WHEN 1 THEN 10.0 WHEN 2 THEN 100.0 WHEN 3 THEN 1000.0
     WHEN 4 THEN 10000.0 WHEN 5 THEN 100000.0 WHEN 6 THEN 1000000.0 ELSE 1.0 END) <= @t";
            sql += " ORDER BY label";

            using (var con = Open())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                if (lowOnly)
                    cmd.Parameters.AddWithValue("@t", lowThreshold);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new StockRow
                        {
                            ProductId = Convert.ToInt32(r["id"]),
                            ProductName = r["label"]?.ToString(),
                            DisplayQty = ToDecimal(r["dq"]),
                            MinQty = ToDecimal(r["minq"]),
                            CostPrice = ToDecimal(r["cost"]),
                            SellPrice = ToDecimal(r["sell"])
                        });
                    }
                }
            }
            return list;
        }

        public List<ProfitRow> GetProfitReport(DateTime from, DateTime to)
        {
            const string sql = @"
SELECT od.id_product AS pid,
       COALESCE(od.product_name_snapshot, p.label) AS pname,
       IFNULL(SUM(COALESCE(od.base_qty, od.qty_unit, 0)), 0) AS qty,
       IFNULL(SUM(od.total), 0) AS rev,
       IFNULL(SUM(COALESCE(od.cost_price, p.cost_price, 0) * COALESCE(od.base_qty, od.qty_unit, 0)), 0) AS cost
FROM Order_Details od
INNER JOIN Orders o ON o.id = od.id_order
LEFT JOIN Products p ON p.id = od.id_product
WHERE date(o.order_date) BETWEEN date(@from) AND date(@to)
GROUP BY od.id_product
ORDER BY rev DESC";
            var list = new List<ProfitRow>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd"));
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        decimal rev = ToDecimal(r["rev"]);
                        decimal cost = ToDecimal(r["cost"]);
                        list.Add(new ProfitRow
                        {
                            ProductId = Convert.ToInt32(r["pid"]),
                            ProductName = r["pname"]?.ToString(),
                            QtySold = ToDecimal(r["qty"]),
                            Revenue = rev,
                            Cost = cost,
                            Profit = rev - cost
                        });
                    }
                }
            }
            return list;
        }

        public List<DebtRow> GetDebtsReport()
        {
            const string sql = @"
SELECT c.id, c.name,
       IFNULL(o.total_orders, 0) AS orders_total,
       IFNULL(p.total_paid, 0) AS paid_total,
       IFNULL(o.total_orders, 0) - IFNULL(p.total_paid, 0) AS balance
FROM Customers c
LEFT JOIN (
    SELECT customer_id, SUM(total) AS total_orders FROM Orders GROUP BY customer_id
) o ON o.customer_id = c.id
LEFT JOIN (
    SELECT o.customer_id, SUM(pay.amount) AS total_paid
    FROM Payments pay INNER JOIN Orders o ON o.id = pay.order_id GROUP BY o.customer_id
) p ON p.customer_id = c.id
WHERE IFNULL(o.total_orders, 0) - IFNULL(p.total_paid, 0) > 0
ORDER BY balance DESC";
            var list = new List<DebtRow>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand(sql, con))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    list.Add(new DebtRow
                    {
                        CustomerId = Convert.ToInt32(r["id"]),
                        CustomerName = r["name"]?.ToString(),
                        TotalOrders = ToDecimal(r["orders_total"]),
                        TotalPaid = ToDecimal(r["paid_total"]),
                        Balance = ToDecimal(r["balance"])
                    });
                }
            }
            return list;
        }

        private List<SalesRow> ReadSalesRows(string sql, DateTime from, DateTime to, Func<SQLiteDataReader, string> period)
        {
            var list = new List<SalesRow>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd"));
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new SalesRow
                        {
                            Period = period(r),
                            OrderCount = Convert.ToInt32(r["cnt"]),
                            Total = ToDecimal(r["t"]),
                            Discount = ToDecimal(r["d"]),
                            Tax = ToDecimal(r["tax"])
                        });
                    }
                }
            }
            return list;
        }

        private static SQLiteConnection Open()
        {
            var con = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            con.Open();
            return con;
        }

        private static decimal ToDecimal(object v)
        {
            if (v == null || v == DBNull.Value) return 0m;
            return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
        }
    }
}
