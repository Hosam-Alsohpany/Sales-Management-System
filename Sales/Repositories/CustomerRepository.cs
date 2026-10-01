// ============================================================
// الملف    : CustomerRepository.cs
// الغرض    : مستودع لتسجيل وإدارة حسابات وبيانات العملاء
// ============================================================

// الملف    : CustomerRepository.cs
// الغرض    : إدارة بيانات العملاء (CRUD) + حساب الرصيد المستحق لكل عميل (إجمالي المبيعات - إجمالي المدفوعات)
// يتعامل مع: Forms/Manager_Customers.cs + Forms/Customers_List.cs + Repositories/OrderRepository.cs + Repositories/PaymentRepository.cs
// الجداول  : Customers, Orders, Payments
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Models;
using Sales.Database;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class CustomerRepository
    {
        /// <summary>
        /// GetAllCustomers: جلب جميع العملاء مع حساب الرصيد (Balance) لكل عميل
        /// المدخلات : لا يوجد
        /// المخرجات : List&lt;Customer&gt; (يتضمن Balance)
        /// التدفق   : Manager_Customers/Customers_List → GetAllCustomers → SQLite(Join Customers/Orders/Payments) → Customer(Model)
        /// ملاحظة   : الرصيد = مجموع فواتير العميل (Orders.total) - مجموع ما دفعه (Payments.amount)
        /// الأخطاء  : أخطاء اتصال/SQL/تحويل أنواع
        /// </summary>
        public List<Customer> GetAllCustomers()
        {
            var list = new List<Customer>();
            using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();

                // ── الاستعلام يحسب الرصيد بدون تخزينه في جدول Customers ──
                // 1) o.total_orders: مجموع إجماليات الفواتير لكل عميل
                // 2) p.total_paid : مجموع الدفعات المرتبطة بفواتير هذا العميل
                // 3) balance      : الفرق بينهما (قد يكون 0 أو سالب إذا دفع أكثر)
                string sql = @"
SELECT
    c.id,
    c.name,
    c.tel,
    c.details,
    c.address,
    IFNULL(o.total_orders, 0) - IFNULL(p.total_paid, 0) AS balance
FROM Customers c
LEFT JOIN (
    SELECT customer_id, SUM(total) AS total_orders
    FROM Orders
    GROUP BY customer_id
) o ON o.customer_id = c.id
LEFT JOIN (
    SELECT o.customer_id AS customer_id, SUM(pay.amount) AS total_paid
    FROM Payments pay
    INNER JOIN Orders o ON o.id = pay.order_id
    GROUP BY o.customer_id
) p ON p.customer_id = c.id
ORDER BY c.name;
";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Customer
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            Name = dr["name"].ToString(),
                            Tel = dr["tel"].ToString(),
                            Details = dr["details"] != DBNull.Value ? dr["details"].ToString() : "",
                            Address = dr["address"] != DBNull.Value ? dr["address"].ToString() : "",
                            Balance = dr["balance"] != DBNull.Value ? Convert.ToDecimal(dr["balance"]) : 0m
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// AddCustomer: إضافة عميل جديد
        /// المدخلات : c كائن العميل
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Customers → AddCustomer → SQLite(Customers INSERT)
        /// الأخطاء  : أخطاء SQLite
        /// </summary>
        public void AddCustomer(Customer c)
        {
            TransactionGuard.Run("Customers.Insert", () =>
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "INSERT INTO Customers (name, tel, details, address) VALUES (@name, @tel, @details, @address)";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@name", c.Name);
                        cmd.Parameters.AddWithValue("@tel", c.Tel ?? "");
                        cmd.Parameters.AddWithValue("@details", c.Details ?? "");
                        cmd.Parameters.AddWithValue("@address", c.Address ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        /// <summary>
        /// UpdateCustomer: تعديل بيانات عميل (يتطلب Admin)
        /// المدخلات : c (Id + بيانات العميل)
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Customers → UpdateCustomer → RequireAdmin → SQLite(Customers UPDATE)
        /// الأخطاء  : UnauthorizedAccessException عند عدم الصلاحية + أخطاء SQLite
        /// </summary>
        public void UpdateCustomer(Customer c)
        {
            TransactionGuard.Run("Customers.Update", () =>
            {
                SessionManager.RequireAdmin();
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "UPDATE Customers SET name=@name, tel=@tel, details=@details, address=@address WHERE id=@id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@name", c.Name);
                        cmd.Parameters.AddWithValue("@tel", c.Tel ?? "");
                        cmd.Parameters.AddWithValue("@details", c.Details ?? "");
                        cmd.Parameters.AddWithValue("@address", c.Address ?? "");
                        cmd.Parameters.AddWithValue("@id", c.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        public decimal GetCustomerBalance(int customerId)
        {
            if (customerId <= 0) return 0m;
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                const string sql = @"
SELECT IFNULL((SELECT SUM(total) FROM Orders WHERE customer_id=@id),0)
     - IFNULL((SELECT SUM(pay.amount) FROM Payments pay INNER JOIN Orders o ON o.id=pay.order_id WHERE o.customer_id=@id),0)";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", customerId);
                    object v = cmd.ExecuteScalar();
                    return v == null || v == DBNull.Value ? 0m : Convert.ToDecimal(v);
                }
            }
        }

        public Customer GetCustomerById(int id)
        {
            if (id <= 0) return null;
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand("SELECT id, name, tel, details, address FROM Customers WHERE id=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return null;
                        var c = new Customer
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            Name = dr["name"]?.ToString(),
                            Tel = dr["tel"]?.ToString(),
                            Details = dr["details"] != DBNull.Value ? dr["details"].ToString() : "",
                            Address = dr["address"] != DBNull.Value ? dr["address"].ToString() : ""
                        };
                        c.Balance = GetCustomerBalance(c.Id);
                        return c;
                    }
                }
            }
        }

        /// <summary>
        /// DeleteCustomer: حذف عميل (يتطلب Admin)
        /// المدخلات : id رقم العميل
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Customers → DeleteCustomer → RequireAdmin → SQLite(Customers DELETE)
        /// الأخطاء  : UnauthorizedAccessException عند عدم الصلاحية + أخطاء SQLite
        /// </summary>
        public void DeleteCustomer(int id)
        {
            TransactionGuard.Run("Customers.Delete", () =>
            {
                SessionManager.RequireAdminForDeleteIfEnabled();
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "DELETE FROM Customers WHERE id=@id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 4                            ║
        // ║  الجداول       : Customers، Orders، Payments  ║
        // ║  يحسب           : Balance (إجمالي الطلبات - إجمالي المدفوعات) ║
        // ║  يستدعيه       : Manager_Customers.cs، Customers_List.cs ║
        // ║  يستدعي        : SessionManager + DatabaseInitializer.ConnectionString ║
        // ╚══════════════════════════════════════════════╝
    }
}
