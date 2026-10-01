// ============================================================
// الملف    : OrderDiscountHelper.cs
// الغرض    : حساب الخصومات ونسب التخفيض المطبقة على فواتير المبيعات
// ============================================================

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sales.Utilities
{
    public static class OrderDiscountHelper
    {
        private static readonly Regex DiscountPattern = new Regex(@"\[خصم\s*:\s*(\d+(\.\d+)?)\]", RegexOptions.Compiled);

        public static decimal ExtractDiscount(string note)
        {
            if (string.IsNullOrWhiteSpace(note)) return 0m;

            var match = DiscountPattern.Match(note);
            if (!match.Success) return 0m;

            decimal discount;
            if (decimal.TryParse(match.Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out discount))
                return discount;

            if (decimal.TryParse(match.Groups[1].Value, out discount))
                return discount;

            return 0m;
        }

        public static string ApplyDiscount(string originalNote, decimal discount)
        {
            var cleaned = RemoveDiscount(originalNote);

            if (discount > 0)
            {
                if (!string.IsNullOrEmpty(cleaned)) cleaned += " ";
                cleaned += "[خصم:" + discount.ToString("G29", CultureInfo.InvariantCulture) + "]";
            }

            return cleaned;
        }

        public static string RemoveDiscount(string note)
        {
            return DiscountPattern.Replace(note ?? string.Empty, string.Empty).Trim();
        }
    }
}
