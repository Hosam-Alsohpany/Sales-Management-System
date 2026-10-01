// ============================================================
// الملف    : ReportExportHelper.cs
// الغرض    : تصدير تقارير الجرد والمبيعات بصيغ قابلة للطباعة والتداول
// ============================================================

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sales.Utilities
{
    public static class ReportExportHelper
    {
        public static void ExportGridToCsv(DataGridView dgv, string defaultName)
        {
            DataGridViewExportHelper.ExportToCsv(dgv, defaultName);
        }

        public static void ExportGridToPdf(string title, DataGridView dgv, string defaultName, string searchFilter = null)
        {
            DataGridViewExportHelper.ExportToPdf(dgv, title, defaultName, searchFilter);
        }

        public static void PrintGrid(string title, DataGridView dgv, string searchFilter = null)
        {
            DataGridViewExportHelper.PrintGrid(dgv, title, searchFilter);
        }

        public static void ExportHtmlReport(string title, DataGridView dgv, string defaultName)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "HTML (*.html)|*.html";
                sfd.FileName = string.IsNullOrWhiteSpace(defaultName) ? "report.html" : defaultName;
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var sb = new StringBuilder();
                sb.AppendLine("<!DOCTYPE html><html dir='rtl' lang='ar'><head><meta charset='utf-8'/><title>" +
                              Escape(title) + "</title>");
                sb.AppendLine("<style>body{font-family:Segoe UI,Arial;margin:20px}table{border-collapse:collapse;width:100%}th,td{border:1px solid #ccc;padding:6px;text-align:center}th{background:#eee}</style></head><body>");
                sb.AppendLine("<h2>" + Escape(title) + "</h2>");
                sb.AppendLine("<p>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>");
                sb.AppendLine("<table><thead><tr>");

                var cols = DataGridViewExportHelper.GetExportColumns(dgv);
                foreach (var col in cols)
                    sb.Append("<th>").Append(Escape(col.HeaderText)).Append("</th>");
                sb.AppendLine("</tr></thead><tbody>");

                foreach (var row in DataGridViewExportHelper.GetExportRows(dgv))
                {
                    sb.AppendLine("<tr>");
                    foreach (var col in cols)
                    {
                        object v = row.Cells[col.Index].Value;
                        sb.Append("<td>").Append(Escape(v?.ToString() ?? "")).Append("</td>");
                    }
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</tbody></table></body></html>");
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            }
        }

        private static string Escape(string s)
        {
            if (s == null) return string.Empty;
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}
