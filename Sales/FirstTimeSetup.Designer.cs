namespace Sales
{
    partial class FirstTimeSetup
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TabControl tabWizard;
        private System.Windows.Forms.TabPage tabStore;
        private System.Windows.Forms.TabPage tabAdmin;
        private System.Windows.Forms.TabPage tabDefaults;
        private System.Windows.Forms.TabPage tabSummary;
        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnFinish;

        private System.Windows.Forms.Label lblStoreName;
        private System.Windows.Forms.TextBox txtStoreName;
        private System.Windows.Forms.Label lblCurrency;
        private System.Windows.Forms.ComboBox cmbCurrency;
        private System.Windows.Forms.Label lblTax;
        private System.Windows.Forms.TextBox txtDefaultTax;

        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblPwd;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtConfirm;
        private System.Windows.Forms.CheckBox chkShowPassword;

        private System.Windows.Forms.Label lblDefaultUnit;
        private System.Windows.Forms.TextBox txtDefaultUnit;
        private System.Windows.Forms.Label lblDefaultWarehouse;
        private System.Windows.Forms.TextBox txtDefaultWarehouse;
        private System.Windows.Forms.CheckBox chkSeedDemoProduct;
        private System.Windows.Forms.Label lblSummary;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabWizard = new System.Windows.Forms.TabControl();
            this.tabStore = new System.Windows.Forms.TabPage();
            this.lblTax = new System.Windows.Forms.Label();
            this.txtDefaultTax = new System.Windows.Forms.TextBox();
            this.lblCurrency = new System.Windows.Forms.Label();
            this.cmbCurrency = new System.Windows.Forms.ComboBox();
            this.lblStoreName = new System.Windows.Forms.Label();
            this.txtStoreName = new System.Windows.Forms.TextBox();
            this.tabAdmin = new System.Windows.Forms.TabPage();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblPwd = new System.Windows.Forms.Label();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtConfirm = new System.Windows.Forms.TextBox();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.tabDefaults = new System.Windows.Forms.TabPage();
            this.chkSeedDemoProduct = new System.Windows.Forms.CheckBox();
            this.lblDefaultWarehouse = new System.Windows.Forms.Label();
            this.txtDefaultWarehouse = new System.Windows.Forms.TextBox();
            this.lblDefaultUnit = new System.Windows.Forms.Label();
            this.txtDefaultUnit = new System.Windows.Forms.TextBox();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.lblSummary = new System.Windows.Forms.Label();
            this.lblStep = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.tabWizard.SuspendLayout();
            this.tabStore.SuspendLayout();
            this.tabAdmin.SuspendLayout();
            this.tabDefaults.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabWizard
            // 
            this.tabWizard.Controls.Add(this.tabStore);
            this.tabWizard.Controls.Add(this.tabAdmin);
            this.tabWizard.Controls.Add(this.tabDefaults);
            this.tabWizard.Controls.Add(this.tabSummary);
            this.tabWizard.Location = new System.Drawing.Point(18, 45);
            this.tabWizard.Name = "tabWizard";
            this.tabWizard.SelectedIndex = 0;
            this.tabWizard.Size = new System.Drawing.Size(716, 240);
            this.tabWizard.TabIndex = 1;
            this.tabWizard.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabWizard_Selecting);
            // 
            // tabStore
            // 
            this.tabStore.Controls.Add(this.lblTax);
            this.tabStore.Controls.Add(this.txtDefaultTax);
            this.tabStore.Controls.Add(this.lblCurrency);
            this.tabStore.Controls.Add(this.cmbCurrency);
            this.tabStore.Controls.Add(this.lblStoreName);
            this.tabStore.Controls.Add(this.txtStoreName);
            this.tabStore.Location = new System.Drawing.Point(4, 25);
            this.tabStore.Name = "tabStore";
            this.tabStore.Padding = new System.Windows.Forms.Padding(10);
            this.tabStore.Size = new System.Drawing.Size(708, 211);
            this.tabStore.TabIndex = 0;
            this.tabStore.Text = "1) المتجر";
            this.tabStore.UseVisualStyleBackColor = true;
            // 
            // lblTax
            // 
            this.lblTax.AutoSize = true;
            this.lblTax.Location = new System.Drawing.Point(14, 112);
            this.lblTax.Name = "lblTax";
            this.lblTax.Size = new System.Drawing.Size(137, 17);
            this.lblTax.TabIndex = 4;
            this.lblTax.Text = "الضريبة الافتراضية (%)";
            // 
            // txtDefaultTax
            // 
            this.txtDefaultTax.Location = new System.Drawing.Point(14, 132);
            this.txtDefaultTax.Name = "txtDefaultTax";
            this.txtDefaultTax.Size = new System.Drawing.Size(316, 24);
            this.txtDefaultTax.TabIndex = 5;
            // 
            // lblCurrency
            // 
            this.lblCurrency.AutoSize = true;
            this.lblCurrency.Location = new System.Drawing.Point(14, 60);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(44, 17);
            this.lblCurrency.TabIndex = 2;
            this.lblCurrency.Text = "العملة";
            // 
            // cmbCurrency
            // 
            this.cmbCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurrency.FormattingEnabled = true;
            this.cmbCurrency.Location = new System.Drawing.Point(14, 80);
            this.cmbCurrency.Name = "cmbCurrency";
            this.cmbCurrency.Size = new System.Drawing.Size(316, 24);
            this.cmbCurrency.TabIndex = 3;
            // 
            // lblStoreName
            // 
            this.lblStoreName.AutoSize = true;
            this.lblStoreName.Location = new System.Drawing.Point(14, 10);
            this.lblStoreName.Name = "lblStoreName";
            this.lblStoreName.Size = new System.Drawing.Size(76, 17);
            this.lblStoreName.TabIndex = 0;
            this.lblStoreName.Text = "اسم المتجر";
            // 
            // txtStoreName
            // 
            this.txtStoreName.Location = new System.Drawing.Point(14, 30);
            this.txtStoreName.Name = "txtStoreName";
            this.txtStoreName.Size = new System.Drawing.Size(676, 24);
            this.txtStoreName.TabIndex = 1;
            // 
            // tabAdmin
            // 
            this.tabAdmin.Controls.Add(this.lblUser);
            this.tabAdmin.Controls.Add(this.lblFullName);
            this.tabAdmin.Controls.Add(this.lblPwd);
            this.tabAdmin.Controls.Add(this.lblConfirm);
            this.tabAdmin.Controls.Add(this.txtUsername);
            this.tabAdmin.Controls.Add(this.txtFullName);
            this.tabAdmin.Controls.Add(this.txtPassword);
            this.tabAdmin.Controls.Add(this.txtConfirm);
            this.tabAdmin.Controls.Add(this.chkShowPassword);
            this.tabAdmin.Location = new System.Drawing.Point(4, 25);
            this.tabAdmin.Name = "tabAdmin";
            this.tabAdmin.Padding = new System.Windows.Forms.Padding(10);
            this.tabAdmin.Size = new System.Drawing.Size(708, 211);
            this.tabAdmin.TabIndex = 1;
            this.tabAdmin.Text = "2) المدير";
            this.tabAdmin.UseVisualStyleBackColor = true;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(14, 10);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(101, 17);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "اسم الحساب (Username)";
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(14, 60);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(139, 17);
            this.lblFullName.TabIndex = 2;
            this.lblFullName.Text = "اسم المستخدم الظاهر";
            // 
            // lblPwd
            // 
            this.lblPwd.AutoSize = true;
            this.lblPwd.Location = new System.Drawing.Point(14, 110);
            this.lblPwd.Name = "lblPwd";
            this.lblPwd.Size = new System.Drawing.Size(193, 17);
            this.lblPwd.TabIndex = 4;
            this.lblPwd.Text = "كلمة المرور (6 أحرف على الأقل)";
            // 
            // lblConfirm
            // 
            this.lblConfirm.AutoSize = true;
            this.lblConfirm.Location = new System.Drawing.Point(14, 165);
            this.lblConfirm.Name = "lblConfirm";
            this.lblConfirm.Size = new System.Drawing.Size(106, 17);
            this.lblConfirm.TabIndex = 6;
            this.lblConfirm.Text = "تأكيد كلمة المرور";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(14, 30);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(316, 24);
            this.txtUsername.TabIndex = 1;
            // 
            // txtFullName
            // 
            this.txtFullName.Location = new System.Drawing.Point(14, 80);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(316, 24);
            this.txtFullName.TabIndex = 3;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(14, 130);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(316, 24);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtConfirm
            // 
            this.txtConfirm.Location = new System.Drawing.Point(14, 185);
            this.txtConfirm.Name = "txtConfirm";
            this.txtConfirm.Size = new System.Drawing.Size(316, 24);
            this.txtConfirm.TabIndex = 7;
            this.txtConfirm.UseSystemPasswordChar = true;
            // 
            // chkShowPassword
            // 
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Location = new System.Drawing.Point(350, 132);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(119, 21);
            this.chkShowPassword.TabIndex = 8;
            this.chkShowPassword.Text = "إظهار كلمة المرور";
            this.chkShowPassword.UseVisualStyleBackColor = true;
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);
            // 
            // tabDefaults
            // 
            this.tabDefaults.Controls.Add(this.chkSeedDemoProduct);
            this.tabDefaults.Controls.Add(this.lblDefaultWarehouse);
            this.tabDefaults.Controls.Add(this.txtDefaultWarehouse);
            this.tabDefaults.Controls.Add(this.lblDefaultUnit);
            this.tabDefaults.Controls.Add(this.txtDefaultUnit);
            this.tabDefaults.Location = new System.Drawing.Point(4, 25);
            this.tabDefaults.Name = "tabDefaults";
            this.tabDefaults.Padding = new System.Windows.Forms.Padding(10);
            this.tabDefaults.Size = new System.Drawing.Size(708, 211);
            this.tabDefaults.TabIndex = 2;
            this.tabDefaults.Text = "3) افتراضيات";
            this.tabDefaults.UseVisualStyleBackColor = true;
            // 
            // chkSeedDemoProduct
            // 
            this.chkSeedDemoProduct.AutoSize = true;
            this.chkSeedDemoProduct.Location = new System.Drawing.Point(14, 140);
            this.chkSeedDemoProduct.Name = "chkSeedDemoProduct";
            this.chkSeedDemoProduct.Size = new System.Drawing.Size(140, 21);
            this.chkSeedDemoProduct.TabIndex = 4;
            this.chkSeedDemoProduct.Text = "إضافة منتج تجريبي";
            this.chkSeedDemoProduct.UseVisualStyleBackColor = true;
            // 
            // lblDefaultWarehouse
            // 
            this.lblDefaultWarehouse.AutoSize = true;
            this.lblDefaultWarehouse.Location = new System.Drawing.Point(14, 65);
            this.lblDefaultWarehouse.Name = "lblDefaultWarehouse";
            this.lblDefaultWarehouse.Size = new System.Drawing.Size(201, 17);
            this.lblDefaultWarehouse.TabIndex = 2;
            this.lblDefaultWarehouse.Text = "المخزن الافتراضي (سيتم إنشاؤه)";
            // 
            // txtDefaultWarehouse
            // 
            this.txtDefaultWarehouse.Location = new System.Drawing.Point(14, 85);
            this.txtDefaultWarehouse.Name = "txtDefaultWarehouse";
            this.txtDefaultWarehouse.Size = new System.Drawing.Size(316, 24);
            this.txtDefaultWarehouse.TabIndex = 3;
            this.txtDefaultWarehouse.Text = "الرئيسي";
            // 
            // lblDefaultUnit
            // 
            this.lblDefaultUnit.AutoSize = true;
            this.lblDefaultUnit.Location = new System.Drawing.Point(14, 10);
            this.lblDefaultUnit.Name = "lblDefaultUnit";
            this.lblDefaultUnit.Size = new System.Drawing.Size(105, 17);
            this.lblDefaultUnit.TabIndex = 0;
            this.lblDefaultUnit.Text = "الوحدة الافتراضية";
            // 
            // txtDefaultUnit
            // 
            this.txtDefaultUnit.Location = new System.Drawing.Point(14, 30);
            this.txtDefaultUnit.Name = "txtDefaultUnit";
            this.txtDefaultUnit.Size = new System.Drawing.Size(316, 24);
            this.txtDefaultUnit.TabIndex = 1;
            this.txtDefaultUnit.Text = "قطعة";
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.lblSummary);
            this.tabSummary.Location = new System.Drawing.Point(4, 25);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(10);
            this.tabSummary.Size = new System.Drawing.Size(708, 211);
            this.tabSummary.TabIndex = 3;
            this.tabSummary.Text = "4) ملخص";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // lblSummary
            // 
            this.lblSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSummary.Location = new System.Drawing.Point(10, 10);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(688, 191);
            this.lblSummary.TabIndex = 0;
            // 
            // lblStep
            // 
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblStep.Location = new System.Drawing.Point(18, 15);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(104, 21);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "الخطوة 1/4";
            // 
            // lblStatus
            // 
            this.lblStatus.ForeColor = System.Drawing.Color.DarkRed;
            this.lblStatus.Location = new System.Drawing.Point(18, 335);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(716, 40);
            this.lblStatus.TabIndex = 5;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(18, 295);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(110, 32);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "السابق";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(134, 295);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(110, 32);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "التالي";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnFinish
            // 
            this.btnFinish.Location = new System.Drawing.Point(624, 295);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(110, 32);
            this.btnFinish.TabIndex = 4;
            this.btnFinish.Text = "إنهاء";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // FirstTimeSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 390);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.tabWizard);
            this.Controls.Add(this.lblStep);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstTimeSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "إعداد النظام لأول مرة";
            this.Load += new System.EventHandler(this.FirstTimeSetup_Load);
            this.tabWizard.ResumeLayout(false);
            this.tabStore.ResumeLayout(false);
            this.tabStore.PerformLayout();
            this.tabAdmin.ResumeLayout(false);
            this.tabAdmin.PerformLayout();
            this.tabDefaults.ResumeLayout(false);
            this.tabDefaults.PerformLayout();
            this.tabSummary.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
