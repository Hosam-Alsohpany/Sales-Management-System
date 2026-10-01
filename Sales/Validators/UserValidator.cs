// ============================================================
// الملف    : UserValidator.cs
// الغرض    : التحقق من شروط اسم المستخدم وكلمة المرور وصلاحية الحساب
// ============================================================

namespace Sales.Validators
{
    public static class UserValidator
    {
        public static bool TryValidate(string userId, string fullName, string password, bool isNew, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(userId))
            {
                error = "اسم المستخدم مطلوب";
                return false;
            }
            if (string.IsNullOrWhiteSpace(fullName))
            {
                error = "الاسم الكامل مطلوب";
                return false;
            }
            if (isNew && string.IsNullOrWhiteSpace(password))
            {
                error = "كلمة المرور مطلوبة للمستخدم الجديد";
                return false;
            }
            return true;
        }
    }
}
