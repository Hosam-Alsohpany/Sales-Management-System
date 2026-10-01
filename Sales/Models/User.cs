// ============================================================
// الملف    : User.cs
// الغرض    : نموذج حساب المستخدم لحفظ الصلاحيات وبيانات الدخول
// ============================================================

using System;

namespace Sales.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Pwd { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // admin, user
    }
}
