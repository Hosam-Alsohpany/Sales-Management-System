// ============================================================
// الملف    : DataGridViewDateTimeFormatter.cs
// الغرض    : تنسيق وعرض أعمدة التواريخ بالشكل المناسب في جداول البيانات
// ============================================================

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Sales.Utilities
{
    internal static class DataGridViewDateTimeFormatter
    {
        private const string DefaultFormat = "dd/MM/yyyy hh:mm tt";
        private const string AppliedMarker = "__Sales_DateFmt__";

        public static void Apply(DataGridView dgv, string format = null)
        {
            if (dgv == null) return;

            string tag = dgv.Tag as string ?? string.Empty;
            if (tag.IndexOf(AppliedMarker, StringComparison.Ordinal) >= 0) return;

            dgv.Tag = tag + AppliedMarker;
            string fmt = format ?? DefaultFormat;
            dgv.CellFormatting += (s, e) => OnCellFormatting(dgv, e, fmt);
            dgv.CellPainting += (s, e) => OnCellPainting(dgv, e, fmt);
            DataGridViewNumberFormatter.Apply(dgv);
        }

        private static void OnCellFormatting(DataGridView dgv, DataGridViewCellFormattingEventArgs e, string format)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.Value == null || e.Value == DBNull.Value) return;

            string display;
            if (!TryFormatDateValue(dgv, e.ColumnIndex, e.Value, format, out display))
                return;

            e.Value = display;
            e.FormattingApplied = true;
        }

        private static void OnCellPainting(DataGridView dgv, DataGridViewCellPaintingEventArgs e, string format)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if ((e.State & DataGridViewElementStates.Displayed) == 0) return;

            object value = e.FormattedValue ?? e.Value;
            if (value == null || value == DBNull.Value) return;

            string display;
            if (!TryFormatDateValue(dgv, e.ColumnIndex, value, format, out display))
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

        private static bool TryFormatDateValue(DataGridView dgv, int columnIndex, object value, string format, out string display)
        {
            display = null;
            if (dgv == null || columnIndex < 0 || columnIndex >= dgv.Columns.Count) return false;

            var col = dgv.Columns[columnIndex];
            if (col == null) return false;

            string name = col.Name ?? string.Empty;
            string header = col.HeaderText ?? string.Empty;
            string prop = col.DataPropertyName ?? string.Empty;

            if (!SalesNumberFormat.IsDateColumn(col.ValueType, name, header, prop))
                return false;

            DateTime dt;
            if (value is DateTime time)
            {
                dt = time;
            }
            else
            {
                var text = SalesNumberFormat.StripForParse(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
                if (string.IsNullOrWhiteSpace(text)) return false;

                if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dt) &&
                    !DateTime.TryParse(text, SalesNumberFormat.AppCulture, DateTimeStyles.AllowWhiteSpaces, out dt))
                {
                    return false;
                }
            }

            display = format.IndexOf("HH", StringComparison.OrdinalIgnoreCase) >= 0
                      || format.IndexOf("hh", StringComparison.OrdinalIgnoreCase) >= 0
                ? SalesNumberFormat.FormatDateTime(dt, format)
                : SalesNumberFormat.FormatDate(dt, format);

            return true;
        }
    }
}
