// ============================================================
// الملف    : PaymentValidator.cs
// الغرض    : التحقق من صحة وقيمة الدفعات المالية المسجلة للفاتورة
// ============================================================

namespace Sales.Validators
{
    public static class PaymentValidator
    {
        public static bool TryValidate(int orderId, decimal amount, out string error)
        {
            error = null;
            if (orderId <= 0)
            {
                error = "رقم الفاتورة غير صحيح";
                return false;
            }
            if (amount <= 0m)
            {
                error = "مبلغ الدفعة يجب أن يكون أكبر من صفر";
                return false;
            }
            return true;
        }
    }
}
