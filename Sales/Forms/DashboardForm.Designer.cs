namespace Sales.Forms
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutKpi;
        private System.Windows.Forms.Panel cardToday;
        private System.Windows.Forms.Panel accentToday;
        private System.Windows.Forms.Label lblKpiTodayTitle;
        private System.Windows.Forms.Label lblKpiTodayVal;
        private System.Windows.Forms.Panel cardOrders;
        private System.Windows.Forms.Panel accentOrders;
        private System.Windows.Forms.Label lblKpiOrdersTitle;
        private System.Windows.Forms.Label lblKpiOrdersVal;
        private System.Windows.Forms.Panel cardWeek;
        private System.Windows.Forms.Panel accentWeek;
        private System.Windows.Forms.Label lblKpiWeekTitle;
        private System.Windows.Forms.Label lblKpiWeekVal;
        private System.Windows.Forms.Panel cardMonth;
        private System.Windows.Forms.Panel accentMonth;
        private System.Windows.Forms.Label lblKpiMonthTitle;
        private System.Windows.Forms.Label lblKpiMonthVal;
        private System.Windows.Forms.Panel cardDebts;
        private System.Windows.Forms.Panel accentDebts;
        private System.Windows.Forms.Label lblKpiDebtsTitle;
        private System.Windows.Forms.Label lblKpiDebtsVal;
        private System.Windows.Forms.Panel cardLowStock;
        private System.Windows.Forms.Panel accentLowStock;
        private System.Windows.Forms.Label lblKpiLowStockTitle;
        private System.Windows.Forms.Label lblKpiLowStockVal;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSales;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SplitContainer splitSide;
        private System.Windows.Forms.Panel panelAlertsWrap;
        private System.Windows.Forms.Label lblAlertsTitle;
        private System.Windows.Forms.ListView lvAlerts;
        private System.Windows.Forms.ColumnHeader colAlertSeverity;
        private System.Windows.Forms.ColumnHeader colAlertTitle;
        private System.Windows.Forms.ColumnHeader colAlertDetails;
        private System.Windows.Forms.Panel panelActivitiesWrap;
        private System.Windows.Forms.Label lblActivitiesTitle;
        private System.Windows.Forms.ListView lvActivities;
        private System.Windows.Forms.ColumnHeader colActTime;
        private System.Windows.Forms.ColumnHeader colActTitle;
        private System.Windows.Forms.ColumnHeader colActDetails;
        private System.Windows.Forms.SplitContainer splitGrids;
        private System.Windows.Forms.Panel panelTopProductsWrap;
        private System.Windows.Forms.Label lblTopProductsTitle;
        private System.Windows.Forms.DataGridView dgvTop;
        private System.Windows.Forms.Panel panelLowStockWrap;
        private System.Windows.Forms.Label lblLowStockTitle;
        private System.Windows.Forms.DataGridView dgvLow;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tableLayoutKpi = new System.Windows.Forms.TableLayoutPanel();
            this.cardToday = new System.Windows.Forms.Panel();
            this.accentToday = new System.Windows.Forms.Panel();
            this.lblKpiTodayTitle = new System.Windows.Forms.Label();
            this.lblKpiTodayVal = new System.Windows.Forms.Label();
            this.cardOrders = new System.Windows.Forms.Panel();
            this.accentOrders = new System.Windows.Forms.Panel();
            this.lblKpiOrdersTitle = new System.Windows.Forms.Label();
            this.lblKpiOrdersVal = new System.Windows.Forms.Label();
            this.cardWeek = new System.Windows.Forms.Panel();
            this.accentWeek = new System.Windows.Forms.Panel();
            this.lblKpiWeekTitle = new System.Windows.Forms.Label();
            this.lblKpiWeekVal = new System.Windows.Forms.Label();
            this.cardMonth = new System.Windows.Forms.Panel();
            this.accentMonth = new System.Windows.Forms.Panel();
            this.lblKpiMonthTitle = new System.Windows.Forms.Label();
            this.lblKpiMonthVal = new System.Windows.Forms.Label();
            this.cardDebts = new System.Windows.Forms.Panel();
            this.accentDebts = new System.Windows.Forms.Panel();
            this.lblKpiDebtsTitle = new System.Windows.Forms.Label();
            this.lblKpiDebtsVal = new System.Windows.Forms.Label();
            this.cardLowStock = new System.Windows.Forms.Panel();
            this.accentLowStock = new System.Windows.Forms.Panel();
            this.lblKpiLowStockTitle = new System.Windows.Forms.Label();
            this.lblKpiLowStockVal = new System.Windows.Forms.Label();
            this.chartSales = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitSide = new System.Windows.Forms.SplitContainer();
            this.panelAlertsWrap = new System.Windows.Forms.Panel();
            this.lblAlertsTitle = new System.Windows.Forms.Label();
            this.lvAlerts = new System.Windows.Forms.ListView();
            this.colAlertSeverity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAlertTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAlertDetails = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelActivitiesWrap = new System.Windows.Forms.Panel();
            this.lblActivitiesTitle = new System.Windows.Forms.Label();
            this.lvActivities = new System.Windows.Forms.ListView();
            this.colActTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActDetails = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitGrids = new System.Windows.Forms.SplitContainer();
            this.panelTopProductsWrap = new System.Windows.Forms.Panel();
            this.lblTopProductsTitle = new System.Windows.Forms.Label();
            this.dgvTop = new System.Windows.Forms.DataGridView();
            this.panelLowStockWrap = new System.Windows.Forms.Panel();
            this.lblLowStockTitle = new System.Windows.Forms.Label();
            this.dgvLow = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            this.tableLayoutKpi.SuspendLayout();
            this.cardToday.SuspendLayout();
            this.cardOrders.SuspendLayout();
            this.cardWeek.SuspendLayout();
            this.cardMonth.SuspendLayout();
            this.cardDebts.SuspendLayout();
            this.cardLowStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitSide)).BeginInit();
            this.splitSide.Panel1.SuspendLayout();
            this.splitSide.Panel2.SuspendLayout();
            this.splitSide.SuspendLayout();
            this.panelAlertsWrap.SuspendLayout();
            this.panelActivitiesWrap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGrids)).BeginInit();
            this.splitGrids.Panel1.SuspendLayout();
            this.splitGrids.Panel2.SuspendLayout();
            this.splitGrids.SuspendLayout();
            this.panelTopProductsWrap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).BeginInit();
            this.panelLowStockWrap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLow)).BeginInit();
            this.SuspendLayout();
            // panelTop
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.btnExport);
            this.panelTop.Controls.Add(this.btnPrint);
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelTop.Location = new System.Drawing.Point(0, 576);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 44);
            this.panelTop.TabIndex = 0;
            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(300, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // btnExport
            this.btnExport.Location = new System.Drawing.Point(200, 8);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 28);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "تصدير PDF";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // btnPrint
            this.btnPrint.Location = new System.Drawing.Point(100, 8);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 28);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // btnClose
            this.btnClose.Location = new System.Drawing.Point(10, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "إغلاق";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // tableLayoutKpi
            this.tableLayoutKpi.ColumnCount = 6;
            this.tableLayoutKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutKpi.Controls.Add(this.cardToday, 0, 0);
            this.tableLayoutKpi.Controls.Add(this.cardOrders, 1, 0);
            this.tableLayoutKpi.Controls.Add(this.cardWeek, 2, 0);
            this.tableLayoutKpi.Controls.Add(this.cardMonth, 3, 0);
            this.tableLayoutKpi.Controls.Add(this.cardDebts, 4, 0);
            this.tableLayoutKpi.Controls.Add(this.cardLowStock, 5, 0);
            this.tableLayoutKpi.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutKpi.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutKpi.Name = "tableLayoutKpi";
            this.tableLayoutKpi.Padding = new System.Windows.Forms.Padding(6);
            this.tableLayoutKpi.RowCount = 1;
            this.tableLayoutKpi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutKpi.Size = new System.Drawing.Size(900, 88);
            this.tableLayoutKpi.TabIndex = 1;
            this.SetupKpiCard(this.cardToday, this.accentToday, this.lblKpiTodayTitle, this.lblKpiTodayVal, "مبيعات اليوم", System.Drawing.Color.FromArgb(33, 150, 243));
            this.SetupKpiCard(this.cardOrders, this.accentOrders, this.lblKpiOrdersTitle, this.lblKpiOrdersVal, "فواتير اليوم", System.Drawing.Color.FromArgb(156, 39, 176));
            this.SetupKpiCard(this.cardWeek, this.accentWeek, this.lblKpiWeekTitle, this.lblKpiWeekVal, "7 أيام", System.Drawing.Color.FromArgb(76, 175, 80));
            this.SetupKpiCard(this.cardMonth, this.accentMonth, this.lblKpiMonthTitle, this.lblKpiMonthVal, "30 يوم", System.Drawing.Color.FromArgb(255, 152, 0));
            this.SetupKpiCard(this.cardDebts, this.accentDebts, this.lblKpiDebtsTitle, this.lblKpiDebtsVal, "الديون", System.Drawing.Color.FromArgb(244, 67, 54));
            this.SetupKpiCard(this.cardLowStock, this.accentLowStock, this.lblKpiLowStockTitle, this.lblKpiLowStockVal, "مخزون منخفض", System.Drawing.Color.FromArgb(255, 193, 7));
            // chartSales
            chartArea1.Name = "main";
            this.chartSales.ChartAreas.Add(chartArea1);
            this.chartSales.Dock = System.Windows.Forms.DockStyle.Top;
            this.chartSales.Location = new System.Drawing.Point(0, 88);
            this.chartSales.Name = "chartSales";
            series1.ChartArea = "main";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            series1.Color = System.Drawing.Color.FromArgb(33, 150, 243);
            series1.Name = "sales";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            this.chartSales.Series.Add(series1);
            this.chartSales.Size = new System.Drawing.Size(900, 240);
            this.chartSales.TabIndex = 2;
            title1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            title1.ForeColor = System.Drawing.Color.DimGray;
            title1.Name = "Title1";
            title1.Text = "مبيعات آخر 7 أيام";
            this.chartSales.Titles.Add(title1);
            // splitMain
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 328);
            this.splitMain.Name = "splitMain";
            // splitMain.Panel1
            this.splitMain.Panel1.Controls.Add(this.splitSide);
            this.splitMain.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // splitMain.Panel2
            this.splitMain.Panel2.Controls.Add(this.splitGrids);
            this.splitMain.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitMain.Size = new System.Drawing.Size(900, 248);
            this.splitMain.SplitterDistance = 320;
            this.splitMain.TabIndex = 3;
            // splitSide
            this.splitSide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitSide.Location = new System.Drawing.Point(0, 0);
            this.splitSide.Name = "splitSide";
            this.splitSide.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // splitSide.Panel1
            this.splitSide.Panel1.Controls.Add(this.panelAlertsWrap);
            // splitSide.Panel2
            this.splitSide.Panel2.Controls.Add(this.panelActivitiesWrap);
            this.splitSide.Size = new System.Drawing.Size(320, 248);
            this.splitSide.SplitterDistance = 120;
            this.splitSide.TabIndex = 0;
            // panelAlertsWrap
            this.panelAlertsWrap.Controls.Add(this.lvAlerts);
            this.panelAlertsWrap.Controls.Add(this.lblAlertsTitle);
            this.panelAlertsWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAlertsWrap.Location = new System.Drawing.Point(0, 0);
            this.panelAlertsWrap.Name = "panelAlertsWrap";
            this.panelAlertsWrap.Size = new System.Drawing.Size(320, 120);
            this.panelAlertsWrap.TabIndex = 0;
            // lblAlertsTitle
            this.lblAlertsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAlertsTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAlertsTitle.Location = new System.Drawing.Point(0, 0);
            this.lblAlertsTitle.Name = "lblAlertsTitle";
            this.lblAlertsTitle.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lblAlertsTitle.Size = new System.Drawing.Size(320, 26);
            this.lblAlertsTitle.TabIndex = 0;
            this.lblAlertsTitle.Text = "تنبيهات";
            this.lblAlertsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // lvAlerts
            this.lvAlerts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAlertSeverity,
            this.colAlertTitle,
            this.colAlertDetails});
            this.lvAlerts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAlerts.FullRowSelect = true;
            this.lvAlerts.GridLines = true;
            this.lvAlerts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvAlerts.HideSelection = false;
            this.lvAlerts.Location = new System.Drawing.Point(0, 26);
            this.lvAlerts.Name = "lvAlerts";
            this.lvAlerts.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lvAlerts.RightToLeftLayout = true;
            this.lvAlerts.Size = new System.Drawing.Size(320, 94);
            this.lvAlerts.TabIndex = 1;
            this.lvAlerts.UseCompatibleStateImageBehavior = false;
            this.lvAlerts.View = System.Windows.Forms.View.Details;
            this.colAlertSeverity.Text = "النوع";
            this.colAlertSeverity.Width = 70;
            this.colAlertTitle.Text = "العنوان";
            this.colAlertTitle.Width = 120;
            this.colAlertDetails.Text = "التفاصيل";
            this.colAlertDetails.Width = 110;
            // panelActivitiesWrap
            this.panelActivitiesWrap.Controls.Add(this.lvActivities);
            this.panelActivitiesWrap.Controls.Add(this.lblActivitiesTitle);
            this.panelActivitiesWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelActivitiesWrap.Location = new System.Drawing.Point(0, 0);
            this.panelActivitiesWrap.Name = "panelActivitiesWrap";
            this.panelActivitiesWrap.Size = new System.Drawing.Size(320, 124);
            this.panelActivitiesWrap.TabIndex = 0;
            // lblActivitiesTitle
            this.lblActivitiesTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblActivitiesTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblActivitiesTitle.Location = new System.Drawing.Point(0, 0);
            this.lblActivitiesTitle.Name = "lblActivitiesTitle";
            this.lblActivitiesTitle.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lblActivitiesTitle.Size = new System.Drawing.Size(320, 26);
            this.lblActivitiesTitle.TabIndex = 0;
            this.lblActivitiesTitle.Text = "آخر النشاط";
            this.lblActivitiesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // lvActivities
            this.lvActivities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colActTime,
            this.colActTitle,
            this.colActDetails});
            this.lvActivities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvActivities.FullRowSelect = true;
            this.lvActivities.GridLines = true;
            this.lvActivities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvActivities.HideSelection = false;
            this.lvActivities.Location = new System.Drawing.Point(0, 26);
            this.lvActivities.Name = "lvActivities";
            this.lvActivities.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lvActivities.RightToLeftLayout = true;
            this.lvActivities.Size = new System.Drawing.Size(320, 98);
            this.lvActivities.TabIndex = 1;
            this.lvActivities.UseCompatibleStateImageBehavior = false;
            this.lvActivities.View = System.Windows.Forms.View.Details;
            this.colActTime.Text = "الوقت";
            this.colActTime.Width = 90;
            this.colActTitle.Text = "العنوان";
            this.colActTitle.Width = 120;
            this.colActDetails.Text = "التفاصيل";
            this.colActDetails.Width = 90;
            // splitGrids
            this.splitGrids.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitGrids.Location = new System.Drawing.Point(0, 0);
            this.splitGrids.Name = "splitGrids";
            this.splitGrids.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // splitGrids.Panel1
            this.splitGrids.Panel1.Controls.Add(this.panelTopProductsWrap);
            // splitGrids.Panel2
            this.splitGrids.Panel2.Controls.Add(this.panelLowStockWrap);
            this.splitGrids.Size = new System.Drawing.Size(576, 248);
            this.splitGrids.SplitterDistance = 120;
            this.splitGrids.TabIndex = 0;
            // panelTopProductsWrap
            this.panelTopProductsWrap.Controls.Add(this.dgvTop);
            this.panelTopProductsWrap.Controls.Add(this.lblTopProductsTitle);
            this.panelTopProductsWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTopProductsWrap.Location = new System.Drawing.Point(0, 0);
            this.panelTopProductsWrap.Name = "panelTopProductsWrap";
            this.panelTopProductsWrap.Size = new System.Drawing.Size(576, 120);
            this.panelTopProductsWrap.TabIndex = 0;
            // lblTopProductsTitle
            this.lblTopProductsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTopProductsTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTopProductsTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTopProductsTitle.Name = "lblTopProductsTitle";
            this.lblTopProductsTitle.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lblTopProductsTitle.Size = new System.Drawing.Size(576, 26);
            this.lblTopProductsTitle.TabIndex = 0;
            this.lblTopProductsTitle.Text = "أكثر المنتجات مبيعاً";
            this.lblTopProductsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // dgvTop
            this.dgvTop.AllowUserToAddRows = false;
            this.dgvTop.AllowUserToDeleteRows = false;
            this.dgvTop.AutoGenerateColumns = true;
            this.dgvTop.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTop.Location = new System.Drawing.Point(0, 26);
            this.dgvTop.Name = "dgvTop";
            this.dgvTop.ReadOnly = true;
            this.dgvTop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvTop.RowHeadersVisible = false;
            this.dgvTop.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTop.Size = new System.Drawing.Size(576, 94);
            this.dgvTop.TabIndex = 1;
            // panelLowStockWrap
            this.panelLowStockWrap.Controls.Add(this.dgvLow);
            this.panelLowStockWrap.Controls.Add(this.lblLowStockTitle);
            this.panelLowStockWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLowStockWrap.Location = new System.Drawing.Point(0, 0);
            this.panelLowStockWrap.Name = "panelLowStockWrap";
            this.panelLowStockWrap.Size = new System.Drawing.Size(576, 124);
            this.panelLowStockWrap.TabIndex = 0;
            // lblLowStockTitle
            this.lblLowStockTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLowStockTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLowStockTitle.Location = new System.Drawing.Point(0, 0);
            this.lblLowStockTitle.Name = "lblLowStockTitle";
            this.lblLowStockTitle.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lblLowStockTitle.Size = new System.Drawing.Size(576, 26);
            this.lblLowStockTitle.TabIndex = 0;
            this.lblLowStockTitle.Text = "مخزون منخفض / نافد";
            this.lblLowStockTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // dgvLow
            this.dgvLow.AllowUserToAddRows = false;
            this.dgvLow.AllowUserToDeleteRows = false;
            this.dgvLow.AutoGenerateColumns = true;
            this.dgvLow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLow.Location = new System.Drawing.Point(0, 26);
            this.dgvLow.Name = "dgvLow";
            this.dgvLow.ReadOnly = true;
            this.dgvLow.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvLow.RowHeadersVisible = false;
            this.dgvLow.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLow.Size = new System.Drawing.Size(576, 98);
            this.dgvLow.TabIndex = 1;
            // DashboardForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 620);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.chartSales);
            this.Controls.Add(this.tableLayoutKpi);
            this.Controls.Add(this.panelTop);
            this.Name = "DashboardForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "لوحة التحكم";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.DashboardForm_Load);
            this.panelTop.ResumeLayout(false);
            this.tableLayoutKpi.ResumeLayout(false);
            this.cardToday.ResumeLayout(false);
            this.cardOrders.ResumeLayout(false);
            this.cardWeek.ResumeLayout(false);
            this.cardMonth.ResumeLayout(false);
            this.cardDebts.ResumeLayout(false);
            this.cardLowStock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartSales)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitSide.Panel1.ResumeLayout(false);
            this.splitSide.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitSide)).EndInit();
            this.splitSide.ResumeLayout(false);
            this.panelAlertsWrap.ResumeLayout(false);
            this.panelActivitiesWrap.ResumeLayout(false);
            this.splitGrids.Panel1.ResumeLayout(false);
            this.splitGrids.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitGrids)).EndInit();
            this.splitGrids.ResumeLayout(false);
            this.panelTopProductsWrap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).EndInit();
            this.panelLowStockWrap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLow)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupKpiCard(System.Windows.Forms.Panel card, System.Windows.Forms.Panel accent,
            System.Windows.Forms.Label title, System.Windows.Forms.Label value, string titleText, System.Drawing.Color accentColor)
        {
            card.BackColor = System.Drawing.Color.White;
            card.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            card.Controls.Add(value);
            card.Controls.Add(title);
            card.Controls.Add(accent);
            card.Dock = System.Windows.Forms.DockStyle.Fill;
            card.Margin = new System.Windows.Forms.Padding(4);
            card.Name = card.Name;
            accent.BackColor = accentColor;
            accent.Dock = System.Windows.Forms.DockStyle.Left;
            accent.Width = 5;
            title.Dock = System.Windows.Forms.DockStyle.Top;
            title.Font = new System.Drawing.Font("Segoe UI", 9F);
            title.ForeColor = System.Drawing.Color.DimGray;
            title.Height = 22;
            title.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            title.Text = titleText;
            title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            value.Dock = System.Windows.Forms.DockStyle.Fill;
            value.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            value.ForeColor = accentColor;
            value.Text = "—";
            value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }
    }
}
