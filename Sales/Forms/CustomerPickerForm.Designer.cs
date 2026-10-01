namespace Sales.Forms
{
    partial class CustomerPickerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.lblSearch.Text = "بحث:"; this.lblSearch.AutoSize = true; this.lblSearch.Location = new System.Drawing.Point(420, 15);
            this.txtSearch.Location = new System.Drawing.Point(120, 12); this.txtSearch.Width = 290;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.dgv.Location = new System.Drawing.Point(12, 45); this.dgv.Size = new System.Drawing.Size(460, 280);
            this.dgv.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.btnOk.Text = "اختيار"; this.btnOk.Location = new System.Drawing.Point(290, 335);
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.btnClear.Text = "بدون عميل"; this.btnClear.Location = new System.Drawing.Point(170, 335);
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            this.btnCancel.Text = "إلغاء"; this.btnCancel.Location = new System.Drawing.Point(50, 335);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.ClientSize = new System.Drawing.Size(490, 380);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.Text = "اختيار عميل";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += new System.EventHandler(this.CustomerPickerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnCancel;
    }
}
