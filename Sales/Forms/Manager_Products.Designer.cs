namespace Sales.Forms
{
    partial class Manager_Products
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager_Products));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.combFilter = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.checkCategorie = new System.Windows.Forms.CheckBox();
            this.checkProduct = new System.Windows.Forms.CheckBox();
            this.checkBarcode = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ColLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCategoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDefaultBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSku = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCostPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMinQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColExpiryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCreatedAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddProduct = new System.Windows.Forms.ToolStripButton();
            this.btnEditProduct = new System.Windows.Forms.ToolStripButton();
            this.btnProductUnits = new System.Windows.Forms.ToolStripButton();
            this.btnManageCategories = new System.Windows.Forms.ToolStripButton();
            this.btnManageUnits = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteProduct = new System.Windows.Forms.ToolStripButton();
            this.btnClearSearch = new System.Windows.Forms.ToolStripButton();
            this.btnExportCsv = new System.Windows.Forms.ToolStripButton();
            this.btnExportPdf = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.combFilter);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.checkCategorie);
            this.groupBox1.Controls.Add(this.checkProduct);
            this.groupBox1.Controls.Add(this.checkBarcode);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("PT Bold Broken", 10.8F);
            this.groupBox1.Location = new System.Drawing.Point(20, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(1131, 113);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ادراة المنتجات";
            // 
            // combFilter
            // 
            this.combFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combFilter.ItemHeight = 31;
            this.combFilter.Location = new System.Drawing.Point(408, 68);
            this.combFilter.Name = "combFilter";
            this.combFilter.Size = new System.Drawing.Size(172, 39);
            this.combFilter.TabIndex = 144;
            this.combFilter.SelectedIndexChanged += new System.EventHandler(this.combFilter_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.SystemColors.Info;
            this.txtSearch.Location = new System.Drawing.Point(9, 33);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(6);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtSearch.Size = new System.Drawing.Size(369, 31);
            this.txtSearch.TabIndex = 139;
            this.txtSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSearch.WordWrap = false;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // checkCategorie
            // 
            this.checkCategorie.AutoSize = true;
            this.checkCategorie.Location = new System.Drawing.Point(408, 29);
            this.checkCategorie.Name = "checkCategorie";
            this.checkCategorie.Size = new System.Drawing.Size(81, 35);
            this.checkCategorie.TabIndex = 140;
            this.checkCategorie.Text = "الصنف";
            this.checkCategorie.UseVisualStyleBackColor = true;
            this.checkCategorie.CheckedChanged += new System.EventHandler(this.checkCategorie_CheckedChanged);
            // 
            // checkProduct
            // 
            this.checkProduct.AutoSize = true;
            this.checkProduct.Checked = true;
            this.checkProduct.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkProduct.Location = new System.Drawing.Point(506, 29);
            this.checkProduct.Name = "checkProduct";
            this.checkProduct.Size = new System.Drawing.Size(74, 35);
            this.checkProduct.TabIndex = 141;
            this.checkProduct.Text = "المنتج";
            this.checkProduct.UseVisualStyleBackColor = true;
            this.checkProduct.CheckedChanged += new System.EventHandler(this.checkProduct_CheckedChanged);
            // 
            // checkBarcode
            // 
            this.checkBarcode.AutoSize = true;
            this.checkBarcode.Location = new System.Drawing.Point(596, 72);
            this.checkBarcode.Name = "checkBarcode";
            this.checkBarcode.Size = new System.Drawing.Size(88, 35);
            this.checkBarcode.TabIndex = 145;
            this.checkBarcode.Text = "الباركود";
            this.checkBarcode.UseVisualStyleBackColor = true;
            this.checkBarcode.CheckedChanged += new System.EventHandler(this.checkBarcode_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("PT Bold Broken", 10.8F);
            this.label2.Location = new System.Drawing.Point(623, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 31);
            this.label2.TabIndex = 138;
            this.label2.Text = "بحث ب:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtUserName.Enabled = false;
            this.txtUserName.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(806, 29);
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
            this.label1.Location = new System.Drawing.Point(1022, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "اسم المستخدم:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.panel2.Location = new System.Drawing.Point(20, 172);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1131, 338);
            this.panel2.TabIndex = 32;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColLabel,
            this.ColCategoryName,
            this.ColDefaultBarcode,
            this.ColSku,
            this.ColQty,
            this.ColPrice,
            this.ColCostPrice,
            this.ColMinQty,
            this.ColNote,
            this.ColExpiryDate,
            this.ColCreatedAt,
            this.ColCreatedBy,
            this.ColId});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 26;
            this.dataGridView1.Size = new System.Drawing.Size(1131, 338);
            this.dataGridView1.TabIndex = 0;
            // 
            // ColLabel
            // 
            this.ColLabel.DataPropertyName = "Label";
            this.ColLabel.FillWeight = 200F;
            this.ColLabel.HeaderText = "اسم المنتج";
            this.ColLabel.MinimumWidth = 120;
            this.ColLabel.Name = "ColLabel";
            // 
            // ColCategoryName
            // 
            this.ColCategoryName.DataPropertyName = "CategoryName";
            this.ColCategoryName.FillWeight = 130F;
            this.ColCategoryName.HeaderText = "الصنف";
            this.ColCategoryName.MinimumWidth = 100;
            this.ColCategoryName.Name = "ColCategoryName";
            // 
            // ColDefaultBarcode
            // 
            this.ColDefaultBarcode.DataPropertyName = "DefaultBarcode";
            this.ColDefaultBarcode.HeaderText = "الباركود";
            this.ColDefaultBarcode.MinimumWidth = 90;
            this.ColDefaultBarcode.Name = "ColDefaultBarcode";
            // 
            // ColSku
            // 
            this.ColSku.DataPropertyName = "Sku";
            this.ColSku.FillWeight = 90F;
            this.ColSku.HeaderText = "الكود الداخلي";
            this.ColSku.MinimumWidth = 80;
            this.ColSku.Name = "ColSku";
            // 
            // ColQty
            // 
            this.ColQty.DataPropertyName = "DisplayQty";
            this.ColQty.FillWeight = 70F;
            this.ColQty.HeaderText = "الكمية";
            this.ColQty.MinimumWidth = 60;
            this.ColQty.Name = "ColQty";
            // 
            // ColPrice
            // 
            this.ColPrice.DataPropertyName = "Price";
            this.ColPrice.FillWeight = 70F;
            this.ColPrice.HeaderText = "السعر";
            this.ColPrice.MinimumWidth = 60;
            this.ColPrice.Name = "ColPrice";
            // 
            // ColCostPrice
            // 
            this.ColCostPrice.DataPropertyName = "CostPrice";
            this.ColCostPrice.FillWeight = 70F;
            this.ColCostPrice.HeaderText = "التكلفة";
            this.ColCostPrice.MinimumWidth = 60;
            this.ColCostPrice.Name = "ColCostPrice";
            // 
            // ColMinQty
            // 
            this.ColMinQty.DataPropertyName = "MinQty";
            this.ColMinQty.FillWeight = 65F;
            this.ColMinQty.HeaderText = "الحد الأدنى";
            this.ColMinQty.MinimumWidth = 60;
            this.ColMinQty.Name = "ColMinQty";
            // 
            // ColNote
            // 
            this.ColNote.DataPropertyName = "Note";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColNote.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColNote.FillWeight = 150F;
            this.ColNote.HeaderText = "تفاصيل المنتج";
            this.ColNote.MinimumWidth = 100;
            this.ColNote.Name = "ColNote";
            // 
            // ColExpiryDate
            // 
            this.ColExpiryDate.DataPropertyName = "ExpiryDate";
            this.ColExpiryDate.FillWeight = 80F;
            this.ColExpiryDate.HeaderText = "انتهاء الصلاحية";
            this.ColExpiryDate.MinimumWidth = 70;
            this.ColExpiryDate.Name = "ColExpiryDate";
            // 
            // ColCreatedAt
            // 
            this.ColCreatedAt.DataPropertyName = "CreatedAt";
            this.ColCreatedAt.FillWeight = 90F;
            this.ColCreatedAt.HeaderText = "تاريخ الإضافة";
            this.ColCreatedAt.MinimumWidth = 70;
            this.ColCreatedAt.Name = "ColCreatedAt";
            // 
            // ColCreatedBy
            // 
            this.ColCreatedBy.DataPropertyName = "CreatedBy";
            this.ColCreatedBy.FillWeight = 90F;
            this.ColCreatedBy.HeaderText = "المستخدم";
            this.ColCreatedBy.MinimumWidth = 70;
            this.ColCreatedBy.Name = "ColCreatedBy";
            // 
            // ColId
            // 
            this.ColId.DataPropertyName = "Id";
            this.ColId.HeaderText = "المعرف";
            this.ColId.MinimumWidth = 6;
            this.ColId.Name = "ColId";
            this.ColId.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddProduct,
            this.btnEditProduct,
            this.btnProductUnits,
            this.btnManageCategories,
            this.btnManageUnits,
            this.btnDeleteProduct,
            this.btnClearSearch,
            this.btnExportCsv,
            this.btnExportPdf,
            this.btnPrint,
            this.toolStripSeparator1,
            this.btnClose});
            this.toolStrip1.Location = new System.Drawing.Point(20, 20);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStrip1.Size = new System.Drawing.Size(1131, 39);
            this.toolStrip1.TabIndex = 35;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddProduct
            // 
            this.btnAddProduct.Image = ((System.Drawing.Image)(resources.GetObject("btnAddProduct.Image")));
            this.btnAddProduct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Size = new System.Drawing.Size(84, 36);
            this.btnAddProduct.Text = "إضافة";
            this.btnAddProduct.Click += new System.EventHandler(this.btnِAddProduct_Click);
            // 
            // btnEditProduct
            // 
            this.btnEditProduct.Image = ((System.Drawing.Image)(resources.GetObject("btnEditProduct.Image")));
            this.btnEditProduct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditProduct.Name = "btnEditProduct";
            this.btnEditProduct.Size = new System.Drawing.Size(82, 36);
            this.btnEditProduct.Text = "تعديل";
            this.btnEditProduct.Click += new System.EventHandler(this.btnEditProduct_Click);
            // 
            // btnProductUnits
            // 
            this.btnProductUnits.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnProductUnits.Name = "btnProductUnits";
            this.btnProductUnits.Size = new System.Drawing.Size(157, 36);
            this.btnProductUnits.Text = "إدارة الوحدات والباركود";
            this.btnProductUnits.Click += new System.EventHandler(this.btnProductUnits_Click);
            // 
            // btnManageCategories
            // 
            this.btnManageCategories.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnManageCategories.Name = "btnManageCategories";
            this.btnManageCategories.Size = new System.Drawing.Size(65, 36);
            this.btnManageCategories.Text = "الأصناف";
            this.btnManageCategories.Click += new System.EventHandler(this.btnManageCategories_Click);
            // 
            // btnManageUnits
            // 
            this.btnManageUnits.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnManageUnits.Name = "btnManageUnits";
            this.btnManageUnits.Size = new System.Drawing.Size(65, 36);
            this.btnManageUnits.Text = "الوحدات";
            this.btnManageUnits.Click += new System.EventHandler(this.btnManageUnits_Click);
            // 
            // btnDeleteProduct
            // 
            this.btnDeleteProduct.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteProduct.Image")));
            this.btnDeleteProduct.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteProduct.Name = "btnDeleteProduct";
            this.btnDeleteProduct.Size = new System.Drawing.Size(77, 36);
            this.btnDeleteProduct.Text = "حذف";
            this.btnDeleteProduct.Click += new System.EventHandler(this.btnDeleteProduct_Click);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(85, 36);
            this.btnClearSearch.Text = "مسح البحث";
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(82, 36);
            this.btnExportCsv.Text = "تصدير CSV";
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(82, 36);
            this.btnExportPdf.Text = "تصدير PDF";
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(52, 36);
            this.btnPrint.Text = "طباعة";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
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
            // Manager_Products
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 530);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "Manager_Products";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "اداره المنتجات";
            this.Load += new System.EventHandler(this.Manager_Products_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.CheckBox checkCategorie;
        private System.Windows.Forms.CheckBox checkProduct;
        private System.Windows.Forms.CheckBox checkBarcode;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCategoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDefaultBarcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSku;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCostPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMinQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNote;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColExpiryDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCreatedAt;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColId;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddProduct;
        private System.Windows.Forms.ToolStripButton btnEditProduct;
        private System.Windows.Forms.ToolStripButton btnProductUnits;
        private System.Windows.Forms.ToolStripButton btnManageCategories;
        private System.Windows.Forms.ToolStripButton btnManageUnits;
        private System.Windows.Forms.ToolStripButton btnDeleteProduct;
        private System.Windows.Forms.ToolStripButton btnClearSearch;
        private System.Windows.Forms.ToolStripButton btnExportCsv;
        private System.Windows.Forms.ToolStripButton btnExportPdf;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ComboBox combFilter;
    }
}