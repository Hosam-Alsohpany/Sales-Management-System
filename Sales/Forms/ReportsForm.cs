// ============================================================
// الملف    : ReportsForm.cs
// الغرض    : شاشة إدارة وطباعة تقارير المبيعات، المشتريات، والجرد
// ============================================================

using System;
using System.Linq;
using System.Windows.Forms;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly ReportsDataService _reports = new ReportsDataService();

        private enum ReportExportKind { Csv, Html, Pdf, Print }

        private sealed class ReportTabExportTag
        {
            public string Title { get; }
            public DataGridView Grid { get; }
            public ReportExportKind Kind { get; }
            public ReportTabExportTag(string title, DataGridView grid, ReportExportKind kind)
            {
                Title = title;
                Grid = grid;
                Kind = kind;
            }
        }

        public ReportsForm()
        {
            InitializeComponent();
            dtFrom.Value = DateTime.Today.AddDays(-30);
            dtTo.Value = DateTime.Today;
            try { UiTheme.ApplyToForm(this); } catch { }
        }

        private void ReportsForm_Load(object sender, EventArgs e) => LoadAllReports();

        private void btnRun_Click(object sender, EventArgs e) => LoadAllReports();

        private void LoadAllReports()
        {
            var from = dtFrom.Value.Date;
            var to = dtTo.Value.Date;

            dgvSales.DataSource = _reports.GetSalesByDay(from, to)
                .Select(r => new { الفترة = r.Period, الفواتير = r.OrderCount, الإجمالي = r.Total, الخصم = r.Discount, الضريبة = r.Tax }).ToList();

            int lowTh = AppSettingsManager.GetInt(AppSettingsManager.Keys.LowStockThreshold, 5);
            dgvStock.DataSource = _reports.GetStockReport(false, lowTh)
                .Select(r => new { r.ProductName, المخزون = r.DisplayQty, الحد = r.MinQty, التكلفة = r.CostPrice, البيع = r.SellPrice }).ToList();

            dgvProfit.DataSource = _reports.GetProfitReport(from, to)
                .Select(r => new { r.ProductName, الكمية = r.QtySold, الإيراد = r.Revenue, التكلفة = r.Cost, الربح = r.Profit }).ToList();

            dgvDebts.DataSource = _reports.GetDebtsReport()
                .Select(r => new { r.CustomerName, فواتير = r.TotalOrders, مدفوع = r.TotalPaid, المتبقي = r.Balance }).ToList();

            dgvTop.DataSource = _reports.GetTopProducts(from, to, 20, false)
                .Select(r => new { r.ProductName, الكمية = r.QtySold, الإيراد = r.Revenue }).ToList();

            dgvLeast.DataSource = _reports.GetTopProducts(from, to, 20, true)
                .Select(r => new { r.ProductName, الكمية = r.QtySold, الإيراد = r.Revenue }).ToList();
        }

        private void ReportTabExport_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is ReportTabExportTag tag) || tag.Grid == null) return;
            string caption = GetReportsSearchCaption();
            switch (tag.Kind)
            {
                case ReportExportKind.Csv:
                    ReportExportHelper.ExportGridToCsv(tag.Grid, tag.Title + ".csv");
                    break;
                case ReportExportKind.Html:
                    ReportExportHelper.ExportHtmlReport(tag.Title, tag.Grid, tag.Title + ".html");
                    break;
                case ReportExportKind.Pdf:
                    ReportExportHelper.ExportGridToPdf(tag.Title, tag.Grid, tag.Title + ".pdf", caption);
                    break;
                case ReportExportKind.Print:
                    ReportExportHelper.PrintGrid(tag.Title, tag.Grid, caption);
                    break;
            }
        }

        private string GetReportsSearchCaption()
        {
            return "من " + dtFrom.Value.ToString("yyyy-MM-dd") + " إلى " + dtTo.Value.ToString("yyyy-MM-dd");
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
