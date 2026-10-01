// ============================================================
// الملف    : DashboardForm.cs
// الغرض    : لوحة التحكم الرئيسية لعرض إحصائيات المبيعات والأرباح
// ============================================================

using System;

using System.Drawing;

using System.Linq;

using System.Windows.Forms;

using Sales.Utilities;



namespace Sales.Forms

{

    public partial class DashboardForm : Form

    {

        private readonly DashboardDataService _data = new DashboardDataService();



        public DashboardForm()

        {

            InitializeComponent();

            try { UiTheme.ApplyToForm(this); } catch { }

            try { UiTheme.ApplyGridStyle(dgvTop); } catch { }

            try { UiTheme.ApplyGridStyle(dgvLow); } catch { }

        }



        private void DashboardForm_Load(object sender, EventArgs e)

        {

            string store = AppSettingsManager.GetString(AppSettingsManager.Keys.StoreName, "");

            if (!string.IsNullOrWhiteSpace(store))

                Text = "لوحة التحكم — " + store.Trim();

            RefreshData();

        }



        private void RefreshData()

        {

            int lowTh = AppSettingsManager.GetInt(AppSettingsManager.Keys.LowStockThreshold, 5);

            var k = _data.GetKpis(lowTh);



            lblKpiTodayVal.Text = SalesNumberFormat.FormatPrice(k.SalesToday);

            lblKpiOrdersVal.Text = k.OrdersToday.ToString();

            lblKpiWeekVal.Text = SalesNumberFormat.FormatPrice(_data.GetSalesForPeriod(DateTime.Today.AddDays(-6), DateTime.Today));

            lblKpiMonthVal.Text = SalesNumberFormat.FormatPrice(_data.GetSalesForPeriod(DateTime.Today.AddDays(-29), DateTime.Today));

            lblKpiDebtsVal.Text = SalesNumberFormat.FormatPrice(k.OutstandingTotal);

            lblKpiLowStockVal.Text = k.LowStockCount + " / " + k.OutOfStockCount;



            var points = _data.GetSalesLastDays(7);

            var s = chartSales.Series["sales"];

            s.Points.Clear();

            foreach (var p in points)

                s.Points.AddXY(p.Day.ToString("MM/dd"), (double)p.Value);



            var top = _data.GetTopProducts(10);

            dgvTop.DataSource = top.Select(t => new { t.ProductName, الكمية = t.QtySold, الإيراد = t.Revenue }).ToList();



            var low = _data.GetLowStockProducts(lowTh);

            dgvLow.DataSource = low.Select(l => new { l.ProductName, المخزون = l.DisplayQty, الحد = l.MinQty }).ToList();



            lvActivities.Items.Clear();

            foreach (var a in _data.GetLatestActivities(12))

            {

                var item = new ListViewItem(a.When.ToString("MM/dd HH:mm"));

                item.SubItems.Add(a.Title ?? "");

                item.SubItems.Add(a.Details ?? "");

                lvActivities.Items.Add(item);

            }



            lvAlerts.Items.Clear();

            foreach (var a in _data.GetAlerts(lowTh))

            {

                var item = new ListViewItem(a.Severity ?? "");

                item.SubItems.Add(a.Title ?? "");

                item.SubItems.Add(a.Details ?? "");

                if (a.Severity == "Critical") item.ForeColor = Color.Firebrick;

                else if (a.Severity == "Warning") item.ForeColor = Color.DarkOrange;

                lvAlerts.Items.Add(item);

            }

        }



        private void btnRefresh_Click(object sender, EventArgs e) => RefreshData();



        private void btnExport_Click(object sender, EventArgs e)

        {

            DataGridViewExportHelper.ExportToPdf(dgvTop, "لوحة التحكم - أكثر المنتجات", "dashboard_top.pdf");

        }



        private void btnPrint_Click(object sender, EventArgs e)

        {

            DataGridViewExportHelper.PrintGrid(dgvTop, "لوحة التحكم - أكثر المنتجات");

        }



        private void btnClose_Click(object sender, EventArgs e) => Close();

    }

}


