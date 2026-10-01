// ============================================================
// الملف    : NumericTextBoxHelper.cs
// الغرض    : منع إدخال الرموز غير الرقمية وضمان صحة مدخلات النصوص
// ============================================================

using System;
using System.Windows.Forms;

namespace Sales.Utilities
{
    internal static class NumericTextBoxHelper
    {
        private const string AppliedMarker = "__Sales_NumTxt__";

        public static void ApplyToTextBox(TextBox textBox)
        {
            if (textBox == null) return;

            string tag = textBox.Tag as string ?? string.Empty;
            if (tag.IndexOf(AppliedMarker, StringComparison.Ordinal) >= 0) return;

            textBox.Tag = tag + AppliedMarker;
            try { textBox.RightToLeft = RightToLeft.No; } catch { }

            bool priceField = IsPriceField(textBox.Name);
            bool internalChange = false;

            textBox.TextChanged += (s, e) =>
            {
                if (internalChange) return;

                string normalized = SalesNumberFormat.ToEnglishDigits(textBox.Text);
                if (string.Equals(normalized, textBox.Text, StringComparison.Ordinal))
                    return;

                int sel = textBox.SelectionStart;
                internalChange = true;
                textBox.Text = normalized;
                textBox.SelectionStart = Math.Min(sel, textBox.Text.Length);
                internalChange = false;
            };

            if (priceField)
            {
                textBox.Leave += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text)) return;
                    if (SalesNumberFormat.TryParsePrice(textBox.Text, out decimal v) ||
                        SalesNumberFormat.TryParseDecimal(textBox.Text, out v))
                    {
                        bool wasInternal = internalChange;
                        internalChange = true;
                        textBox.Text = SalesNumberFormat.FormatPrice(v);
                        internalChange = wasInternal;
                    }
                };
            }

            try
            {
                string current = textBox.Text;
                if (SalesNumberFormat.ContainsNonEnglishDigits(current))
                {
                    internalChange = true;
                    textBox.Text = SalesNumberFormat.ToEnglishDigits(current);
                    internalChange = false;
                }
            }
            catch { }
        }

        public static void ApplyToMaskedTextBox(MaskedTextBox textBox)
        {
            if (textBox == null) return;

            string tag = textBox.Tag as string ?? string.Empty;
            if (tag.IndexOf(AppliedMarker, StringComparison.Ordinal) >= 0) return;

            textBox.Tag = tag + AppliedMarker;
            try { textBox.RightToLeft = RightToLeft.No; } catch { }
            bool internalChange = false;

            textBox.TextChanged += (s, e) =>
            {
                if (internalChange) return;

                string normalized = SalesNumberFormat.ToEnglishDigits(textBox.Text);
                if (string.Equals(normalized, textBox.Text, StringComparison.Ordinal))
                    return;

                int sel = textBox.SelectionStart;
                internalChange = true;
                textBox.Text = normalized;
                textBox.SelectionStart = Math.Min(sel, textBox.Text.Length);
                internalChange = false;
            };
        }

        public static bool IsPriceField(string controlName)
        {
            if (string.IsNullOrEmpty(controlName)) return false;
            return SalesNumberFormat.IsPriceColumn(controlName, controlName, controlName, string.Empty);
        }

        public static bool LooksNumericTextBox(TextBox textBox)
        {
            if (textBox == null) return false;
            if (IsPriceField(textBox.Name)) return true;

            string name = textBox.Name ?? string.Empty;
            if (name.IndexOf("qty", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("Qty", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("count", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("ID", StringComparison.Ordinal) >= 0 && name.Length <= 6) return true;
            if (name.IndexOf("discount", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("paid", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("total", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("amount", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("factor", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("pack", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (name.IndexOf("delta", StringComparison.OrdinalIgnoreCase) >= 0) return true;

            return SalesNumberFormat.LooksLikeNumericText(textBox.Text);
        }
    }
}
