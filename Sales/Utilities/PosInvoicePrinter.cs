using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using Sales.Models;
using Sales.Repositories;

namespace Sales.Utilities
{
    /// <summary>
    /// طباعة فاتورة حرارية ~80mm.
    /// </summary>
    public static class PosInvoicePrinter
    {
        private static readonly List<string> _lines = new List<string>();
        private static int _lineIndex;

        public static void PrintOrder(int orderId, Order order, IEnumerable<SaleLine> lines, string storeName)
        {
            if (orderId <= 0 && (order == null || order.Id <= 0)) return;
            if (orderId <= 0) orderId = order.Id;

            _lines.Clear();
            string store = string.IsNullOrWhiteSpace(storeName) ? "Sales" : storeName.Trim();
            _lines.Add(store);
            _lines.Add("فاتورة #" + orderId);
            _lines.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
            _lines.Add(new string('-', 32));

            if (lines != null)
            {
                foreach (var l in lines)
                {
                    if (l == null) continue;
                    _lines.Add(Truncate(l.ProductName, 28));
                    _lines.Add("  " + SalesNumberFormat.FormatQuantity(l.Qty) + " x " +
                               SalesNumberFormat.FormatPrice(l.Price) + " = " +
                               SalesNumberFormat.FormatPrice(l.LineTotal));
                }
            }

            _lines.Add(new string('-', 32));
            if (order != null)
            {
                if (order.Discount > 0m)
                    _lines.Add("خصم: " + SalesNumberFormat.FormatPrice(order.Discount));
                if (order.TaxAmount > 0m)
                    _lines.Add("ضريبة: " + SalesNumberFormat.FormatPrice(order.TaxAmount));
                _lines.Add("الإجمالي: " + SalesNumberFormat.FormatPrice(order.Total));
            }
            _lines.Add("");
            _lines.Add("شكراً لزيارتكم");

            _lineIndex = 0;
            using (var doc = new PrintDocument())
            {
                doc.DocumentName = "Invoice_" + orderId;
                try
                {
                    doc.DefaultPageSettings.PaperSize = new PaperSize("Receipt80", 280, 600);
                    doc.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);
                }
                catch { }

                doc.PrintPage += Doc_PrintPage;
                doc.Print();
            }
        }

        private static void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float y = 0;
            float left = 0;
            using (var font = new Font("Arial", 9f))
            using (var fontBold = new Font("Arial", 10f, FontStyle.Bold))
            {
                float lineH = font.GetHeight(g) + 2;
                while (_lineIndex < _lines.Count)
                {
                    var f = _lineIndex <= 2 ? fontBold : font;
                    g.DrawString(_lines[_lineIndex], f, Brushes.Black, left, y);
                    y += lineH;
                    _lineIndex++;
                    if (y > e.MarginBounds.Height - lineH)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                }
            }
            e.HasMorePages = false;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Length <= max ? s : s.Substring(0, max - 1) + "…";
        }
    }
}
