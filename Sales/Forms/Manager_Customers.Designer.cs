namespace Sales.Forms

{

    partial class Manager_Customers

    {

        /// <summary>

        /// Required designer variable.

        /// </summary>

        private System.ComponentModel.IContainer components = null;



        /// <summary>

        /// Clean up any resources being used.

        /// </summary>

        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        protected override void Dispose(bool disposing)

        {

            if (disposing && (components != null))

            {

                components.Dispose();

            }

            base.Dispose(disposing);

        }



        #region Windows Form Designer generated code



        /// <summary>

        /// Required method for Designer support - do not modify

        /// the contents of this method with the code editor.

        /// </summary>

        private void InitializeComponent()

        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager_Customers));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView_Customer = new System.Windows.Forms.DataGridView();
            this.ColId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColBalance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSaveCustomer = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteCustomer = new System.Windows.Forms.ToolStripButton();
            this.btnExportCsv = new System.Windows.Forms.ToolStripButton();
            this.btnExportPdf = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.btnClearSearch = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Customer)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("PT Bold Broken", 10.8F);
            this.groupBox1.Location = new System.Drawing.Point(20, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(1058, 77);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "اداره العملاء";
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.SystemColors.Info;
            this.txtSearch.Location = new System.Drawing.Point(79, 32);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(6);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtSearch.Size = new System.Drawing.Size(484, 31);
            this.txtSearch.TabIndex = 139;
            this.txtSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSearch.WordWrap = false;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("PT Bold Broken", 10.8F);
            this.label2.Location = new System.Drawing.Point(582, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 31);
            this.label2.TabIndex = 138;
            this.label2.Text = "بحث";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtUserName.Enabled = false;
            this.txtUserName.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(733, 29);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(6);
            this.txtUserName.Multiline = true;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtUserName.Size = new System.Drawing.Size(207, 29);
            this.txtUserName.TabIndex = 136;
            this.txtUserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtUserName.WordWrap = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(949, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "اسم المستخدم:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dataGridView_Customer
            // 
            this.dataGridView_Customer.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_Customer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Customer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColId,
            this.ColName,
            this.ColTel,
            this.ColAddress,
            this.ColBalance,
            this.ColNote});
            this.dataGridView_Customer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Customer.Location = new System.Drawing.Point(20, 136);
            this.dataGridView_Customer.Name = "dataGridView_Customer";
            this.dataGridView_Customer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dataGridView_Customer.RowHeadersWidth = 51;
            this.dataGridView_Customer.RowTemplate.Height = 26;
            this.dataGridView_Customer.Size = new System.Drawing.Size(1058, 408);
            this.dataGridView_Customer.TabIndex = 34;
            // 
            // ColId
            // 
            this.ColId.DataPropertyName = "Id";
            this.ColId.HeaderText = "المعرف";
            this.ColId.MinimumWidth = 6;
            this.ColId.Name = "ColId";
            this.ColId.Visible = false;
            // 
            // ColName
            // 
            this.ColName.DataPropertyName = "Name";
            this.ColName.HeaderText = "اسم العميل ";
            this.ColName.MinimumWidth = 6;
            this.ColName.Name = "ColName";
            // 
            // ColTel
            // 
            this.ColTel.DataPropertyName = "Tel";
            this.ColTel.HeaderText = "رقم العميل";
            this.ColTel.MinimumWidth = 6;
            this.ColTel.Name = "ColTel";
            // 
            // ColAddress
            // 
            this.ColAddress.DataPropertyName = "Address";
            this.ColAddress.HeaderText = "العنوان";
            this.ColAddress.MinimumWidth = 6;
            this.ColAddress.Name = "ColAddress";
            // 
            // ColBalance
            // 
            this.ColBalance.DataPropertyName = "Balance";
            this.ColBalance.HeaderText = "حساب العميل";
            this.ColBalance.MinimumWidth = 6;
            this.ColBalance.Name = "ColBalance";
            this.ColBalance.ReadOnly = true;
            // 
            // ColNote
            // 
            this.ColNote.DataPropertyName = "Details";
            this.ColNote.HeaderText = "تفاصيل العميل ان وجد";
            this.ColNote.MinimumWidth = 6;
            this.ColNote.Name = "ColNote";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveCustomer,
            this.btnDeleteCustomer,
            this.btnExportCsv,
            this.btnExportPdf,
            this.btnPrint,
            this.toolStripSeparator1,
            this.btnClose,
            this.btnClearSearch});
            this.toolStrip1.Location = new System.Drawing.Point(20, 20);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStrip1.Size = new System.Drawing.Size(1058, 39);
            this.toolStrip1.TabIndex = 35;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // btnSaveCustomer
            // 
            this.btnSaveCustomer.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveCustomer.Image")));
            this.btnSaveCustomer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveCustomer.Name = "btnSaveCustomer";
            this.btnSaveCustomer.Size = new System.Drawing.Size(142, 36);
            this.btnSaveCustomer.Text = "حفظ المعاملات";
            this.btnSaveCustomer.Click += new System.EventHandler(this.btnSaveCustomer_Click);
            // 
            // btnDeleteCustomer
            // 
            this.btnDeleteCustomer.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteCustomer.Image")));
            this.btnDeleteCustomer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteCustomer.Name = "btnDeleteCustomer";
            this.btnDeleteCustomer.Size = new System.Drawing.Size(170, 36);
            this.btnDeleteCustomer.Text = "حذف العميل المحدد";
            this.btnDeleteCustomer.Click += new System.EventHandler(this.btnDeleteCustomer_Click);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(99, 36);
            this.btnExportCsv.Text = "تصدير CSV";
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            this.btnExportPdf.Text = "تصدير PDF"; this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            this.btnPrint.Text = "طباعة"; this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(82, 36);
            this.btnClearSearch.Text = "إلغاء البحث";
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // btnClose
            // 
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(78, 36);
            this.btnClose.Text = "إغلاق";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Manager_Customers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 564);
            this.Controls.Add(this.dataGridView_Customer);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MinimizeBox = false;
            this.Name = "Manager_Customers";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "اداره العملاء";
            this.Load += new System.EventHandler(this.Manager_Customers_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Customer)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        private void btnSaveCustomers_Click(object sender, System.EventArgs e)

        {

            throw new System.NotImplementedException();

        }



        #endregion



        private System.Windows.Forms.GroupBox groupBox1;

        private System.Windows.Forms.TextBox txtSearch;

        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.TextBox txtUserName;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.DataGridView dataGridView_Customer;

        private System.Windows.Forms.DataGridViewTextBoxColumn ColId;

        private System.Windows.Forms.DataGridViewTextBoxColumn ColName;

        private System.Windows.Forms.DataGridViewTextBoxColumn ColTel;

        private System.Windows.Forms.DataGridViewTextBoxColumn ColAddress;

        private System.Windows.Forms.DataGridViewTextBoxColumn ColBalance;

        private System.Windows.Forms.DataGridViewTextBoxColumn ColNote;

        private System.Windows.Forms.ToolStrip toolStrip1;

        private System.Windows.Forms.ToolStripButton btnSaveCustomer;

        private System.Windows.Forms.ToolStripButton btnDeleteCustomer;

        private System.Windows.Forms.ToolStripButton btnExportCsv;
        private System.Windows.Forms.ToolStripButton btnExportPdf;
        private System.Windows.Forms.ToolStripButton btnPrint;

        private System.Windows.Forms.ToolStripButton btnClearSearch;

        private System.Windows.Forms.ToolStripButton btnClose;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

    }

}