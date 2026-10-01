// ============================================================
// الملف    : OrderValidator.cs
// الغرض    : التحقق من سلامة بيانات الفاتورة قبل الحفظ النهائي
// ============================================================

using System.Collections.Generic;
using Sales.Models;

namespace Sales.Validators
{
    public static class OrderValidator
    {
        public static bool TryValidateForSave(Order order, decimal paidAmount, bool allowPartialPayment, bool requireCustomer, out List<string> errors)
        {
            errors = new List<string>();
            if (order == null)
            {
                errors.Add("بيانات الفاتورة غير مكتملة");
                return false;
            }

            if (order.Details == null || order.Details.Count == 0)
                errors.Add("لا توجد منتجات في الفاتورة");

            if (requireCustomer && (!order.CustomerId.HasValue || order.CustomerId.Value <= 0))
                errors.Add("يجب اختيار عميل");

            if (paidAmount < 0m)
                errors.Add("قيمة المدفوع غير صحيحة");

            if (!allowPartialPayment && order.Total > 0m && paidAmount != order.Total)
                errors.Add("يجب أن يساوي المدفوع إجمالي الفاتورة");

            if (allowPartialPayment && paidAmount > order.Total)
                errors.Add("المبلغ المدفوع أكبر من إجمالي الفاتورة");

            if (order.Details != null)
            {
                foreach (var d in order.Details)
                {
                    if (d == null) continue;
                    if (d.ProductId <= 0) errors.Add("رقم المنتج غير صحيح في أحد السطور");
                    if (!d.BaseQty.HasValue || d.BaseQty.Value <= 0m) errors.Add("الكمية يجب أن تكون أكبر من صفر في أحد السطور");
                    if (d.Price <= 0m) errors.Add("السعر يجب أن يكون أكبر من صفر في أحد السطور");
                }
            }

            return errors.Count == 0;
        }
    }
}
