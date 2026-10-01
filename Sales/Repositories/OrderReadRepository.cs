using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Sales.Database;
using Sales.Utilities;

namespace Sales.Repositories
{
    /// <summary>
    /// قراءة بيانات الفاتورة للتعديل/العرض (بدون SQL داخل Forms).
    /// </summary>
    public class OrderReadRepository
    {
        public class OrderHeaderForEdit
        {
            public DateTime OrderDate { get; set; }
            public string Note { get; set; }
            public decimal Discount { get; set; }
            public string CreatedBy { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerTel { get; set; }
            public decimal PaidAmount { get; set; }
        }

        public OrderHeaderForEdit GetOrderHeaderForEdit(int orderId)
        {
            const string sql = @"
                SELECT o.order_date, o.note, IFNULL(o.discount, 0) AS discount, o.created_by,
                       c.id AS customer_id, c.name AS customer_name, c.tel AS customer_tel
                FROM Orders o
                LEFT JOIN Customers c ON o.customer_id = c.id
                WHERE o.id = @orderId";

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return null;

                        string note = dr["note"] != DBNull.Value ? dr["note"].ToString() : string.Empty;
                        decimal discount = dr["discount"] != DBNull.Value ? Convert.ToDecimal(dr["discount"]) : 0m;
                        if (discount <= 0m)
                            discount = OrderDiscountHelper.ExtractDiscount(note);

                        var header = new OrderHeaderForEdit
                        {
                            Note = note,
                            Discount = discount,
                            CreatedBy = dr["created_by"] != DBNull.Value ? dr["created_by"].ToString() : string.Empty,
                            CustomerId = dr["customer_id"] != DBNull.Value ? Convert.ToInt32(dr["customer_id"]) : 0,
                            CustomerName = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : string.Empty,
                            CustomerTel = dr["customer_tel"] != DBNull.Value ? dr["customer_tel"].ToString() : string.Empty
                        };

                        string dateText = dr["order_date"] != DBNull.Value ? dr["order_date"].ToString() : string.Empty;
                        header.OrderDate = DateTime.ParseExact(dateText, "yyyy-MM-dd HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture);

                        header.PaidAmount = GetPaidTotal(con, orderId);
                        return header;
                    }
                }
            }
        }

        public DataTable GetOrderDetailRowsForEdit(int orderId)
        {
            const string sql = @"
                SELECT
                    p.id AS product_id,
                    COALESCE(od.product_name_snapshot, p.label) AS product_label,
                    p.note AS product_note,
                    c.name AS category_name,
                    COALESCE(od.base_qty, od.qty_unit, 0) AS qty,
                    COALESCE(od.sell_price_snapshot, od.price) AS price,
                    od.total AS total,
                    od.barcode_snapshot AS barcode_snapshot
                FROM Order_Details od
                INNER JOIN Products p ON od.id_product = p.id
                LEFT JOIN Categories c ON p.category_id = c.id
                WHERE od.id_order = @orderId";

            var dt = new DataTable();
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public List<RecentOrderSummary> GetRecentOrders(int limit)
        {
            return GetRecentOrdersInternal(limit, null);
        }

        public List<RecentOrderSummary> GetRecentOrdersForCustomer(int customerId, int limit)
        {
            if (customerId <= 0) return GetRecentOrders(limit);
            return GetRecentOrdersInternal(limit, customerId);
        }

        private List<RecentOrderSummary> GetRecentOrdersInternal(int limit, int? customerId)
        {
            if (limit <= 0) limit = 5;
            var list = new List<RecentOrderSummary>();
            string sql = @"
SELECT o.id, o.order_date, o.total, IFNULL(c.name,'') AS customer_name
FROM Orders o
LEFT JOIN Customers c ON c.id = o.customer_id";
            if (customerId.HasValue)
                sql += " WHERE o.customer_id = @cid";
            sql += @"
ORDER BY datetime(o.order_date) DESC
LIMIT @lim";
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@lim", limit);
                    if (customerId.HasValue)
                        cmd.Parameters.AddWithValue("@cid", customerId.Value);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            DateTime dt = DateTime.Now;
                            DateTime.TryParse(r["order_date"]?.ToString(), out dt);
                            list.Add(new RecentOrderSummary
                            {
                                OrderId = Convert.ToInt32(r["id"]),
                                OrderDate = dt,
                                Total = r["total"] == DBNull.Value ? 0m : Convert.ToDecimal(r["total"]),
                                CustomerName = r["customer_name"]?.ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public class RecentOrderSummary
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
            public string CustomerName { get; set; }
        }

        private static decimal GetPaidTotal(SQLiteConnection con, int orderId)
        {
            using (var cmd = new SQLiteCommand("SELECT COALESCE(SUM(amount),0) FROM Payments WHERE order_id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", orderId);
                object v = cmd.ExecuteScalar();
                return v == null || v == DBNull.Value ? 0m : Convert.ToDecimal(v);
            }
        }
    }
}
