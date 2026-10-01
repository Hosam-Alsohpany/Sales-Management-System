// ============================================================
// الملف    : ValidationHelper.cs
// الغرض    : التحقق من صحة صيغ المدخلات كالبريد الإلكتروني والأرقام
// ============================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sales.Utilities
{
    public static class ValidationHelper
    {
        public static bool ValidateRequiredField(string value, string fieldName, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (errors != null)
                    errors.Add("حقل '" + fieldName + "' مطلوب ولا يمكن تركه فارغاً");
                return false;
            }
            return true;
        }

        public static bool ValidatePositiveNumber(decimal value, string fieldName, List<string> errors)
        {
            if (value <= 0)
            {
                if (errors != null)
                    errors.Add("قيمة '" + fieldName + "' يجب أن تكون أكبر من صفر");
                return false;
            }
            return true;
        }

        public static bool ValidateNonNegativeNumber(decimal value, string fieldName, List<string> errors)
        {
            if (value < 0)
            {
                if (errors != null)
                    errors.Add("قيمة '" + fieldName + "' لا يمكن أن تكون سالبة");
                return false;
            }
            return true;
        }

        public static bool ValidateEmail(string email, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern))
            {
                if (errors != null)
                    errors.Add("صيغة البريد الإلكتروني غير صحيحة");
                return false;
            }
            return true;
        }
    }
}
