// ============================================================
 // الملف    : SessionManager.cs
 // الغرض    : إدارة “جلسة المستخدم” الحالية داخل التطبيق (اسم المستخدم + الدور) وتوفير دوال تحقق للصلاحيات
 // يتعامل مع: Login.cs (تعيين المستخدم بعد نجاح الدخول) + Repositories (RequireAdmin) + أي Form يحتاج اسم المستخدم الحالي
 // المخرجات : قيم ثابتة أثناء تشغيل البرنامج (In-Memory) وليست مخزنة في قاعدة البيانات
 // ============================================================

using System;

namespace Sales.Utilities
{
    public static class SessionManager
    {
        /// <summary>
        /// CurrentUsername: اسم المستخدم الحالي بعد تسجيل الدخول
        /// المدخلات : يتم تعيينه عبر SetCurrentUser
        /// المخرجات : string
        /// التدفق   : Login → SetCurrentUser → CurrentUsername → (استخدام داخل Repositories/Forms)
        /// </summary>
        public static string CurrentUsername { get; private set; }

        /// <summary>
        /// CurrentUserRole: صلاحية المستخدم الحالي (مثل: admin/user)
        /// المدخلات : يتم تعيينه عبر SetCurrentUser
        /// المخرجات : string
        /// التدفق   : Login → SetCurrentUser → CurrentUserRole → IsAdmin/RequireAdmin
        /// </summary>
        public static string CurrentUserRole { get; private set; }

        /// <summary>
        /// IsAdmin: هل المستخدم الحالي يمتلك صلاحية admin؟
        /// المدخلات : لا يوجد (يعتمد على CurrentUserRole)
        /// المخرجات : bool
        /// التدفق   : أي عملية محمية → RequireAdmin → IsAdmin
        /// </summary>
        public static bool IsAdmin
        {
            get { return string.Equals(CurrentUserRole, "admin", StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// SetCurrentUser: تعيين بيانات المستخدم الحالي بعد نجاح تسجيل الدخول
        /// المدخلات : username اسم المستخدم، role الدور
        /// المخرجات : لا يوجد
        /// التدفق   : Login.ValidateUser → SessionManager.SetCurrentUser → استخدام القيم لاحقاً
        /// </summary>
        public static void SetCurrentUser(string username, string role)
        {
            CurrentUsername = username;
            CurrentUserRole = role;
        }

        /// <summary>
        /// Clear: مسح بيانات الجلسة (يستخدم عند تسجيل الخروج أو إعادة تهيئة حالة التطبيق)
        /// المدخلات : لا يوجد
        /// المخرجات : لا يوجد
        /// التدفق   : Logout/Exit → Clear → (تفريغ CurrentUsername/Role)
        /// </summary>
        public static void Clear()
        {
            CurrentUsername = null;
            CurrentUserRole = null;
        }

        /// <summary>
        /// RequireAdmin: شرط أمان يرمي UnauthorizedAccessException إذا لم يكن المستخدم Admin
        /// المدخلات : لا يوجد
        /// المخرجات : لا يوجد (إما يمر أو يرمي استثناء)
        /// التدفق   : Repository.Add/Update/Delete (حساس) → RequireAdmin → (تمرير/منع)
        /// الأخطاء  : UnauthorizedAccessException
        /// </summary>
        public static void RequireAdmin()
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("هذه العملية متاحة للمدير فقط");
        }

        public static bool IsDeleteRestrictedToAdmin
        {
            get { return AppSettingsManager.GetBool(AppSettingsManager.Keys.EnforceAdminForDeletes, true); }
        }

        public static bool CanCurrentUserDelete
        {
            get { return !IsDeleteRestrictedToAdmin || IsAdmin; }
        }

        public static bool IsReadOnlyForCurrentUser
        {
            get { return !IsAdmin && AppSettingsManager.GetBool(AppSettingsManager.Keys.ReadOnlyForNormalUser, false); }
        }

        public static void RequireAdminForDeleteIfEnabled()
        {
            if (IsDeleteRestrictedToAdmin)
                RequireAdmin();
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  الدور          : إدارة جلسة المستخدم         ║
        // ║  أهم القيم      : CurrentUsername/CurrentUserRole ║
        // ║  أهم التحقق     : RequireAdmin               ║
        // ╚══════════════════════════════════════════════╝
    }
}
