namespace Sales.Forms
{
    partial class AdvancedSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpTax = new System.Windows.Forms.GroupBox();
            this.txtTaxPercent = new System.Windows.Forms.TextBox();
            this.lblTaxPercent = new System.Windows.Forms.Label();
            this.chkTaxEnabled = new System.Windows.Forms.CheckBox();
            this.grpSecurity = new System.Windows.Forms.GroupBox();
            this.numMaxFailed = new System.Windows.Forms.NumericUpDown();
            this.lblMaxFailed = new System.Windows.Forms.Label();
            this.numAutoLogout = new System.Windows.Forms.NumericUpDown();
            this.lblAutoLogout = new System.Windows.Forms.Label();
            this.grpBackup = new System.Windows.Forms.GroupBox();
            this.btnBackupNow = new System.Windows.Forms.Button();
            this.btnBrowseBackup = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.lblBackupPath = new System.Windows.Forms.Label();
            this.chkDailyBackup = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpTax.SuspendLayout();
            this.grpSecurity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFailed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoLogout)).BeginInit();
            this.grpBackup.SuspendLayout();
            this.SuspendLayout();
            // grpTax
            this.grpTax.Controls.Add(this.txtTaxPercent);
            this.grpTax.Controls.Add(this.lblTaxPercent);
            this.grpTax.Controls.Add(this.chkTaxEnabled);
            this.grpTax.Location = new System.Drawing.Point(20, 20);
            this.grpTax.Size = new System.Drawing.Size(520, 90);
            this.grpTax.Text = "الضريبة";
            this.grpTax.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkTaxEnabled.Text = "تفعيل الضريبة على الفواتير";
            this.chkTaxEnabled.Location = new System.Drawing.Point(280, 28);
            this.lblTaxPercent.Text = "النسبة %:"; this.lblTaxPercent.AutoSize = true; this.lblTaxPercent.Location = new System.Drawing.Point(200, 58);
            this.txtTaxPercent.Location = new System.Drawing.Point(80, 54); this.txtTaxPercent.Width = 100;
            // grpSecurity
            this.grpSecurity.Controls.Add(this.numMaxFailed);
            this.grpSecurity.Controls.Add(this.lblMaxFailed);
            this.grpSecurity.Controls.Add(this.numAutoLogout);
            this.grpSecurity.Controls.Add(this.lblAutoLogout);
            this.grpSecurity.Location = new System.Drawing.Point(20, 120);
            this.grpSecurity.Size = new System.Drawing.Size(520, 100);
            this.grpSecurity.Text = "الأمان والجلسة";
            this.grpSecurity.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblAutoLogout.Text = "خروج تلقائي (دقيقة، 0=معطل):"; this.lblAutoLogout.AutoSize = true; this.lblAutoLogout.Location = new System.Drawing.Point(300, 30);
            this.numAutoLogout.Maximum = 480; this.numAutoLogout.Location = new System.Drawing.Point(200, 26); this.numAutoLogout.Width = 80;
            this.lblMaxFailed.Text = "محاولات دخول فاشلة قبل القفل:"; this.lblMaxFailed.AutoSize = true; this.lblMaxFailed.Location = new System.Drawing.Point(280, 62);
            this.numMaxFailed.Minimum = 1; this.numMaxFailed.Maximum = 50; this.numMaxFailed.Value = 5;
            this.numMaxFailed.Location = new System.Drawing.Point(200, 58); this.numMaxFailed.Width = 80;
            // grpBackup
            this.grpBackup.Controls.Add(this.btnBackupNow);
            this.grpBackup.Controls.Add(this.btnBrowseBackup);
            this.grpBackup.Controls.Add(this.txtBackupPath);
            this.grpBackup.Controls.Add(this.lblBackupPath);
            this.grpBackup.Controls.Add(this.chkDailyBackup);
            this.grpBackup.Location = new System.Drawing.Point(20, 230);
            this.grpBackup.Size = new System.Drawing.Size(520, 120);
            this.grpBackup.Text = "النسخ الاحتياطي اليومي";
            this.grpBackup.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkDailyBackup.Text = "تفعيل النسخ عند الإغلاق / يومياً";
            this.chkDailyBackup.Location = new System.Drawing.Point(260, 28);
            this.lblBackupPath.Text = "مسار الحفظ:"; this.lblBackupPath.AutoSize = true; this.lblBackupPath.Location = new System.Drawing.Point(420, 58);
            this.txtBackupPath.Location = new System.Drawing.Point(120, 54); this.txtBackupPath.Width = 290;
            this.btnBrowseBackup.Text = "..."; this.btnBrowseBackup.Location = new System.Drawing.Point(80, 52); this.btnBrowseBackup.Width = 35;
            this.btnBrowseBackup.Click += new System.EventHandler(this.btnBrowseBackup_Click);
            this.btnBackupNow.Text = "نسخ الآن"; this.btnBackupNow.Location = new System.Drawing.Point(20, 85);
            this.btnBackupNow.Click += new System.EventHandler(this.btnBackupNow_Click);
            // buttons
            this.btnSave.Text = "حفظ"; this.btnSave.Location = new System.Drawing.Point(420, 370); this.btnSave.Width = 110;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnClose.Text = "إغلاق"; this.btnClose.Location = new System.Drawing.Point(300, 370); this.btnClose.Width = 110;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // Form
            this.ClientSize = new System.Drawing.Size(560, 420);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpBackup);
            this.Controls.Add(this.grpSecurity);
            this.Controls.Add(this.grpTax);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "الإعدادات المتقدمة";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Load += new System.EventHandler(this.AdvancedSettingsForm_Load);
            this.grpTax.ResumeLayout(false);
            this.grpTax.PerformLayout();
            this.grpSecurity.ResumeLayout(false);
            this.grpSecurity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFailed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoLogout)).EndInit();
            this.grpBackup.ResumeLayout(false);
            this.grpBackup.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpTax;
        private System.Windows.Forms.CheckBox chkTaxEnabled;
        private System.Windows.Forms.Label lblTaxPercent;
        private System.Windows.Forms.TextBox txtTaxPercent;
        private System.Windows.Forms.GroupBox grpSecurity;
        private System.Windows.Forms.Label lblAutoLogout;
        private System.Windows.Forms.NumericUpDown numAutoLogout;
        private System.Windows.Forms.Label lblMaxFailed;
        private System.Windows.Forms.NumericUpDown numMaxFailed;
        private System.Windows.Forms.GroupBox grpBackup;
        private System.Windows.Forms.CheckBox chkDailyBackup;
        private System.Windows.Forms.Label lblBackupPath;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.Button btnBrowseBackup;
        private System.Windows.Forms.Button btnBackupNow;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
    }
}
