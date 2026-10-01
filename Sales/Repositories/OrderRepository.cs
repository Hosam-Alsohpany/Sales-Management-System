// ============================================================
// الملف    : OrderRepository.cs
// الغرض    : مستودع حفظ وإدارة فواتير المبيعات والتعديل عليها
// ============================================================

// الملف    : OrderRepository.cs
// الغرض    : حفظ/تعديل فواتير البيع (Orders + Order_Details) مع تحديث المخزون وتسجيل حركاته وحفظ الدفعات
// يتعامل مع: Forms/Manager_Orders.cs (واجهة إدارة الفواتير) + ProductRepository.cs (خصم المخزون)
// الجداول  : Orders, Order_Details, Products, StockHistory, Payments
// ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using Sales.Database;
using Sales.Models;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    // ============================================
    // مخزن بيانات الفواتير (Order Repository)
    // ============================================
    public class OrderRepository
    {
        private readonly SalesTransactionService _tx = new SalesTransactionService();

        /// <summary>
        /// SaveOrder: حفظ فاتورة جديدة (رأس + تفاصيل) مع خصم المخزون وتسجيل حركة المخزون وحفظ دفعة أولية
        /// المدخلات : order كائن الفاتورة (Order + Details)، paidAmount المبلغ المدفوع الآن
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Orders → OrderRepository.SaveOrder → SQLite(Orders/Order_Details) → ProductRepository.DecreaseStock → SQLite(StockHistory/Payments)
        /// الأخطاء  : بيانات فاتورة ناقصة، نفاد مخزون، أخطاء SQLite، أي خطأ يؤدي لعمل Rollback
        /// </summary>
        public void SaveOrder(Order order, decimal paidAmount)
        {
            _tx.SaveSaleOrder(order, paidAmount);
        }

        // تحديث فاتورة موجودة (مع معالجة المخزون بدقة)
        /// <summary>
        /// UpdateOrder: تحديث فاتورة موجودة بإرجاع مخزون التفاصيل القديمة ثم تطبيق التفاصيل الجديدة
        /// المدخلات : order الفاتورة بعد التعديل (يجب أن تحتوي Details الجديدة)، paidAmount المبلغ المدفوع، userName اسم المستخدم الذي نفذ التعديل
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Orders → OrderRepository.UpdateOrder → (قراءة تفاصيل قديمة) → Restore Stock → Delete Details → Insert New Details → Decrease Stock → StockHistory
        /// الأخطاء  : أي خطأ يؤدي لـ Rollback، وقد يحدث فشل إذا كان المخزون الجديد غير كاف (لا يوجد فحص مسبق هنا)
        /// </summary>
        public void UpdateOrder(Order order, decimal paidAmount, string userName)
        {
            _tx.UpdateSaleOrder(order, paidAmount, userName);
        }

        private static bool IsUniqueConstraint(SQLiteException ex)
        {
            if (ex == null) return false;

            // System.Data.SQLite exposes ErrorCode in older versions; ResultCode may not exist.
            try
            {
                // In some versions ErrorCode is int, in others it is SQLiteErrorCode.
                if (object.Equals(ex.ErrorCode, SQLiteErrorCode.Constraint) || object.Equals(ex.ErrorCode, (int)SQLiteErrorCode.Constraint))
                    return true;
            }
            catch
            {
                // ignore
            }

            var msg = ex.Message ?? string.Empty;
            return msg.IndexOf("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) >= 0
                   || msg.IndexOf("constraint failed", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void DeleteOrder(int orderId, string userName, string reason)
        {
            _tx.DeleteSaleOrder(orderId, userName, reason);
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 3                            ║
        // ║  الجداول       : Orders، Order_Details، Products، StockHistory، Payments ║
        // ║  يستدعيه       : Forms/Manager_Orders.cs      ║
        // ║  يستدعي        : ProductRepository.cs + DatabaseInitializer.ConnectionString ║
        // ╚══════════════════════════════════════════════╝
    }
}
