using System.Drawing;
using System.Windows.Forms;

namespace Sales.Utilities
{
    /// <summary>
    /// رسم النص بأرقام لاتينية 0-9 حتى مع واجهة RTL وإعدادات Windows العربية.
    /// </summary>
    internal static class EnglishDigitTextRenderer
    {
        private const int EnglishLangId = 0x0409;

        private static readonly StringFormat LatinDigitFormat;

        static EnglishDigitTextRenderer()
        {
            LatinDigitFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            LatinDigitFormat.Trimming = StringTrimming.EllipsisCharacter;
            LatinDigitFormat.FormatFlags = StringFormatFlags.NoWrap;
            LatinDigitFormat.SetDigitSubstitution(EnglishLangId, StringDigitSubstitute.None);
        }

        public static void DrawCellText(Graphics graphics, string text, Font font, Rectangle bounds, Color foreColor, DataGridViewContentAlignment alignment)
        {
            if (graphics == null || string.IsNullOrEmpty(text)) return;

            text = SalesNumberFormat.ForDisplay(text);
            using (var brush = new SolidBrush(foreColor))
            {
                var sf = (StringFormat)LatinDigitFormat.Clone();
                MapAlignment(alignment, sf);
                graphics.DrawString(text, font, brush, bounds, sf);
            }
        }

        private static void MapAlignment(DataGridViewContentAlignment alignment, StringFormat sf)
        {
            switch (alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.MiddleLeft:
                case DataGridViewContentAlignment.BottomLeft:
                    sf.Alignment = StringAlignment.Near;
                    break;
                case DataGridViewContentAlignment.TopRight:
                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.BottomRight:
                    sf.Alignment = StringAlignment.Far;
                    break;
                default:
                    sf.Alignment = StringAlignment.Center;
                    break;
            }

            switch (alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.TopRight:
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case DataGridViewContentAlignment.BottomLeft:
                case DataGridViewContentAlignment.BottomCenter:
                case DataGridViewContentAlignment.BottomRight:
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                default:
                    sf.LineAlignment = StringAlignment.Center;
                    break;
            }
        }
    }
}
