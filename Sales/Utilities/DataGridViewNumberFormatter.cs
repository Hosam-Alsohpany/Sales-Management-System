// ============================================================
// الملف    : DataGridViewNumberFormatter.cs
// الغرض    : تنسيق وتعديل عرض الأرقام والعملات والنسب في الجداول
// ============================================================

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Sales.Utilities
{
    internal static class DataGridViewNumberFormatter
    {
        private const string AppliedMarker = "__Sales_NumFmt__";

        public static void Apply(DataGridView dgv)
        {
            if (dgv == null) return;

            string tag = dgv.Tag as string ?? string.Empty;
            if (tag.IndexOf(AppliedMarker, StringComparison.Ordinal) >= 0) return;

            dgv.Tag = tag + AppliedMarker;
            dgv.CellFormatting += OnCellFormatting;
            dgv.CellPainting += OnCellPainting;
            dgv.EditingControlShowing += OnEditingControlShowing;
            dgv.DataBindingComplete += OnDataBindingComplete;

            ClearBuiltInNumericFormats(dgv);
            try { dgv.Invalidate(); } catch { }
        }

        private static void OnDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;
            ClearBuiltInNumericFormats(dgv);
            try { dgv.Invalidate(); } catch { }
        }

        private static void OnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e?.Control is TextBox tb)
            {
                try
                {
                    tb.RightToLeft = RightToLeft.No;
                    NumericTextBoxHelper.ApplyToTextBox(tb);
                }
                catch { }
            }
        }

        private static void ClearBuiltInNumericFormats(DataGridView dgv)
        {
            if (dgv?.Columns == null) return;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col == null) continue;

                string fmt = col.DefaultCellStyle?.Format ?? string.Empty;
                if (string.IsNullOrEmpty(fmt)) continue;

                if (fmt.StartsWith("N", StringComparison.OrdinalIgnoreCase) ||
                    fmt.StartsWith("C", StringComparison.OrdinalIgnoreCase) ||
                    fmt.StartsWith("F", StringComparison.OrdinalIgnoreCase) ||
                    fmt.StartsWith("D", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        col.DefaultCellStyle.Format = string.Empty;
                        col.DefaultCellStyle.FormatProvider = CultureInfo.InvariantCulture;
                    }
                    catch { }
                }
            }
        }

        private static void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.Value == null || e.Value == DBNull.Value) return;

            string display;
            if (!TryFormatCellValue(sender as DataGridView, e.ColumnIndex, e.Value, out display))
                return;

            e.Value = display;
            e.FormattingApplied = true;
        }

        private static void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if ((e.State & DataGridViewElementStates.Displayed) == 0) return;

            var dgv = sender as DataGridView;
            if (dgv == null) return;

            object value = e.FormattedValue ?? e.Value;
            if (value == null || value == DBNull.Value) return;

            string display;
            if (!TryFormatCellValue(dgv, e.ColumnIndex, value, out display))
                return;

            e.Paint(e.ClipBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            var style = e.CellStyle ?? dgv.DefaultCellStyle;
            EnglishDigitTextRenderer.DrawCellText(
                e.Graphics,
                display,
                style.Font,
                e.CellBounds,
                style.ForeColor,
                style.Alignment);

            e.Handled = true;
        }

        private static bool TryFormatCellValue(DataGridView dgv, int columnIndex, object value, out string display)
        {
            display = null;
            if (dgv == null || columnIndex < 0 || columnIndex >= dgv.Columns.Count) return false;

            var col = dgv.Columns[columnIndex];
            if (col == null) return false;

            string name = col.Name ?? string.Empty;
            string header = col.HeaderText ?? string.Empty;
            string prop = col.DataPropertyName ?? string.Empty;
            string format = col.DefaultCellStyle?.Format ?? string.Empty;

            if (SalesNumberFormat.IsDateColumn(col.ValueType, name, header, prop))
                return false;

            if (SalesNumberFormat.IsPhoneOrIdColumn(name, header, prop))
            {
                display = SalesNumberFormat.ForDisplay(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
                return true;
            }

            bool isNumericColumn = SalesNumberFormat.IsNumericColumn(col.ValueType, name, header, prop, format);
            string asText = SalesNumberFormat.ToEnglishDigits(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);

            if (!isNumericColumn && !SalesNumberFormat.LooksLikeNumericText(asText))
                return false;

            if (!TryGetNumericValue(value, out decimal number))
            {
                display = SalesNumberFormat.ForDisplay(
                    SalesNumberFormat.LooksLikeNumericText(asText) ? asText : SalesNumberFormat.ToEnglishDigits(asText));
                return true;
            }

            if (SalesNumberFormat.IsFactorColumn(name, header, prop))
            {
                display = SalesNumberFormat.FormatFactor(number);
                return true;
            }

            bool isPrice = SalesNumberFormat.IsPriceColumn(name, header, prop, format);
            int decimals = SalesNumberFormat.GetDecimalPlacesForColumn(isPrice, name, header, prop, format);

            display = isPrice
                ? SalesNumberFormat.FormatPrice(number)
                : SalesNumberFormat.FormatNumber(number, decimals);

            return true;
        }

        private static bool TryGetNumericValue(object value, out decimal number)
        {
            number = 0;
            if (value == null || value == DBNull.Value) return false;

            switch (value)
            {
                case decimal d: number = d; return true;
                case double dbl: number = (decimal)dbl; return true;
                case float f: number = (decimal)f; return true;
                case long l: number = l; return true;
                case int i: number = i; return true;
                case short s: number = s; return true;
                case byte b: number = b; return true;
            }

            string text = SalesNumberFormat.StripForParse(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
            if (string.IsNullOrWhiteSpace(text)) return false;

            return SalesNumberFormat.TryParseDecimal(text, out number)
                   || SalesNumberFormat.TryParsePrice(text, out number);
        }
    }
}
