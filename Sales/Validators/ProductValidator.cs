// ============================================================
// الملف    : ProductValidator.cs
// الغرض    : التحقق من صحة بيانات المنتج (السعر، الباركود، الاسم)
// ============================================================

namespace Sales.Validators
{
    public static class ProductValidator
    {
        public static bool TryValidate(string label, int qty, decimal price, decimal costPrice, decimal minQty, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(label))
            {
                error = "اسم المنتج مطلوب";
                return false;
            }
            if (qty < 0)
            {
                error = "الكمية لا يمكن أن تكون سالبة";
                return false;
            }
            if (price < 0m)
            {
                error = "سعر البيع غير صحيح";
                return false;
            }
            if (costPrice < 0m)
            {
                error = "سعر التكلفة غير صحيح";
                return false;
            }
            if (minQty < 0m)
            {
                error = "الحد الأدنى للمخزون غير صحيح";
                return false;
            }
            return true;
        }
    }
}
