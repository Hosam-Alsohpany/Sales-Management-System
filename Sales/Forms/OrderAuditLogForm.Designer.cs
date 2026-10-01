namespace Sales.Forms
{
    partial class OrderAuditLogForm
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtOrderId = new System.Windows.Forms.TextBox();
            this.lblOrderId = new System.Windows.Forms.Label();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.lblAction = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // panelTop
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Controls.Add(this.btnExportPdf);
            this.panelTop.Controls.Add(this.btnPrint);
            this.panelTop.Controls.Add(this.btnExport);
            this.panelTop.Controls.Add(this.btnSearch);
            this.panelTop.Controls.Add(this.txtOrderId);
            this.panelTop.Controls.Add(this.lblOrderId);
            this.panelTop.Controls.Add(this.cmbAction);
            this.panelTop.Controls.Add(this.lblAction);
            this.panelTop.Controls.Add(this.txtUser);
            this.panelTop.Controls.Add(this.lblUser);
            this.panelTop.Controls.Add(this.dtpTo);
            this.panelTop.Controls.Add(this.lblTo);
            this.panelTop.Controls.Add(this.dtpFrom);
            this.panelTop.Controls.Add(this.lblFrom);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 72;
            this.panelTop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // lblFrom
            this.lblFrom.Text = "من:"; this.lblFrom.AutoSize = true; this.lblFrom.Location = new System.Drawing.Point(900, 12);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(780, 8); this.dtpFrom.Width = 110;
            this.lblTo.Text = "إلى:"; this.lblTo.AutoSize = true; this.lblTo.Location = new System.Drawing.Point(720, 12);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(600, 8); this.dtpTo.Width = 110;
            this.lblUser.Text = "المستخدم:"; this.lblUser.AutoSize = true; this.lblUser.Location = new System.Drawing.Point(520, 12);
            this.txtUser.Location = new System.Drawing.Point(380, 8); this.txtUser.Width = 130;
            this.lblAction.Text = "العملية:"; this.lblAction.AutoSize = true; this.lblAction.Location = new System.Drawing.Point(320, 12);
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.Location = new System.Drawing.Point(200, 8); this.cmbAction.Width = 110;
            this.lblOrderId.Text = "فاتورة:"; this.lblOrderId.AutoSize = true; this.lblOrderId.Location = new System.Drawing.Point(150, 12);
            this.txtOrderId.Location = new System.Drawing.Point(70, 8); this.txtOrderId.Width = 70;
            this.btnSearch.Text = "بحث"; this.btnSearch.Location = new System.Drawing.Point(200, 40); this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnExport.Text = "تصدير CSV"; this.btnExport.Location = new System.Drawing.Point(90, 40); this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            this.btnExportPdf.Text = "تصدير PDF"; this.btnExportPdf.Location = new System.Drawing.Point(300, 40); this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            this.btnPrint.Text = "طباعة"; this.btnPrint.Location = new System.Drawing.Point(410, 40); this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            this.btnClose.Text = "إغلاق"; this.btnClose.Location = new System.Drawing.Point(10, 40); this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // splitMain
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitMain.SplitterDistance = 320;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.SelectionChanged += new System.EventHandler(this.dgv_SelectionChanged);
            this.splitMain.Panel1.Controls.Add(this.dgv);
            this.txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetails.Multiline = true;
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.splitMain.Panel2.Controls.Add(this.txtDetails);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.ClientSize = new System.Drawing.Size(980, 480);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelTop);
            this.Name = "OrderAuditLogForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "سجل تدقيق الفواتير";
            this.Load += new System.EventHandler(this.OrderAuditLogForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.ComboBox cmbAction;
        private System.Windows.Forms.Label lblOrderId;
        private System.Windows.Forms.TextBox txtOrderId;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.TextBox txtDetails;
    }
}
