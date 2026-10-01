// ============================================================
// الملف    : PaymentRepository.cs
// الغرض    : مستودع لتسجيل وإدارة ومتابعة تسديد دفعات المبيعات
// ============================================================

// الملف    : PaymentRepository.cs
// الغرض    : إدارة المدفوعات المرتبطة بالفواتير (عرض/إضافة/حذف) مع تطبيق صلاحيات الحذف
// يتعامل مع: Forms/Manager_Payments.cs (واجهة المدفوعات) + SessionManager.cs (صلاحيات/مستخدم حالي)
// الجداول  : Payments (ويرتبط منطقياً بـ Orders عبر order_id)
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class PaymentRepository
    {
        // جلب جميع المدفوعات
        /// <summary>
        /// GetAllPayments: جلب كل سجلات المدفوعات من قاعدة البيانات بترتيب الأحدث أولاً
        /// المدخلات : لا يوجد
        /// المخرجات : List&lt;Payment&gt; قائمة المدفوعات
        /// التدفق   : Manager_Payments → PaymentRepository.GetAllPayments → SQLite(Payments) → Payment(Model)
        /// الأخطاء  : أخطاء اتصال/قراءة SQLite أو تحويل الأنواع (Date/Decimal)
        /// </summary>
        public List<Payment> GetAllPayments()
        {
            // list: الحاوية النهائية التي سنرجعها للواجهة
            var list = new List<Payment>();
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                // ── استعلام بسيط: جميع الأعمدة من Payments (الأحدث أولاً) ──
                string sql = "SELECT * FROM Payments ORDER BY id DESC";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        // ── القراءة سطر بسطر وتحويل كل صف إلى كائن Payment ──
                        while (reader.Read())
                        {
                            list.Add(new Payment
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                OrderId = Convert.ToInt32(reader["order_id"]),
                                Amount = Convert.ToDecimal(reader["amount"]),
                                PaymentDate = Convert.ToDateTime(reader["payment_date"]),
                                UserName = reader["user_name"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// AddPayment: إضافة دفعة جديدة لفاتورة محددة
        /// المدخلات : payment كائن الدفعة (يجب أن يحتوي OrderId و Amount)، UserName اختياري
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Payments → PaymentRepository.AddPayment → SessionManager(CurrentUsername) → SQLite(Payments)
        /// الأخطاء  : payment null، رقم فاتورة غير صحيح، مبلغ غير صحيح، أخطاء SQLite أثناء INSERT
        /// </summary>
        public void AddPayment(Payment payment)
        {
            TransactionGuard.Run("Payments.Insert", () =>
            {
                // ── تحقق مبكر لحماية قاعدة البيانات من بيانات غير صالحة ──
                if (payment == null) throw new ArgumentNullException("payment");
                if (payment.OrderId <= 0) throw new InvalidOperationException("رقم الفاتورة غير صحيح");
                if (payment.Amount <= 0) throw new InvalidOperationException("المبلغ يجب أن يكون أكبر من صفر");

                var resolvedUser = payment.UserName ?? SessionManager.CurrentUsername;
                if (string.IsNullOrWhiteSpace(resolvedUser))
                    throw new InvalidOperationException("User required");

                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    using (var tran = con.BeginTransaction())
                    {
                        // ── Atomic overpayment prevention: single statement that succeeds only if it doesn't exceed total ──
                        const string sql = @"
                        INSERT INTO Payments (order_id, amount, payment_date, user_name)
                        SELECT @oid, @amount, CURRENT_TIMESTAMP, @user
                        WHERE (
                            (SELECT IFNULL(SUM(amount),0) FROM Payments WHERE order_id=@oid) + @amount
                        ) <= (
                            SELECT total FROM Orders WHERE id=@oid
                        );";
                        using (var cmd = new SQLiteCommand(sql, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@oid", payment.OrderId);
                            cmd.Parameters.AddWithValue("@amount", payment.Amount);
                            cmd.Parameters.AddWithValue("@user", resolvedUser);
                            int affected = cmd.ExecuteNonQuery();
                            if (affected == 0)
                                throw new InvalidOperationException("المبلغ يتجاوز إجمالي الفاتورة");
                        }

                        tran.Commit();
                    }
                }
            });
        }

        public void AddPayment(int orderId, decimal amount, SQLiteConnection con, SQLiteTransaction tran, string userName = null)
        {
            if (orderId <= 0) throw new InvalidOperationException("رقم الفاتورة غير صحيح");
            if (amount <= 0) throw new InvalidOperationException("المبلغ يجب أن يكون أكبر من صفر");
            if (con == null) throw new ArgumentNullException(nameof(con));
            if (tran == null) throw new ArgumentNullException(nameof(tran));

            var resolvedUser = userName ?? SessionManager.CurrentUsername;
            if (string.IsNullOrWhiteSpace(resolvedUser))
                throw new InvalidOperationException("User required");

            const string sql = @"
                INSERT INTO Payments (order_id, amount, payment_date, user_name)
                SELECT @oid, @amount, CURRENT_TIMESTAMP, @user
                WHERE (
                    (SELECT IFNULL(SUM(amount),0) FROM Payments WHERE order_id=@oid) + @amount
                ) <= (
                    SELECT total FROM Orders WHERE id=@oid
                );";
            using (var cmd = new SQLiteCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@oid", orderId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@user", resolvedUser);
                int affected = cmd.ExecuteNonQuery();
                if (affected == 0)
                    throw new InvalidOperationException("المبلغ يتجاوز إجمالي الفاتورة");
            }
        }

        /// <summary>
        /// DeletePayment: حذف دفعة من Payments (مسموح فقط للأدمن)
        /// المدخلات : paymentId رقم الدفعة
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Payments → PaymentRepository.DeletePayment → SessionManager.RequireAdmin → SQLite(Payments)
        /// الأخطاء  : عدم توفر صلاحيات (RequireAdmin) أو أخطاء SQLite أثناء DELETE
        /// </summary>
        public void DeletePayment(int paymentId)
        {
            TransactionGuard.Run("Payments.Delete", () =>
            {
                // ── شرط أمان: منع الحذف إلا للمسؤول ──
                SessionManager.RequireAdminForDeleteIfEnabled();
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = "DELETE FROM Payments WHERE id = @id";
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", paymentId);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 3                            ║
        // ║  الجداول       : Payments                     ║
        // ║  يستدعيه       : Forms/Manager_Payments.cs    ║
        // ║  يستدعي        : SessionManager.cs + DatabaseInitializer.ConnectionString ║
        // ╚══════════════════════════════════════════════╝
    }
}
