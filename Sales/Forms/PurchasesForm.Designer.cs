namespace Sales.Forms
{
    partial class PurchasesForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel top;
        private System.Windows.Forms.ComboBox cmbSuppliers;
        private System.Windows.Forms.DateTimePicker dtPurchaseDate;

        private System.Windows.Forms.Label lblSupplier;
        private System.Windows.Forms.Label lblPurchaseDate;

        private System.Windows.Forms.Panel entry;
        private System.Windows.Forms.Button btnPickProduct;
        private System.Windows.Forms.TextBox txtProductId;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.ComboBox cmbUnits;
        private System.Windows.Forms.TextBox txtQtyUnit;
        private System.Windows.Forms.TextBox txtCostPrice;
        private System.Windows.Forms.DateTimePicker dtExpiry;
        private System.Windows.Forms.CheckBox chkHasExpiry;
        private System.Windows.Forms.Button btnAddLine;
        private System.Windows.Forms.Button btnRemoveLine;
        private System.Windows.Forms.Button btnSave;

        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.Label lblCost;
        private System.Windows.Forms.Label lblExpiry;

        private System.Windows.Forms.DataGridView dgvLines;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PurchasesForm));
            this.top = new System.Windows.Forms.Panel();
            this.dtPurchaseDate = new System.Windows.Forms.DateTimePicker();
            this.lblPurchaseDate = new System.Windows.Forms.Label();
            this.cmbSuppliers = new System.Windows.Forms.ComboBox();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.entry = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRemoveLine = new System.Windows.Forms.Button();
            this.btnAddLine = new System.Windows.Forms.Button();
            this.chkHasExpiry = new System.Windows.Forms.CheckBox();
            this.dtExpiry = new System.Windows.Forms.DateTimePicker();
            this.lblExpiry = new System.Windows.Forms.Label();
            this.txtCostPrice = new System.Windows.Forms.TextBox();
            this.lblCost = new System.Windows.Forms.Label();
            this.txtQtyUnit = new System.Windows.Forms.TextBox();
            this.lblQty = new System.Windows.Forms.Label();
            this.cmbUnits = new System.Windows.Forms.ComboBox();
            this.lblUnit = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtProductId = new System.Windows.Forms.TextBox();
            this.btnPickProduct = new System.Windows.Forms.Button();
            this.lblProduct = new System.Windows.Forms.Label();
            this.dgvLines = new System.Windows.Forms.DataGridView();
            this.colProductId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductUnitId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQtyUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFactor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBaseQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCostPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExpiryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.top.SuspendLayout();
            this.entry.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).BeginInit();
            this.SuspendLayout();
            // 
            // top
            // 
            this.top.Controls.Add(this.dtPurchaseDate);
            this.top.Controls.Add(this.lblPurchaseDate);
            this.top.Controls.Add(this.cmbSuppliers);
            this.top.Controls.Add(this.lblSupplier);
            this.top.Dock = System.Windows.Forms.DockStyle.Top;
            this.top.Location = new System.Drawing.Point(0, 0);
            this.top.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.top.Name = "top";
            this.top.Padding = new System.Windows.Forms.Padding(15, 14, 15, 14);
            this.top.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.top.Size = new System.Drawing.Size(1150, 128);
            this.top.TabIndex = 0;
            // 
            // dtPurchaseDate
            // 
            this.dtPurchaseDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.dtPurchaseDate.Location = new System.Drawing.Point(15, 85);
            this.dtPurchaseDate.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dtPurchaseDate.Name = "dtPurchaseDate";
            this.dtPurchaseDate.Size = new System.Drawing.Size(1120, 30);
            this.dtPurchaseDate.TabIndex = 1;
            // 
            // lblPurchaseDate
            // 
            this.lblPurchaseDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPurchaseDate.Location = new System.Drawing.Point(15, 65);
            this.lblPurchaseDate.Name = "lblPurchaseDate";
            this.lblPurchaseDate.Size = new System.Drawing.Size(1120, 20);
            this.lblPurchaseDate.TabIndex = 3;
            this.lblPurchaseDate.Text = "تاريخ الشراء";
            this.lblPurchaseDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbSuppliers
            // 
            this.cmbSuppliers.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbSuppliers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSuppliers.FormattingEnabled = true;
            this.cmbSuppliers.Location = new System.Drawing.Point(15, 34);
            this.cmbSuppliers.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cmbSuppliers.Name = "cmbSuppliers";
            this.cmbSuppliers.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmbSuppliers.Size = new System.Drawing.Size(1120, 31);
            this.cmbSuppliers.TabIndex = 0;
            // 
            // lblSupplier
            // 
            this.lblSupplier.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSupplier.Location = new System.Drawing.Point(15, 14);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(1120, 20);
            this.lblSupplier.TabIndex = 2;
            this.lblSupplier.Text = "المورد";
            this.lblSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // entry
            // 
            this.entry.Controls.Add(this.btnSave);
            this.entry.Controls.Add(this.btnRemoveLine);
            this.entry.Controls.Add(this.btnAddLine);
            this.entry.Controls.Add(this.chkHasExpiry);
            this.entry.Controls.Add(this.dtExpiry);
            this.entry.Controls.Add(this.lblExpiry);
            this.entry.Controls.Add(this.txtCostPrice);
            this.entry.Controls.Add(this.lblCost);
            this.entry.Controls.Add(this.txtQtyUnit);
            this.entry.Controls.Add(this.lblQty);
            this.entry.Controls.Add(this.cmbUnits);
            this.entry.Controls.Add(this.lblUnit);
            this.entry.Controls.Add(this.txtProductName);
            this.entry.Controls.Add(this.txtProductId);
            this.entry.Controls.Add(this.btnPickProduct);
            this.entry.Controls.Add(this.lblProduct);
            this.entry.Dock = System.Windows.Forms.DockStyle.Top;
            this.entry.Location = new System.Drawing.Point(0, 128);
            this.entry.Name = "entry";
            this.entry.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.entry.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.entry.Size = new System.Drawing.Size(1150, 145);
            this.entry.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(15, 90);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 35);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "حفظ الفاتورة";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRemoveLine
            // 
            this.btnRemoveLine.Location = new System.Drawing.Point(165, 90);
            this.btnRemoveLine.Name = "btnRemoveLine";
            this.btnRemoveLine.Size = new System.Drawing.Size(140, 35);
            this.btnRemoveLine.TabIndex = 9;
            this.btnRemoveLine.Text = "حذف السطر";
            this.btnRemoveLine.UseVisualStyleBackColor = true;
            this.btnRemoveLine.Click += new System.EventHandler(this.btnRemoveLine_Click);
            // 
            // btnAddLine
            // 
            this.btnAddLine.Location = new System.Drawing.Point(315, 90);
            this.btnAddLine.Name = "btnAddLine";
            this.btnAddLine.Size = new System.Drawing.Size(140, 35);
            this.btnAddLine.TabIndex = 8;
            this.btnAddLine.Text = "إضافة سطر";
            this.btnAddLine.UseVisualStyleBackColor = true;
            this.btnAddLine.Click += new System.EventHandler(this.btnAddLine_Click);
            // 
            // chkHasExpiry
            // 
            this.chkHasExpiry.AutoSize = true;
            this.chkHasExpiry.Location = new System.Drawing.Point(475, 96);
            this.chkHasExpiry.Name = "chkHasExpiry";
            this.chkHasExpiry.Size = new System.Drawing.Size(127, 27);
            this.chkHasExpiry.TabIndex = 7;
            this.chkHasExpiry.Text = "له تاريخ انتهاء";
            this.chkHasExpiry.UseVisualStyleBackColor = true;
            this.chkHasExpiry.CheckedChanged += new System.EventHandler(this.chkHasExpiry_CheckedChanged);
            // 
            // dtExpiry
            // 
            this.dtExpiry.Enabled = false;
            this.dtExpiry.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtExpiry.Location = new System.Drawing.Point(615, 92);
            this.dtExpiry.Name = "dtExpiry";
            this.dtExpiry.Size = new System.Drawing.Size(140, 30);
            this.dtExpiry.TabIndex = 6;
            // 
            // lblExpiry
            // 
            this.lblExpiry.AutoSize = true;
            this.lblExpiry.Location = new System.Drawing.Point(632, 66);
            this.lblExpiry.Name = "lblExpiry";
            this.lblExpiry.Size = new System.Drawing.Size(97, 23);
            this.lblExpiry.TabIndex = 15;
            this.lblExpiry.Text = "تاريخ الانتهاء";
            // 
            // txtCostPrice
            // 
            this.txtCostPrice.Location = new System.Drawing.Point(765, 92);
            this.txtCostPrice.Name = "txtCostPrice";
            this.txtCostPrice.Size = new System.Drawing.Size(140, 30);
            this.txtCostPrice.TabIndex = 5;
            this.txtCostPrice.Text = "0";
            this.txtCostPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblCost
            // 
            this.lblCost.AutoSize = true;
            this.lblCost.Location = new System.Drawing.Point(796, 66);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(83, 23);
            this.lblCost.TabIndex = 14;
            this.lblCost.Text = "سعر التكلفة";
            // 
            // txtQtyUnit
            // 
            this.txtQtyUnit.Location = new System.Drawing.Point(915, 92);
            this.txtQtyUnit.Name = "txtQtyUnit";
            this.txtQtyUnit.Size = new System.Drawing.Size(100, 30);
            this.txtQtyUnit.TabIndex = 4;
            this.txtQtyUnit.Text = "1";
            this.txtQtyUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Location = new System.Drawing.Point(945, 66);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(47, 23);
            this.lblQty.TabIndex = 13;
            this.lblQty.Text = "الكمية";
            // 
            // cmbUnits
            // 
            this.cmbUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUnits.FormattingEnabled = true;
            this.cmbUnits.Location = new System.Drawing.Point(1025, 92);
            this.cmbUnits.Name = "cmbUnits";
            this.cmbUnits.Size = new System.Drawing.Size(110, 31);
            this.cmbUnits.TabIndex = 3;
            this.cmbUnits.SelectedValueChanged += new System.EventHandler(this.cmbUnits_SelectedValueChanged);
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(1054, 66);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(52, 23);
            this.lblUnit.TabIndex = 12;
            this.lblUnit.Text = "الوحدة";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(165, 25);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(740, 30);
            this.txtProductName.TabIndex = 2;
            // 
            // txtProductId
            // 
            this.txtProductId.Location = new System.Drawing.Point(915, 25);
            this.txtProductId.Name = "txtProductId";
            this.txtProductId.ReadOnly = true;
            this.txtProductId.Size = new System.Drawing.Size(100, 30);
            this.txtProductId.TabIndex = 1;
            this.txtProductId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPickProduct
            // 
            this.btnPickProduct.Location = new System.Drawing.Point(1025, 23);
            this.btnPickProduct.Name = "btnPickProduct";
            this.btnPickProduct.Size = new System.Drawing.Size(110, 35);
            this.btnPickProduct.TabIndex = 0;
            this.btnPickProduct.Text = "اختيار منتج";
            this.btnPickProduct.UseVisualStyleBackColor = true;
            this.btnPickProduct.Click += new System.EventHandler(this.btnPickProduct_Click);
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(1035, 0);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(48, 23);
            this.lblProduct.TabIndex = 11;
            this.lblProduct.Text = "المنتج";
            // 
            // dgvLines
            // 
            this.dgvLines.AllowUserToAddRows = false;
            this.dgvLines.AllowUserToDeleteRows = false;
            this.dgvLines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProductId,
            this.colProductUnitId,
            this.colProductName,
            this.colUnitName,
            this.colQtyUnit,
            this.colFactor,
            this.colBaseQty,
            this.colCostPrice,
            this.colExpiryDate});
            this.dgvLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLines.Location = new System.Drawing.Point(0, 273);
            this.dgvLines.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dgvLines.Name = "dgvLines";
            this.dgvLines.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvLines.RowHeadersWidth = 51;
            this.dgvLines.RowTemplate.Height = 24;
            this.dgvLines.Size = new System.Drawing.Size(1150, 327);
            this.dgvLines.TabIndex = 2;
            // 
            // colProductId
            // 
            this.colProductId.DataPropertyName = "ProductId";
            this.colProductId.HeaderText = "ProductId";
            this.colProductId.MinimumWidth = 6;
            this.colProductId.Name = "ProductId";
            this.colProductId.ReadOnly = true;
            this.colProductId.Visible = false;
            // 
            // colProductUnitId
            // 
            this.colProductUnitId.DataPropertyName = "ProductUnitId";
            this.colProductUnitId.HeaderText = "ProductUnitId";
            this.colProductUnitId.MinimumWidth = 6;
            this.colProductUnitId.Name = "ProductUnitId";
            this.colProductUnitId.ReadOnly = true;
            this.colProductUnitId.Visible = false;
            // 
            // colProductName
            // 
            this.colProductName.DataPropertyName = "ProductName";
            this.colProductName.HeaderText = "المنتج";
            this.colProductName.MinimumWidth = 6;
            this.colProductName.Name = "ProductName";
            this.colProductName.ReadOnly = true;
            // 
            // colUnitName
            // 
            this.colUnitName.DataPropertyName = "UnitName";
            this.colUnitName.HeaderText = "الوحدة";
            this.colUnitName.MinimumWidth = 6;
            this.colUnitName.Name = "UnitName";
            this.colUnitName.ReadOnly = true;
            // 
            // colQtyUnit
            // 
            this.colQtyUnit.DataPropertyName = "QtyUnit";
            dataGridViewCellStyle1.Format = "N2";
            this.colQtyUnit.DefaultCellStyle = dataGridViewCellStyle1;
            this.colQtyUnit.HeaderText = "الكمية";
            this.colQtyUnit.MinimumWidth = 6;
            this.colQtyUnit.Name = "QtyUnit";
            this.colQtyUnit.ReadOnly = true;
            // 
            // colFactor
            // 
            this.colFactor.DataPropertyName = "Factor";
            this.colFactor.HeaderText = "Factor";
            this.colFactor.MinimumWidth = 6;
            this.colFactor.Name = "Factor";
            this.colFactor.ReadOnly = true;
            this.colFactor.Visible = false;
            // 
            // colBaseQty
            // 
            this.colBaseQty.DataPropertyName = "BaseQty";
            dataGridViewCellStyle2.Format = "N2";
            this.colBaseQty.DefaultCellStyle = dataGridViewCellStyle2;
            this.colBaseQty.HeaderText = "الكمية الأساسية";
            this.colBaseQty.MinimumWidth = 6;
            this.colBaseQty.Name = "BaseQty";
            this.colBaseQty.ReadOnly = true;
            // 
            // colCostPrice
            // 
            this.colCostPrice.DataPropertyName = "CostPrice";
            dataGridViewCellStyle3.Format = "N2";
            this.colCostPrice.DefaultCellStyle = dataGridViewCellStyle3;
            this.colCostPrice.HeaderText = "سعر التكلفة";
            this.colCostPrice.MinimumWidth = 6;
            this.colCostPrice.Name = "CostPrice";
            this.colCostPrice.ReadOnly = true;
            // 
            // colExpiryDate
            // 
            this.colExpiryDate.DataPropertyName = "ExpiryDate";
            this.colExpiryDate.HeaderText = "تاريخ الانتهاء";
            this.colExpiryDate.MinimumWidth = 6;
            this.colExpiryDate.Name = "ExpiryDate";
            this.colExpiryDate.ReadOnly = true;
            // 
            // PurchasesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 600);
            this.Controls.Add(this.dgvLines);
            this.Controls.Add(this.entry);
            this.Controls.Add(this.top);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PurchasesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "المشتريات";
            this.Load += new System.EventHandler(this.PurchasesForm_Load);
            this.top.ResumeLayout(false);
            this.entry.ResumeLayout(false);
            this.entry.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridViewTextBoxColumn colProductId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductUnitId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQtyUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFactor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBaseQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCostPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExpiryDate;
    }
}
