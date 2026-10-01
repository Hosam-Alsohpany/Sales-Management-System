using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Sales.Utilities
{
    /// <summary>
    /// تصدير جداول إلى PDF مع دعم العربية (RTL) عبر iTextSharp.
    /// </summary>
    public static class ReportPdfExporter
    {
        public static void ExportGridToPdf(string title, DataGridView dgv, string defaultName, string searchFilter = null)
        {
            if (dgv == null) return;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = string.IsNullOrWhiteSpace(defaultName)
                    ? "report.pdf"
                    : Path.ChangeExtension(defaultName, ".pdf");
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    WritePdf(sfd.FileName, title, dgv, searchFilter);
                    MessageBox.Show("تم حفظ ملف PDF بنجاح.", "تصدير PDF",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل تصدير PDF: " + ex.Message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.LogError("فشل تصدير PDF", ex);
                }
            }
        }

        private static void WritePdf(string path, string title, DataGridView dgv, string searchFilter)
        {
            var columns = DataGridViewExportHelper.GetExportColumns(dgv);
            var rows = DataGridViewExportHelper.GetExportRows(dgv);
            if (columns.Count == 0)
                throw new InvalidOperationException("لا توجد أعمدة للتصدير");

            BaseFont bf = LoadArabicBaseFont();
            var fontTitle = new Font(bf, 14, Font.BOLD);
            var fontMeta = new Font(bf, 9, Font.NORMAL);
            var fontHeader = new Font(bf, 9, Font.BOLD);
            var fontCell = new Font(bf, 8, Font.NORMAL);

            bool landscape = columns.Count > 5;
            var pageSize = landscape ? PageSize.A4.Rotate() : PageSize.A4;

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var document = new Document(pageSize, 36, 36, 48, 36))
            {
                PdfWriter.GetInstance(document, stream);
                document.AddCreator("Sales");
                document.AddTitle(title ?? "Report");
                document.Open();

                string store = AppSettingsManager.GetString(AppSettingsManager.Keys.StoreName, "Sales");
                AddRtlParagraph(document, store, fontTitle, Element.ALIGN_RIGHT);
                AddRtlParagraph(document, title ?? "تقرير", fontTitle, Element.ALIGN_RIGHT);
                AddRtlParagraph(document,
                    "التاريخ: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    fontMeta, Element.ALIGN_RIGHT);

                if (!string.IsNullOrWhiteSpace(searchFilter))
                    AddRtlParagraph(document, "البحث: " + searchFilter.Trim(), fontMeta, Element.ALIGN_RIGHT);

                AddRtlParagraph(document, "عدد السجلات: " + rows.Count, fontMeta, Element.ALIGN_RIGHT);
                document.Add(new Paragraph(" "));

                var widths = ComputeRelativeWidths(columns);
                var table = new PdfPTable(columns.Count)
                {
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    WidthPercentage = 100f
                };
                table.SetWidths(widths);

                foreach (var col in columns)
                {
                    var cell = new PdfPCell(new Phrase(col.HeaderText ?? "", fontHeader))
                    {
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        BackgroundColor = new BaseColor(230, 230, 230),
                        Padding = 4f
                    };
                    table.AddCell(cell);
                }

                int rowNum = 0;
                foreach (var row in rows)
                {
                    rowNum++;
                    var bg = rowNum % 2 == 0 ? new BaseColor(245, 245, 245) : BaseColor.WHITE;
                    foreach (var col in columns)
                    {
                        string text = Convert.ToString(row.Cells[col.Index].Value) ?? "";
                        if (text.Length > 120) text = text.Substring(0, 117) + "...";
                        var cell = new PdfPCell(new Phrase(text, fontCell))
                        {
                            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            BackgroundColor = bg,
                            Padding = 3f
                        };
                        table.AddCell(cell);
                    }
                }

                document.Add(table);
                document.Close();
            }
        }

        private static void AddRtlParagraph(Document doc, string text, Font font, int alignment)
        {
            var p = new Paragraph(text ?? "", font) { Alignment = alignment };
            doc.Add(p);
        }

        private static BaseFont LoadArabicBaseFont()
        {
            string fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string[] candidates =
            {
                Path.Combine(fontsDir, "tahoma.ttf"),
                Path.Combine(fontsDir, "arial.ttf"),
                Path.Combine(fontsDir, "segoeui.ttf"),
                Path.Combine(fontsDir, "times.ttf")
            };

            foreach (var path in candidates)
            {
                if (!File.Exists(path)) continue;
                try
                {
                    return BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                }
                catch { }
            }

            return BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        }

        private static float[] ComputeRelativeWidths(List<DataGridViewColumn> columns)
        {
            var weights = columns.Select(c => (float)(c.Width > 0 ? c.Width : 80)).ToArray();
            float sum = weights.Sum();
            if (sum <= 0) sum = columns.Count;
            return weights.Select(w => w / sum).ToArray();
        }
    }
}
