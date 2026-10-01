namespace Sales.Forms
{
    partial class ErrorLogForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Controls.Add(this.btnExportPdf);
            this.panelTop.Controls.Add(this.btnPrint);
            this.panelTop.Controls.Add(this.btnExport);
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 44;
            this.btnRefresh.Text = "تحديث"; this.btnRefresh.Location = new System.Drawing.Point(200, 8);
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            this.btnExport.Text = "تصدير CSV"; this.btnExport.Location = new System.Drawing.Point(90, 8);
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            this.btnExportPdf.Text = "تصدير PDF"; this.btnExportPdf.Location = new System.Drawing.Point(200, 8);
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            this.btnPrint.Text = "طباعة"; this.btnPrint.Location = new System.Drawing.Point(310, 8);
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            this.btnClose.Text = "إغلاق"; this.btnClose.Location = new System.Drawing.Point(10, 8);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.panelTop);
            this.Text = "سجل الأخطاء";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += new System.EventHandler(this.ErrorLogForm_Load);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgv;
    }
}
