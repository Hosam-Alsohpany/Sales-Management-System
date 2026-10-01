namespace Sales.Forms
{
    partial class ReportsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TabControl tabReports;
        private System.Windows.Forms.TabPage tabSales;
        private System.Windows.Forms.Panel barSales;
        private System.Windows.Forms.Button btnSalesCsv;
        private System.Windows.Forms.Button btnSalesHtml;
        private System.Windows.Forms.Button btnSalesPdf;
        private System.Windows.Forms.Button btnSalesPrint;
        private System.Windows.Forms.DataGridView dgvSales;
        private System.Windows.Forms.TabPage tabStock;
        private System.Windows.Forms.Panel barStock;
        private System.Windows.Forms.Button btnStockCsv;
        private System.Windows.Forms.Button btnStockHtml;
        private System.Windows.Forms.Button btnStockPdf;
        private System.Windows.Forms.Button btnStockPrint;
        private System.Windows.Forms.DataGridView dgvStock;
        private System.Windows.Forms.TabPage tabProfit;
        private System.Windows.Forms.Panel barProfit;
        private System.Windows.Forms.Button btnProfitCsv;
        private System.Windows.Forms.Button btnProfitHtml;
        private System.Windows.Forms.Button btnProfitPdf;
        private System.Windows.Forms.Button btnProfitPrint;
        private System.Windows.Forms.DataGridView dgvProfit;
        private System.Windows.Forms.TabPage tabDebts;
        private System.Windows.Forms.Panel barDebts;
        private System.Windows.Forms.Button btnDebtsCsv;
        private System.Windows.Forms.Button btnDebtsHtml;
        private System.Windows.Forms.Button btnDebtsPdf;
        private System.Windows.Forms.Button btnDebtsPrint;
        private System.Windows.Forms.DataGridView dgvDebts;
        private System.Windows.Forms.TabPage tabTop;
        private System.Windows.Forms.Panel barTop;
        private System.Windows.Forms.Button btnTopCsv;
        private System.Windows.Forms.Button btnTopHtml;
        private System.Windows.Forms.Button btnTopPdf;
        private System.Windows.Forms.Button btnTopPrint;
        private System.Windows.Forms.DataGridView dgvTop;
        private System.Windows.Forms.TabPage tabLeast;
        private System.Windows.Forms.Panel barLeast;
        private System.Windows.Forms.Button btnLeastCsv;
        private System.Windows.Forms.Button btnLeastHtml;
        private System.Windows.Forms.Button btnLeastPdf;
        private System.Windows.Forms.Button btnLeastPrint;
        private System.Windows.Forms.DataGridView dgvLeast;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelFilter = new System.Windows.Forms.Panel();
            this.lblFrom = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.btnRun = new System.Windows.Forms.Button();
            this.tabReports = new System.Windows.Forms.TabControl();
            this.tabSales = new System.Windows.Forms.TabPage();
            this.barSales = new System.Windows.Forms.Panel();
            this.btnSalesCsv = new System.Windows.Forms.Button();
            this.btnSalesHtml = new System.Windows.Forms.Button();
            this.btnSalesPdf = new System.Windows.Forms.Button();
            this.btnSalesPrint = new System.Windows.Forms.Button();
            this.dgvSales = new System.Windows.Forms.DataGridView();
            this.tabStock = new System.Windows.Forms.TabPage();
            this.barStock = new System.Windows.Forms.Panel();
            this.btnStockCsv = new System.Windows.Forms.Button();
            this.btnStockHtml = new System.Windows.Forms.Button();
            this.btnStockPdf = new System.Windows.Forms.Button();
            this.btnStockPrint = new System.Windows.Forms.Button();
            this.dgvStock = new System.Windows.Forms.DataGridView();
            this.tabProfit = new System.Windows.Forms.TabPage();
            this.barProfit = new System.Windows.Forms.Panel();
            this.btnProfitCsv = new System.Windows.Forms.Button();
            this.btnProfitHtml = new System.Windows.Forms.Button();
            this.btnProfitPdf = new System.Windows.Forms.Button();
            this.btnProfitPrint = new System.Windows.Forms.Button();
            this.dgvProfit = new System.Windows.Forms.DataGridView();
            this.tabDebts = new System.Windows.Forms.TabPage();
            this.barDebts = new System.Windows.Forms.Panel();
            this.btnDebtsCsv = new System.Windows.Forms.Button();
            this.btnDebtsHtml = new System.Windows.Forms.Button();
            this.btnDebtsPdf = new System.Windows.Forms.Button();
            this.btnDebtsPrint = new System.Windows.Forms.Button();
            this.dgvDebts = new System.Windows.Forms.DataGridView();
            this.tabTop = new System.Windows.Forms.TabPage();
            this.barTop = new System.Windows.Forms.Panel();
            this.btnTopCsv = new System.Windows.Forms.Button();
            this.btnTopHtml = new System.Windows.Forms.Button();
            this.btnTopPdf = new System.Windows.Forms.Button();
            this.btnTopPrint = new System.Windows.Forms.Button();
            this.dgvTop = new System.Windows.Forms.DataGridView();
            this.tabLeast = new System.Windows.Forms.TabPage();
            this.barLeast = new System.Windows.Forms.Panel();
            this.btnLeastCsv = new System.Windows.Forms.Button();
            this.btnLeastHtml = new System.Windows.Forms.Button();
            this.btnLeastPdf = new System.Windows.Forms.Button();
            this.btnLeastPrint = new System.Windows.Forms.Button();
            this.dgvLeast = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelFilter.SuspendLayout();
            this.tabReports.SuspendLayout();
            this.tabSales.SuspendLayout();
            this.barSales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).BeginInit();
            this.tabStock.SuspendLayout();
            this.barStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).BeginInit();
            this.tabProfit.SuspendLayout();
            this.barProfit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfit)).BeginInit();
            this.tabDebts.SuspendLayout();
            this.barDebts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDebts)).BeginInit();
            this.tabTop.SuspendLayout();
            this.barTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).BeginInit();
            this.tabLeast.SuspendLayout();
            this.barLeast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeast)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // panelFilter
            this.panelFilter.Controls.Add(this.btnRun);
            this.panelFilter.Controls.Add(this.dtTo);
            this.panelFilter.Controls.Add(this.lblTo);
            this.panelFilter.Controls.Add(this.dtFrom);
            this.panelFilter.Controls.Add(this.lblFrom);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(960, 44);
            this.panelFilter.TabIndex = 0;
            // lblFrom
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(720, 12);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(29, 16);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "من:";
            // dtFrom
            this.dtFrom.Location = new System.Drawing.Point(580, 8);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(120, 22);
            this.dtFrom.TabIndex = 1;
            // lblTo
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(520, 12);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(28, 16);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "إلى:";
            // dtTo
            this.dtTo.Location = new System.Drawing.Point(380, 8);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(120, 22);
            this.dtTo.TabIndex = 3;
            // btnRun
            this.btnRun.Location = new System.Drawing.Point(280, 6);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(80, 30);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "تشغيل";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // tabReports
            this.tabReports.Controls.Add(this.tabSales);
            this.tabReports.Controls.Add(this.tabStock);
            this.tabReports.Controls.Add(this.tabProfit);
            this.tabReports.Controls.Add(this.tabDebts);
            this.tabReports.Controls.Add(this.tabTop);
            this.tabReports.Controls.Add(this.tabLeast);
            this.tabReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabReports.Location = new System.Drawing.Point(0, 44);
            this.tabReports.Name = "tabReports";
            this.tabReports.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabReports.RightToLeftLayout = true;
            this.tabReports.SelectedIndex = 0;
            this.tabReports.Size = new System.Drawing.Size(960, 496);
            this.tabReports.TabIndex = 1;
            this.SetupReportTab(this.tabSales, "المبيعات", this.barSales, this.dgvSales, this.btnSalesCsv, this.btnSalesHtml, this.btnSalesPdf, this.btnSalesPrint);
            this.SetupReportTab(this.tabStock, "المخزون", this.barStock, this.dgvStock, this.btnStockCsv, this.btnStockHtml, this.btnStockPdf, this.btnStockPrint);
            this.SetupReportTab(this.tabProfit, "الأرباح", this.barProfit, this.dgvProfit, this.btnProfitCsv, this.btnProfitHtml, this.btnProfitPdf, this.btnProfitPrint);
            this.SetupReportTab(this.tabDebts, "الديون", this.barDebts, this.dgvDebts, this.btnDebtsCsv, this.btnDebtsHtml, this.btnDebtsPdf, this.btnDebtsPrint);
            this.SetupReportTab(this.tabTop, "الأكثر مبيعاً", this.barTop, this.dgvTop, this.btnTopCsv, this.btnTopHtml, this.btnTopPdf, this.btnTopPrint);
            this.SetupReportTab(this.tabLeast, "الأقل مبيعاً", this.barLeast, this.dgvLeast, this.btnLeastCsv, this.btnLeastHtml, this.btnLeastPdf, this.btnLeastPrint);
            // panelBottom
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 540);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(960, 40);
            this.panelBottom.TabIndex = 2;
            // btnClose
            this.btnClose.Location = new System.Drawing.Point(20, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "إغلاق";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // ReportsForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 580);
            this.Controls.Add(this.tabReports);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.panelBottom);
            this.Name = "ReportsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "التقارير";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ReportsForm_Load);
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            this.tabReports.ResumeLayout(false);
            this.tabSales.ResumeLayout(false);
            this.barSales.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).EndInit();
            this.tabStock.ResumeLayout(false);
            this.barStock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStock)).EndInit();
            this.tabProfit.ResumeLayout(false);
            this.barProfit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfit)).EndInit();
            this.tabDebts.ResumeLayout(false);
            this.barDebts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDebts)).EndInit();
            this.tabTop.ResumeLayout(false);
            this.barTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop)).EndInit();
            this.tabLeast.ResumeLayout(false);
            this.barLeast.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeast)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void SetupReportTab(System.Windows.Forms.TabPage tab, string title, System.Windows.Forms.Panel bar,
            System.Windows.Forms.DataGridView dgv, System.Windows.Forms.Button btnCsv, System.Windows.Forms.Button btnHtml,
            System.Windows.Forms.Button btnPdf, System.Windows.Forms.Button btnPrint)
        {
            tab.Controls.Add(dgv);
            tab.Controls.Add(bar);
            tab.Location = new System.Drawing.Point(4, 25);
            tab.Padding = new System.Windows.Forms.Padding(3);
            tab.Size = new System.Drawing.Size(952, 467);
            tab.TabIndex = 0;
            tab.Text = title;
            tab.UseVisualStyleBackColor = true;
            bar.Controls.Add(btnPrint);
            bar.Controls.Add(btnPdf);
            bar.Controls.Add(btnHtml);
            bar.Controls.Add(btnCsv);
            bar.Dock = System.Windows.Forms.DockStyle.Top;
            bar.Height = 36;
            bar.Name = "bar" + title;
            SetupExportButton(btnCsv, "CSV", 10);
            SetupExportButton(btnHtml, "HTML", 90);
            SetupExportButton(btnPdf, "PDF", 170);
            SetupExportButton(btnPrint, "طباعة", 250);
            btnCsv.Tag = new ReportTabExportTag(title, dgv, ReportExportKind.Csv);
            btnHtml.Tag = new ReportTabExportTag(title, dgv, ReportExportKind.Html);
            btnPdf.Tag = new ReportTabExportTag(title, dgv, ReportExportKind.Pdf);
            btnPrint.Tag = new ReportTabExportTag(title, dgv, ReportExportKind.Print);
            btnCsv.Click += new System.EventHandler(this.ReportTabExport_Click);
            btnHtml.Click += new System.EventHandler(this.ReportTabExport_Click);
            btnPdf.Click += new System.EventHandler(this.ReportTabExport_Click);
            btnPrint.Click += new System.EventHandler(this.ReportTabExport_Click);
            dgv.AllowUserToAddRows = false;
            dgv.AutoGenerateColumns = true;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            dgv.RowHeadersVisible = false;
        }

        private static void SetupExportButton(System.Windows.Forms.Button btn, string text, int x)
        {
            btn.Location = new System.Drawing.Point(x, 4);
            btn.Size = new System.Drawing.Size(70, 28);
            btn.Text = text;
            btn.UseVisualStyleBackColor = true;
        }
    }
}
