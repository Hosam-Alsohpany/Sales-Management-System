// ============================================================
// الملف    : PasswordHasher.cs
// الغرض    : توليد والتحقق من Hash لكلمات المرور باستخدام PBKDF2 (Rfc2898DeriveBytes) مع Salt و Iterations
// يتعامل مع: Repositories/UserRepository.cs (تخزين/تحقق pwd_hash) + أي شاشة تسجيل/إدارة مستخدمين
// التنسيق  : iterations.salt.key (Base64) مثل: 10000.<saltBase64>.<keyBase64>
// ============================================================

using System;
using System.Linq;
using System.Security.Cryptography;

namespace Sales.Utilities
{
    public static class PasswordHasher
    {
        // SaltSize: حجم الملح (Salt) بالبايت لإضافة عشوائية لكل كلمة مرور
        private const int SaltSize = 16;
        // KeySize: حجم المفتاح المشتق (Derived Key) النهائي بالبايت
        private const int KeySize = 32;
        // Iterations: عدد دورات الاشتقاق (كلما زاد كانت العملية أبطأ وأصعب على التخمين)
        private const int Iterations = 10000;

        /// <summary>
        /// HashPassword: تحويل كلمة المرور إلى نص Hash آمن للتخزين (بدون حفظ كلمة المرور نفسها)
        /// المدخلات : password كلمة المرور
        /// المخرجات : string بصيغة iterations.salt.key
        /// التدفق   : Signup/AddUser/UpdateUser → HashPassword → PBKDF2 → تخزين في Users.pwd_hash
        /// الأخطاء  : ArgumentNullException إذا كانت كلمة المرور null
        /// </summary>
        public static string HashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException("password");

            // ── PBKDF2: اشتقاق مفتاح من كلمة المرور + salt عشوائي + عدد دورات ──
            using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                // ── نخزن iterations داخل النص لتسهيل تغيير القيمة مستقبلاً بدون كسر القديم ──
                return string.Format("{0}.{1}.{2}", Iterations, salt, key);
            }
        }

        /// <summary>
        /// VerifyPassword: التحقق من كلمة المرور بمقارنتها مع hash المخزن
        /// المدخلات : hash النص المخزن (iterations.salt.key)، password كلمة المرور المُدخلة
        /// المخرجات : bool
        /// التدفق   : Login/ValidateUser → VerifyPassword → إعادة اشتقاق المفتاح → مقارنة SequenceEqual
        /// الأخطاء  : يعيد false عند أي صيغة غير صحيحة أو Base64 غير صالح
        /// </summary>
        public static bool VerifyPassword(string hash, string password)
        {
            if (string.IsNullOrWhiteSpace(hash) || password == null) return false;

            // ── تفكيك التنسيق: iterations.salt.key ──
            var parts = hash.Split('.');
            if (parts.Length != 3) return false;

            int iterations;
            if (!int.TryParse(parts[0], out iterations)) return false;

            byte[] salt;
            byte[] key;

            try
            {
                salt = Convert.FromBase64String(parts[1]);
                key = Convert.FromBase64String(parts[2]);
            }
            catch
            {
                return false;
            }

            // ── نعيد الاشتقاق بنفس salt و iterations ثم نقارن المفتاح الناتج بالمخزن ──
            using (var algorithm = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);
                return key.SequenceEqual(keyToCheck);
            }
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 2                            ║
        // ║  الخوارزمية    : PBKDF2 (Rfc2898DeriveBytes)  ║
        // ║  التنسيق       : iterations.salt.key          ║
        // ║  يستخدمه       : UserRepository.cs            ║
        // ╚══════════════════════════════════════════════╝
    }
}
