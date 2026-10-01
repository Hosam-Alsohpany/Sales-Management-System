using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sales.Utilities
{
    /// <summary>
    /// تنسيق الأرقام بالأرقام الإنجليزية (0-9).
    /// الأسعار: فاصل آلاف وعشري بنقطة — مثل 5.000.00 و 11.000.000.00
    /// </summary>
    public static class SalesNumberFormat
    {
        public static readonly CultureInfo AppCulture;

        private const char ThousandSeparator = '.';
        private const char DecimalSeparator = '.';

        /// <summary>علامة LTR لإجبار عرض 0-9 في بيئة عربية.</summary>
        public const char LtrMark = '\u200E';

        static SalesNumberFormat()
        {
            AppCulture = (CultureInfo)CultureInfo.GetCultureInfo("en-US").Clone();
            ConfigureCultureNumberFormat(AppCulture);
        }

        private static void ConfigureCultureNumberFormat(CultureInfo culture)
        {
            culture.NumberFormat.NumberDecimalDigits = 2;
            culture.NumberFormat.NumberGroupSeparator = ThousandSeparator.ToString();
            culture.NumberFormat.NumberDecimalSeparator = DecimalSeparator.ToString();
            // منع Windows من استبدال 0-9 بأرقام عربية/هندية (٠١٢...)
            culture.NumberFormat.DigitSubstitution = DigitShapes.None;
        }

        public static void ApplyApplicationCulture()
        {
            ConfigureCultureNumberFormat(AppCulture);
            CultureInfo.DefaultThreadCurrentCulture = AppCulture;
            CultureInfo.DefaultThreadCurrentUICulture = AppCulture;
            ApplyToCurrentThread();
        }

        public static void ApplyToCurrentThread()
        {
            Thread.CurrentThread.CurrentCulture = AppCulture;
            Thread.CurrentThread.CurrentUICulture = AppCulture;
        }

        /// <summary>تحويل حرف رقم عربي/فارسي إلى 0-9 إن وُجد.</summary>
        public static char? MapDigitCharToEnglish(char ch)
        {
            if (ch >= '\u0660' && ch <= '\u0669')
                return (char)('0' + (ch - '\u0660'));
            if (ch >= '\u06F0' && ch <= '\u06F9')
                return (char)('0' + (ch - '\u06F0'));
            return null;
        }

        public static bool ContainsNonEnglishDigits(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            foreach (char c in text)
            {
                if ((c >= '\u0660' && c <= '\u0669') || (c >= '\u06F0' && c <= '\u06F9'))
                    return true;
            }
            return false;
        }

        public static bool LooksLikeNumericText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            text = ToEnglishDigits(text).Trim();
            bool hasDigit = false;
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    continue;
                }
                if (c == '.' || c == ',' || c == '-' || c == '+' || c == ' ') continue;
                return false;
            }
            return hasDigit;
        }

        public static string ToEnglishDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                if (c == LtrMark || c == '\u2066' || c == '\u2069' || c == '\u200F')
                    continue;
                char? mapped = MapDigitCharToEnglish(c);
                sb.Append(mapped ?? c);
            }

            return sb.ToString();
        }

        /// <summary>نص للعرض في الواجهة (أرقام إنجليزية + اتجاه LTR).</summary>
        public static string ForDisplay(string text)
        {
            text = ToEnglishDigits(text ?? string.Empty);
            if (string.IsNullOrEmpty(text)) return text;
            if (text[0] == LtrMark) return text;
            return LtrMark + text;
        }

        public static string StripForParse(string text)
        {
            return ToEnglishDigits(text ?? string.Empty).Trim();
        }

        /// <summary>سعر/مبلغ: 5.000.00 ، 100.00 ، 11.000.000.00</summary>
        public static string FormatPrice(decimal value)
        {
            return FormatWithDecimals(value, 2);
        }

        /// <summary>كمية أو رقم عام بدون كسور (مع فاصل آلاف بنقطة).</summary>
        public static string FormatQuantity(decimal value)
        {
            return FormatWithDecimals(value, 0);
        }

        public static string FormatInteger(long value)
        {
            if (value < 0)
                return ForDisplay("-" + FormatThousands(Math.Abs(value), insertLeadingGroup: true));
            return ForDisplay(FormatThousands(value, insertLeadingGroup: true));
        }

        public static string FormatNumber(decimal value, int decimalPlaces)
        {
            return FormatWithDecimals(value, decimalPlaces);
        }

        /// <summary>
        /// معامل التحويل بين الوحدات (بدون فاصل آلاف): 1 ، 12 ، 1.5 ، 24
        /// </summary>
        public static string FormatFactor(decimal value)
        {
            value = Math.Round(value, 6, MidpointRounding.AwayFromZero);
            if (value == Math.Truncate(value))
                return ForDisplay(value.ToString("0", CultureInfo.InvariantCulture));

            string s = value.ToString("0.######", CultureInfo.InvariantCulture);
            s = s.TrimEnd('0');
            if (s.EndsWith(".", StringComparison.Ordinal))
                s = s.Substring(0, s.Length - 1);
            return ForDisplay(s);
        }

        /// <summary>تاريخ/وقت بأرقام إنجليزية.</summary>
        public static string FormatDate(DateTime dt, string format = "dd/MM/yyyy")
        {
            return ForDisplay(dt.ToString(format, CultureInfo.InvariantCulture));
        }

        public static string FormatDateTime(DateTime dt, string format = "dd/MM/yyyy HH:mm")
        {
            return ForDisplay(dt.ToString(format, CultureInfo.InvariantCulture));
        }

        public static bool TryParsePrice(string text, out decimal value)
        {
            return TryParseDecimal(text, out value, expectPriceSuffix: true);
        }

        public static bool TryParseDecimal(string text, out decimal value)
        {
            return TryParseDecimal(text, out value, expectPriceSuffix: false);
        }

        public static bool TryParseInteger(string text, out int value)
        {
            value = 0;
            if (!TryParseDecimal(text, out decimal d)) return false;
            if (d != Math.Truncate(d)) return false;
            if (d > int.MaxValue || d < int.MinValue) return false;
            value = (int)d;
            return true;
        }

        private static string FormatWithDecimals(decimal value, int decimalPlaces)
        {
            bool negative = value < 0;
            value = Math.Abs(value);
            value = Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);

            decimal scale = 1m;
            for (int i = 0; i < decimalPlaces; i++) scale *= 10m;

            long intPart = decimalPlaces > 0
                ? (long)Math.Truncate(value)
                : (long)value;

            int fracPart = 0;
            if (decimalPlaces > 0)
            {
                decimal fraction = value - intPart;
                fracPart = (int)Math.Round(fraction * scale, 0, MidpointRounding.AwayFromZero);
                if (fracPart >= (int)scale)
                {
                    intPart += 1;
                    fracPart = 0;
                }
            }

            var sb = new StringBuilder();
            if (negative) sb.Append('-');
            sb.Append(FormatThousands(intPart, insertLeadingGroup: true));
            if (decimalPlaces > 0)
            {
                sb.Append(DecimalSeparator);
                sb.Append(fracPart.ToString(new string('0', decimalPlaces), CultureInfo.InvariantCulture));
            }

            return ForDisplay(sb.ToString());
        }

        private static string FormatThousands(long intPart, bool insertLeadingGroup)
        {
            string digits = intPart.ToString(CultureInfo.InvariantCulture);
            if (digits.Length <= 3) return digits;

            var sb = new StringBuilder();
            int len = digits.Length;
            for (int i = 0; i < len; i++)
            {
                if (i > 0 && (len - i) % 3 == 0)
                    sb.Append(ThousandSeparator);
                sb.Append(digits[i]);
            }

            return sb.ToString();
        }

        private static bool TryParseDecimal(string text, out decimal value, bool expectPriceSuffix)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            text = StripForParse(text);
            text = text.Replace("ر.ي.‏", string.Empty).Replace(" ", string.Empty);
            text = text.Replace(",", string.Empty);

            bool negative = false;
            if (text.StartsWith("-", StringComparison.Ordinal))
            {
                negative = true;
                text = text.Substring(1);
            }
            else if (text.StartsWith("+", StringComparison.Ordinal))
            {
                text = text.Substring(1);
            }

            if (string.IsNullOrWhiteSpace(text)) return false;

            string[] parts = text.Split('.');
            if (parts.Length >= 2
                && parts[parts.Length - 1].Length == 2
                && parts[parts.Length - 1].All(char.IsDigit))
            {
                string dec = parts[parts.Length - 1];
                string integer = string.Concat(parts.Take(parts.Length - 1));
                if (string.IsNullOrEmpty(integer)) integer = "0";
                if (!long.TryParse(integer, NumberStyles.Integer, CultureInfo.InvariantCulture, out long intPart))
                    return false;
                if (!int.TryParse(dec, NumberStyles.Integer, CultureInfo.InvariantCulture, out int frac))
                    return false;

                value = intPart + frac / 100m;
                if (negative) value = -value;
                return true;
            }

            if (expectPriceSuffix) return false;

            string plain = text.Replace(".", string.Empty);
            if (!decimal.TryParse(plain, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return false;

            if (negative) value = -value;
            return true;
        }

        public static bool IsPriceColumn(string name, string headerText, string dataPropertyName, string format)
        {
            if (ContainsAny(name, PriceTokens) || ContainsAny(headerText, PriceTokens) || ContainsAny(dataPropertyName, PriceTokens))
                return true;

            if (!string.IsNullOrEmpty(format) &&
                (format.IndexOf("C", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 format.Equals("N2", StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }

        public static bool IsDateColumn(Type valueType, string name, string headerText, string dataPropertyName)
        {
            if (valueType == typeof(DateTime))
                return true;

            return ContainsAny(name, DateTokens)
                   || ContainsAny(headerText, DateTokens)
                   || ContainsAny(dataPropertyName, DateTokens);
        }

        public static bool IsFactorColumn(string name, string headerText, string dataPropertyName)
        {
            return ContainsAny(name, FactorTokens)
                   || ContainsAny(headerText, FactorTokens)
                   || ContainsAny(dataPropertyName, FactorTokens);
        }

        public static bool IsNumericColumn(Type valueType, string name, string headerText, string dataPropertyName, string format)
        {
            if (valueType == typeof(DateTime))
                return false;

            if (IsDateColumn(valueType, name, headerText, dataPropertyName))
                return false;

            if (IsFactorColumn(name, headerText, dataPropertyName))
                return true;

            if (valueType == typeof(byte) || valueType == typeof(sbyte) ||
                valueType == typeof(short) || valueType == typeof(ushort) ||
                valueType == typeof(int) || valueType == typeof(uint) ||
                valueType == typeof(long) || valueType == typeof(ulong) ||
                valueType == typeof(float) || valueType == typeof(double) ||
                valueType == typeof(decimal))
                return true;

            if (!string.IsNullOrEmpty(format) &&
                format.StartsWith("N", StringComparison.OrdinalIgnoreCase))
                return true;

            if (ContainsAny(name, NumericTokens) || ContainsAny(headerText, NumericTokens) || ContainsAny(dataPropertyName, NumericTokens))
                return true;

            return false;
        }

        public static bool IsPhoneOrIdColumn(string name, string headerText, string dataPropertyName)
        {
            if (ContainsAny(name, PhoneTokens) || ContainsAny(headerText, PhoneTokens) || ContainsAny(dataPropertyName, PhoneTokens))
                return true;

            if (ContainsAny(name, IdTokens) || ContainsAny(headerText, IdTokens) || ContainsAny(dataPropertyName, IdTokens))
                return true;

            return false;
        }

        public static int GetDecimalPlacesForColumn(bool isPrice, string name, string headerText, string dataPropertyName, string format)
        {
            if (isPrice) return 2;
            if (IsFactorColumn(name, headerText, dataPropertyName))
                return 0;
            if (!string.IsNullOrEmpty(format) && format.Length >= 2 && format[0] == 'N' &&
                int.TryParse(format.Substring(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n))
                return n;

            return 0;
        }

        private static bool ContainsAny(string haystack, string[] tokens)
        {
            if (string.IsNullOrEmpty(haystack)) return false;
            foreach (var token in tokens)
            {
                if (haystack.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        private static readonly string[] PriceTokens =
        {
            "price", "cost", "total", "amount", "paid", "discount", "net", "balance", "subtotal", "rest",
            "sell", "السعر", "مبلغ", "صافي", "خصم", "إجمالي", "مدفوع", "تكلفة", "رصيد", "المبلغ", "الصافي", "الخصم"
        };

        private static readonly string[] NumericTokens =
        {
            "qty", "quantity", "count", "stock", "factor", "pack", "level", "id", "كمية", "عدد", "مخزون", "معامل", "رقم", "معرف"
        };

        private static readonly string[] FactorTokens = { "factor", "معامل", "conversion" };

        private static readonly string[] DateTokens =
        {
            "date", "time", "created", "updated", "expiry", "validity",
            "تاريخ", "انتهاء", "صلاحية", "إضافة"
        };

        private static readonly string[] PhoneTokens = { "tel", "phone", "mobile", "هاتف", "جوال" };

        private static readonly string[] IdTokens = { "barcode", "باركود" };
    }
}
