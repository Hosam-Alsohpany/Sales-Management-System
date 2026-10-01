namespace Sales.Forms
{
    partial class PosHotkeysForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Button btnPickProduct;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.ComboBox cmbUnits;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.TextBox txtQtyDelta;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnClose;

        private System.Windows.Forms.DataGridView dgv;

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
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtQtyDelta = new System.Windows.Forms.TextBox();
            this.lblQty = new System.Windows.Forms.Label();
            this.cmbUnits = new System.Windows.Forms.ComboBox();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.btnPickProduct = new System.Windows.Forms.Button();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lblKey = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.key_code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.product_unit_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.product_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unit_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.factor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty_delta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updated_at = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Controls.Add(this.btnNew);
            this.panelTop.Controls.Add(this.btnDelete);
            this.panelTop.Controls.Add(this.btnSave);
            this.panelTop.Controls.Add(this.txtNote);
            this.panelTop.Controls.Add(this.lblNote);
            this.panelTop.Controls.Add(this.txtQtyDelta);
            this.panelTop.Controls.Add(this.lblQty);
            this.panelTop.Controls.Add(this.cmbUnits);
            this.panelTop.Controls.Add(this.txtProduct);
            this.panelTop.Controls.Add(this.btnPickProduct);
            this.panelTop.Controls.Add(this.txtKey);
            this.panelTop.Controls.Add(this.lblKey);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(9, 10, 9, 10);
            this.panelTop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panelTop.Size = new System.Drawing.Size(875, 167);
            this.panelTop.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnClose.Location = new System.Drawing.Point(263, 82);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(79, 75);
            this.btnClose.TabIndex = 12;
            this.btnClose.Text = "خروج";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNew
            // 
            this.btnNew.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnNew.Location = new System.Drawing.Point(184, 82);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(79, 75);
            this.btnNew.TabIndex = 11;
            this.btnNew.Text = "جديد";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnDelete.Location = new System.Drawing.Point(105, 82);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(79, 75);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "حذف";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSave.Location = new System.Drawing.Point(9, 82);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 75);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "حفظ";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtNote
            // 
            this.txtNote.Location = new System.Drawing.Point(348, 82);
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(294, 24);
            this.txtNote.TabIndex = 8;
            this.txtNote.Visible = false;
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblNote.Location = new System.Drawing.Point(549, 82);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(52, 17);
            this.lblNote.TabIndex = 7;
            this.lblNote.Text = "ملاحظة";
            // 
            // txtQtyDelta
            // 
            this.txtQtyDelta.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtQtyDelta.Location = new System.Drawing.Point(601, 82);
            this.txtQtyDelta.Name = "txtQtyDelta";
            this.txtQtyDelta.Size = new System.Drawing.Size(84, 24);
            this.txtQtyDelta.TabIndex = 6;
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblQty.Location = new System.Drawing.Point(685, 82);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(44, 17);
            this.lblQty.TabIndex = 5;
            this.lblQty.Text = "الكمية";
            // 
            // cmbUnits
            // 
            this.cmbUnits.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUnits.FormattingEnabled = true;
            this.cmbUnits.Location = new System.Drawing.Point(9, 58);
            this.cmbUnits.Name = "cmbUnits";
            this.cmbUnits.Size = new System.Drawing.Size(720, 24);
            this.cmbUnits.TabIndex = 4;
            this.cmbUnits.SelectedValueChanged += new System.EventHandler(this.cmbUnits_SelectedValueChanged);
            // 
            // txtProduct
            // 
            this.txtProduct.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtProduct.Location = new System.Drawing.Point(9, 34);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.ReadOnly = true;
            this.txtProduct.Size = new System.Drawing.Size(720, 24);
            this.txtProduct.TabIndex = 3;
            // 
            // btnPickProduct
            // 
            this.btnPickProduct.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPickProduct.Location = new System.Drawing.Point(9, 10);
            this.btnPickProduct.Name = "btnPickProduct";
            this.btnPickProduct.Size = new System.Drawing.Size(720, 24);
            this.btnPickProduct.TabIndex = 2;
            this.btnPickProduct.Text = "اختيار منتج";
            this.btnPickProduct.UseVisualStyleBackColor = true;
            this.btnPickProduct.Click += new System.EventHandler(this.btnPickProduct_Click);
            // 
            // txtKey
            // 
            this.txtKey.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtKey.Location = new System.Drawing.Point(729, 10);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(88, 24);
            this.txtKey.TabIndex = 1;
            // 
            // lblKey
            // 
            this.lblKey.AutoSize = true;
            this.lblKey.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblKey.Location = new System.Drawing.Point(817, 10);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(49, 17);
            this.lblKey.TabIndex = 0;
            this.lblKey.Text = "المفتاح";
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.key_code,
            this.product_unit_id,
            this.product_name,
            this.unit_name,
            this.factor,
            this.qty_delta,
            this.note,
            this.updated_at});
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 167);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowHeadersWidth = 51;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(875, 433);
            this.dgv.TabIndex = 1;
            this.dgv.SelectionChanged += new System.EventHandler(this.dgv_SelectionChanged);
            // 
            // key_code
            // 
            this.key_code.DataPropertyName = "key_code";
            this.key_code.HeaderText = "المفتاح";
            this.key_code.MinimumWidth = 6;
            this.key_code.Name = "key_code";
            this.key_code.ReadOnly = true;
            // 
            // product_unit_id
            // 
            this.product_unit_id.DataPropertyName = "product_unit_id";
            this.product_unit_id.HeaderText = "رقم وحدة المنتج";
            this.product_unit_id.MinimumWidth = 6;
            this.product_unit_id.Name = "product_unit_id";
            this.product_unit_id.ReadOnly = true;
            this.product_unit_id.Visible = false;
            // 
            // product_name
            // 
            this.product_name.DataPropertyName = "product_name";
            this.product_name.HeaderText = "اسم المنتج";
            this.product_name.MinimumWidth = 6;
            this.product_name.Name = "product_name";
            this.product_name.ReadOnly = true;
            // 
            // unit_name
            // 
            this.unit_name.DataPropertyName = "unit_name";
            this.unit_name.HeaderText = "الوحدة";
            this.unit_name.MinimumWidth = 6;
            this.unit_name.Name = "unit_name";
            this.unit_name.ReadOnly = true;
            // 
            // factor
            // 
            this.factor.DataPropertyName = "factor";
            this.factor.HeaderText = "المعامل";
            this.factor.MinimumWidth = 6;
            this.factor.Name = "factor";
            this.factor.ReadOnly = true;
            this.factor.Visible = false;
            // 
            // qty_delta
            // 
            this.qty_delta.DataPropertyName = "qty_delta";
            dataGridViewCellStyle1.Format = "N2";
            this.qty_delta.DefaultCellStyle = dataGridViewCellStyle1;
            this.qty_delta.HeaderText = "الكمية";
            this.qty_delta.MinimumWidth = 6;
            this.qty_delta.Name = "qty_delta";
            this.qty_delta.ReadOnly = true;
            // 
            // note
            // 
            this.note.DataPropertyName = "note";
            this.note.HeaderText = "ملاحظة";
            this.note.MinimumWidth = 6;
            this.note.Name = "note";
            this.note.ReadOnly = true;
            this.note.Visible = false;
            // 
            // updated_at
            // 
            this.updated_at.DataPropertyName = "updated_at";
            dataGridViewCellStyle2.Format = "dd/MM/yyyy hh:mm tt";
            this.updated_at.DefaultCellStyle = dataGridViewCellStyle2;
            this.updated_at.HeaderText = "آخر تحديث";
            this.updated_at.MinimumWidth = 6;
            this.updated_at.Name = "updated_at";
            this.updated_at.ReadOnly = true;
            // 
            // PosHotkeysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 600);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.panelTop);
            this.Name = "PosHotkeysForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "إعداد اختصارات POS";
            this.Load += new System.EventHandler(this.PosHotkeysForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridViewTextBoxColumn key_code;
        private System.Windows.Forms.DataGridViewTextBoxColumn product_unit_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn product_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn unit_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn factor;
        private System.Windows.Forms.DataGridViewTextBoxColumn qty_delta;
        private System.Windows.Forms.DataGridViewTextBoxColumn note;
        private System.Windows.Forms.DataGridViewTextBoxColumn updated_at;
    }
}
