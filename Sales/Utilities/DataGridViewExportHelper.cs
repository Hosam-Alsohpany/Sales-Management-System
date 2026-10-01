// ============================================================
// الملف    : DataGridViewExportHelper.cs
// الغرض    : مساعد تصدير محتويات جداول البيانات إلى Excel أو CSV
// ============================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sales.Utilities
{
    public static class DataGridViewExportHelper
    {
        public static List<DataGridViewColumn> GetExportColumns(DataGridView dgv)
        {
            if (dgv == null) return new List<DataGridViewColumn>();
            return dgv.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c != null && c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();
        }

        public static List<DataGridViewRow> GetExportRows(DataGridView dgv)
        {
            var rows = new List<DataGridViewRow>();
            if (dgv == null) return rows;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row == null || row.IsNewRow) continue;
                if (!row.Visible) continue;
                rows.Add(row);
            }
            return rows;
        }

        public static void ExportToCsv(DataGridView dgv, string defaultFileName)
        {
            if (dgv == null) return;

            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV (*.csv)|*.csv";
                    sfd.FileName = string.IsNullOrWhiteSpace(defaultFileName) ? "export.csv" : defaultFileName;

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    var cols = GetExportColumns(dgv);
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Join(",", cols.Select(c => EscapeCsv(c.HeaderText))));

                    foreach (var row in GetExportRows(dgv))
                        sb.AppendLine(string.Join(",", cols.Select(c => EscapeCsv(Convert.ToString(row.Cells[c.Index].Value)))));

                    File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ أثناء التصدير", ex.Message);
            }
        }

        public static void ExportToPdf(DataGridView dgv, string title, string defaultFileName, string searchFilter = null)
        {
            ReportPdfExporter.ExportGridToPdf(title, dgv, defaultFileName, searchFilter);
        }

        public static void PrintGrid(DataGridView dgv, string title, string searchFilter = null)
        {
            if (dgv == null) return;

            var cols = GetExportColumns(dgv);
            var rows = GetExportRows(dgv);
            if (cols.Count == 0)
            {
                MessageHelper.ShowError("طباعة", "لا توجد أعمدة للطباعة.");
                return;
            }

            try
            {
                using (var doc = new PrintDocument())
                {
                    doc.DocumentName = title ?? "تقرير";
                    bool landscape = cols.Count > 5;
                    doc.DefaultPageSettings.Landscape = landscape;

                    int rowIndex = 0;
                    int pageNum = 0;

                    doc.PrintPage += (s, e) =>
                    {
                        pageNum++;
                        var g = e.Graphics;
                        float margin = 40f;
                        float y = margin;
                        float pageW = e.MarginBounds.Width;
                        float pageH = e.MarginBounds.Height;
                        float x0 = e.MarginBounds.Left;

                        using (var titleFont = new Font("Tahoma", 14f, FontStyle.Bold))
                        using (var metaFont = new Font("Tahoma", 9f))
                        using (var headerFont = new Font("Tahoma", 9f, FontStyle.Bold))
                        using (var cellFont = new Font("Tahoma", 8f))
                        {
                            var rtl = new StringFormat(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.NoWrap)
                            {
                                Alignment = StringAlignment.Far,
                                LineAlignment = StringAlignment.Center,
                                Trimming = StringTrimming.EllipsisCharacter
                            };
                            var rtlNear = new StringFormat(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.NoWrap)
                            {
                                Alignment = StringAlignment.Near,
                                LineAlignment = StringAlignment.Center,
                                Trimming = StringTrimming.EllipsisCharacter
                            };

                            if (pageNum == 1)
                            {
                                string store = AppSettingsManager.GetString(AppSettingsManager.Keys.StoreName, "Sales");
                                g.DrawString(store, titleFont, Brushes.DarkSlateGray, x0 + pageW, y, rtl);
                                y += titleFont.GetHeight(g) + 4;

                                g.DrawString(title ?? "تقرير", titleFont, Brushes.Black, x0 + pageW, y, rtl);
                                y += titleFont.GetHeight(g) + 2;

                                g.DrawString("التاريخ: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), metaFont, Brushes.Gray, x0 + pageW, y, rtl);
                                y += metaFont.GetHeight(g) + 2;

                                if (!string.IsNullOrWhiteSpace(searchFilter))
                                {
                                    g.DrawString("البحث: " + searchFilter.Trim(), metaFont, Brushes.DimGray, x0 + pageW, y, rtl);
                                    y += metaFont.GetHeight(g) + 2;
                                }

                                g.DrawString("عدد السجلات: " + rows.Count, metaFont, Brushes.DimGray, x0 + pageW, y, rtl);
                                y += metaFont.GetHeight(g) + 8;
                            }

                            float[] colW = ComputePrintColumnWidths(cols, pageW);
                            float headerH = headerFont.GetHeight(g) + 8;
                            float rowH = cellFont.GetHeight(g) + 6;

                            if (pageNum > 1 || y + headerH < margin + pageH)
                            {
                                float x = x0 + pageW;
                                g.FillRectangle(Brushes.Gainsboro, x0, y, pageW, headerH);
                                g.DrawRectangle(Pens.LightGray, x0, y, pageW, headerH);
                                for (int i = 0; i < cols.Count; i++)
                                {
                                    x -= colW[i];
                                    g.DrawString(cols[i].HeaderText ?? "", headerFont, Brushes.Black,
                                        new RectangleF(x, y, colW[i], headerH), rtl);
                                }
                                y += headerH;
                            }

                            while (rowIndex < rows.Count)
                            {
                                if (y + rowH > margin + pageH - 10)
                                {
                                    e.HasMorePages = true;
                                    return;
                                }

                                if (rowIndex % 2 == 1)
                                    g.FillRectangle(Brushes.WhiteSmoke, x0, y, pageW, rowH);

                                float x = x0 + pageW;
                                var row = rows[rowIndex];
                                for (int i = 0; i < cols.Count; i++)
                                {
                                    x -= colW[i];
                                    string text = Convert.ToString(row.Cells[cols[i].Index].Value) ?? "";
                                    g.DrawString(text, cellFont, Brushes.Black,
                                        new RectangleF(x + 2, y, colW[i] - 4, rowH), rtlNear);
                                }
                                y += rowH;
                                rowIndex++;
                            }

                            e.HasMorePages = false;
                        }
                    };

                    using (var dlg = new PrintDialog())
                    {
                        dlg.Document = doc;
                        dlg.UseEXDialog = true;
                        if (dlg.ShowDialog() != DialogResult.OK) return;
                    }

                    doc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ أثناء الطباعة", ex.Message);
            }
        }

        private static float[] ComputePrintColumnWidths(List<DataGridViewColumn> cols, float total)
        {
            var weights = cols.Select(c => (float)(c.Width > 0 ? c.Width : 80)).ToArray();
            float sum = weights.Sum();
            if (sum <= 0) sum = cols.Count;
            return weights.Select(w => total * (w / sum)).ToArray();
        }

        private static string EscapeCsv(string value)
        {
            value = value ?? string.Empty;
            value = value.Replace("\r", " ").Replace("\n", " ");

            bool mustQuote = value.Contains(",") || value.Contains("\"") || value.Contains(";");
            if (value.Contains("\""))
                value = value.Replace("\"", "\"\"");

            return mustQuote ? "\"" + value + "\"" : value;
        }
    }
}
