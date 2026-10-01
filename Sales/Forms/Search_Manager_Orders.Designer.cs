namespace Sales.Forms
{
    partial class Search_Manager_Orders
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Search_Manager_Orders));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnDeleteOrders = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnEditOrders = new System.Windows.Forms.Button();
            this.dataGridView_Search_Orders = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiscount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNetTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddOrders = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Search_Orders)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDeleteOrders
            // 
            this.btnDeleteOrders.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDeleteOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteOrders.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnDeleteOrders.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.btnDeleteOrders.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteOrders.Image")));
            this.btnDeleteOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteOrders.Location = new System.Drawing.Point(516, 0);
            this.btnDeleteOrders.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnDeleteOrders.Name = "btnDeleteOrders";
            this.btnDeleteOrders.Padding = new System.Windows.Forms.Padding(20);
            this.btnDeleteOrders.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnDeleteOrders.Size = new System.Drawing.Size(228, 76);
            this.btnDeleteOrders.TabIndex = 31;
            this.btnDeleteOrders.Text = "حذف الفاتورةالمحددة";
            this.btnDeleteOrders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDeleteOrders.UseVisualStyleBackColor = false;
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnExportCsv.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportCsv.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnExportCsv.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.btnExportCsv.Location = new System.Drawing.Point(346, 0);
            this.btnExportCsv.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Padding = new System.Windows.Forms.Padding(20);
            this.btnExportCsv.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnExportCsv.Size = new System.Drawing.Size(170, 76);
            this.btnExportCsv.TabIndex = 32;
            this.btnExportCsv.Text = "تصدير CSV";
            this.btnExportCsv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportCsv.UseVisualStyleBackColor = false;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnExportPdf.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.btnExportPdf.Location = new System.Drawing.Point(516, 0);
            this.btnExportPdf.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Padding = new System.Windows.Forms.Padding(20);
            this.btnExportPdf.Size = new System.Drawing.Size(170, 76);
            this.btnExportPdf.TabIndex = 34;
            this.btnExportPdf.Text = "تصدير PDF";
            this.btnExportPdf.UseVisualStyleBackColor = false;
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnPrint.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.btnPrint.Location = new System.Drawing.Point(686, 0);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Padding = new System.Windows.Forms.Padding(20);
            this.btnPrint.Size = new System.Drawing.Size(170, 76);
            this.btnPrint.TabIndex = 35;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClearSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearSearch.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnClearSearch.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.btnClearSearch.Location = new System.Drawing.Point(176, 0);
            this.btnClearSearch.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Padding = new System.Windows.Forms.Padding(20);
            this.btnClearSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnClearSearch.Size = new System.Drawing.Size(170, 76);
            this.btnClearSearch.TabIndex = 33;
            this.btnClearSearch.Text = "مسح البحث";
            this.btnClearSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClearSearch.UseVisualStyleBackColor = false;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(0, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(20);
            this.btnClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnClose.Size = new System.Drawing.Size(176, 76);
            this.btnClose.TabIndex = 30;
            this.btnClose.Text = "اغلاق";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnEditOrders
            // 
            this.btnEditOrders.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEditOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditOrders.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnEditOrders.Image = ((System.Drawing.Image)(resources.GetObject("btnEditOrders.Image")));
            this.btnEditOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditOrders.Location = new System.Drawing.Point(621, 0);
            this.btnEditOrders.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnEditOrders.Name = "btnEditOrders";
            this.btnEditOrders.Padding = new System.Windows.Forms.Padding(20);
            this.btnEditOrders.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEditOrders.Size = new System.Drawing.Size(234, 76);
            this.btnEditOrders.TabIndex = 29;
            this.btnEditOrders.Text = "تعديل الفاتورة المحددة";
            this.btnEditOrders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEditOrders.UseVisualStyleBackColor = false;
            this.btnEditOrders.Click += new System.EventHandler(this.btnEditOrders_Click);
            // 
            // dataGridView_Search_Orders
            // 
            this.dataGridView_Search_Orders.AllowUserToAddRows = false;
            this.dataGridView_Search_Orders.AllowUserToDeleteRows = false;
            this.dataGridView_Search_Orders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_Search_Orders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Search_Orders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colOrderDate,
            this.colCreatedBy,
            this.colNote,
            this.colTotal,
            this.colCustomerName,
            this.colProductCount,
            this.colDiscount,
            this.colNetTotal});
            this.dataGridView_Search_Orders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Search_Orders.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Search_Orders.Margin = new System.Windows.Forms.Padding(6);
            this.dataGridView_Search_Orders.MultiSelect = false;
            this.dataGridView_Search_Orders.Name = "dataGridView_Search_Orders";
            this.dataGridView_Search_Orders.ReadOnly = true;
            this.dataGridView_Search_Orders.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dataGridView_Search_Orders.RowHeadersVisible = false;
            this.dataGridView_Search_Orders.RowHeadersWidth = 51;
            this.dataGridView_Search_Orders.RowTemplate.Height = 26;
            this.dataGridView_Search_Orders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_Search_Orders.Size = new System.Drawing.Size(1125, 462);
            this.dataGridView_Search_Orders.TabIndex = 1;
            // 
            // colId
            // 
            this.colId.DataPropertyName = "id";
            this.colId.HeaderText = "المعرف";
            this.colId.MinimumWidth = 6;
            this.colId.Name = "id";
            this.colId.ReadOnly = true;
            this.colId.Width = 70;
            // 
            // colOrderDate
            // 
            this.colOrderDate.DataPropertyName = "order_date";
            this.colOrderDate.HeaderText = "تاريخ البيع";
            this.colOrderDate.MinimumWidth = 6;
            this.colOrderDate.Name = "order_date";
            this.colOrderDate.ReadOnly = true;
            this.colOrderDate.Width = 300;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.DataPropertyName = "created_by";
            this.colCreatedBy.HeaderText = "البائع";
            this.colCreatedBy.MinimumWidth = 6;
            this.colCreatedBy.Name = "created_by";
            this.colCreatedBy.ReadOnly = true;
            // 
            // colNote
            // 
            this.colNote.DataPropertyName = "note";
            this.colNote.HeaderText = "وصف الفاتورة";
            this.colNote.MinimumWidth = 6;
            this.colNote.Name = "note";
            this.colNote.ReadOnly = true;
            // 
            // colTotal
            // 
            this.colTotal.DataPropertyName = "total";
            dataGridViewCellStyle1.Format = "N0";
            this.colTotal.DefaultCellStyle = dataGridViewCellStyle1;
            this.colTotal.HeaderText = "اجمالي السعر";
            this.colTotal.MinimumWidth = 6;
            this.colTotal.Name = "total";
            this.colTotal.ReadOnly = true;
            // 
            // colCustomerName
            // 
            this.colCustomerName.DataPropertyName = "name";
            this.colCustomerName.HeaderText = "اسم العميل";
            this.colCustomerName.MinimumWidth = 6;
            this.colCustomerName.Name = "name";
            this.colCustomerName.ReadOnly = true;
            // 
            // colProductCount
            // 
            this.colProductCount.DataPropertyName = "ProductCount";
            this.colProductCount.HeaderText = "عدد المنتجات";
            this.colProductCount.MinimumWidth = 6;
            this.colProductCount.Name = "ProductCount";
            this.colProductCount.ReadOnly = true;
            // 
            // colDiscount
            // 
            this.colDiscount.DataPropertyName = "discount";
            dataGridViewCellStyle2.Format = "N0";
            this.colDiscount.DefaultCellStyle = dataGridViewCellStyle2;
            this.colDiscount.HeaderText = "الخصم";
            this.colDiscount.MinimumWidth = 6;
            this.colDiscount.Name = "discount";
            this.colDiscount.ReadOnly = true;
            // 
            // colNetTotal
            // 
            this.colNetTotal.DataPropertyName = "net_total";
            dataGridViewCellStyle3.Format = "N0";
            this.colNetTotal.DefaultCellStyle = dataGridViewCellStyle3;
            this.colNetTotal.HeaderText = "الصافي";
            this.colNetTotal.MinimumWidth = 6;
            this.colNetTotal.Name = "net_total";
            this.colNetTotal.ReadOnly = true;
            // 
            // btnAddOrders
            // 
            this.btnAddOrders.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnAddOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddOrders.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddOrders.Image = ((System.Drawing.Image)(resources.GetObject("btnAddOrders.Image")));
            this.btnAddOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddOrders.Location = new System.Drawing.Point(855, 0);
            this.btnAddOrders.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.btnAddOrders.Name = "btnAddOrders";
            this.btnAddOrders.Padding = new System.Windows.Forms.Padding(20);
            this.btnAddOrders.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnAddOrders.Size = new System.Drawing.Size(270, 76);
            this.btnAddOrders.TabIndex = 28;
            this.btnAddOrders.Text = "فاتورة جديدة";
            this.btnAddOrders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddOrders.UseVisualStyleBackColor = false;
            this.btnAddOrders.Click += new System.EventHandler(this.btnAddOrders_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Sales.Properties.Resources.pngegg__1_;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Controls.Add(this.btnDeleteOrders);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnExportPdf);
            this.panel1.Controls.Add(this.btnExportCsv);
            this.panel1.Controls.Add(this.btnClearSearch);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnEditOrders);
            this.panel1.Controls.Add(this.btnAddOrders);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.panel1.Location = new System.Drawing.Point(20, 480);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1125, 76);
            this.panel1.TabIndex = 33;
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.SystemColors.Info;
            this.txtSearch.Location = new System.Drawing.Point(89, 33);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(6);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtSearch.Size = new System.Drawing.Size(484, 31);
            this.txtSearch.TabIndex = 139;
            this.txtSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSearch.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("PT Bold Broken", 10.8F);
            this.label2.Location = new System.Drawing.Point(623, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 34);
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
            this.txtUserName.Location = new System.Drawing.Point(800, 29);
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
            this.label1.Location = new System.Drawing.Point(1016, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "اسم المستخدم:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Sales.Properties.Resources.pngegg__1_;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel2.Controls.Add(this.dataGridView_Search_Orders);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.panel2.Location = new System.Drawing.Point(20, 94);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1125, 462);
            this.panel2.TabIndex = 35;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("PT Bold Broken", 10.8F);
            this.groupBox1.Location = new System.Drawing.Point(20, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(1125, 74);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "البحث عن الفاتوره";
            // 
            // Search_Manager_Orders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1165, 576);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MinimizeBox = false;
            this.Name = "Search_Manager_Orders";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "شاشة البحث عن المبيعات";
            this.Load += new System.EventHandler(this.Search_Manager_Orders_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Search_Orders)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDeleteOrders;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnEditOrders;
        private System.Windows.Forms.DataGridView dataGridView_Search_Orders;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNote;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiscount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNetTotal;
        private System.Windows.Forms.Button btnAddOrders;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}