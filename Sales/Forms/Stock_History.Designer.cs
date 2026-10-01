namespace Sales.Forms
{
    partial class Stock_History
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQtyChange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChangeDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabBatches = new System.Windows.Forms.TabPage();
            this.splitBatches = new System.Windows.Forms.SplitContainer();
            this.dgvBatches = new System.Windows.Forms.DataGridView();
            this.colBatchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.created_at = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.created_by = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reference = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.items_count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_qty_change = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvBatchItems = new System.Windows.Forms.DataGridView();
            this.colBatchItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.batch_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.product_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.product_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty_before = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty_after = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qty_change = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabMain.SuspendLayout();
            this.tabHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.tabBatches.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitBatches)).BeginInit();
            this.splitBatches.Panel1.SuspendLayout();
            this.splitBatches.Panel2.SuspendLayout();
            this.splitBatches.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatches)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatchItems)).BeginInit();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabHistory);
            this.tabMain.Controls.Add(this.tabBatches);
            this.tabMain.Location = new System.Drawing.Point(14, 58);
            this.tabMain.Margin = new System.Windows.Forms.Padding(4);
            this.tabMain.Name = "tabMain";
            this.tabMain.RightToLeftLayout = true;
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1176, 404);
            this.tabMain.TabIndex = 0;
            // 
            // tabHistory
            // 
            this.tabHistory.Controls.Add(this.dgvHistory);
            this.tabHistory.Location = new System.Drawing.Point(4, 30);
            this.tabHistory.Margin = new System.Windows.Forms.Padding(4);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(6);
            this.tabHistory.Size = new System.Drawing.Size(1168, 370);
            this.tabHistory.TabIndex = 0;
            this.tabHistory.Text = "السجل";
            this.tabHistory.UseVisualStyleBackColor = true;
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colProduct,
            this.colQtyChange,
            this.colReason,
            this.colChangeDate,
            this.colUserName});
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.EnableHeadersVisualStyles = false;
            this.dgvHistory.Location = new System.Drawing.Point(6, 6);
            this.dgvHistory.Margin = new System.Windows.Forms.Padding(4);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersWidth = 51;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(1156, 358);
            this.dgvHistory.TabIndex = 0;
            // 
            // colId
            // 
            this.colId.DataPropertyName = "Id";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colId.DefaultCellStyle = dataGridViewCellStyle1;
            this.colId.HeaderText = "رقم الحركة";
            this.colId.MinimumWidth = 6;
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            // 
            // colProduct
            // 
            this.colProduct.DataPropertyName = "Product";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colProduct.DefaultCellStyle = dataGridViewCellStyle2;
            this.colProduct.HeaderText = "اسم المنتج";
            this.colProduct.MinimumWidth = 6;
            this.colProduct.Name = "colProduct";
            this.colProduct.ReadOnly = true;
            // 
            // colQtyChange
            // 
            this.colQtyChange.DataPropertyName = "QtyChange";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.DarkBlue;
            dataGridViewCellStyle3.Format = "N0";
            this.colQtyChange.DefaultCellStyle = dataGridViewCellStyle3;
            this.colQtyChange.HeaderText = "الكمية";
            this.colQtyChange.MinimumWidth = 6;
            this.colQtyChange.Name = "colQtyChange";
            this.colQtyChange.ReadOnly = true;
            // 
            // colReason
            // 
            this.colReason.DataPropertyName = "Reason";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colReason.DefaultCellStyle = dataGridViewCellStyle4;
            this.colReason.HeaderText = "السبب / البيان";
            this.colReason.MinimumWidth = 6;
            this.colReason.Name = "colReason";
            this.colReason.ReadOnly = true;
            // 
            // colChangeDate
            // 
            this.colChangeDate.DataPropertyName = "ChangeDate";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colChangeDate.DefaultCellStyle = dataGridViewCellStyle5;
            this.colChangeDate.HeaderText = "التاريخ";
            this.colChangeDate.MinimumWidth = 6;
            this.colChangeDate.Name = "colChangeDate";
            this.colChangeDate.ReadOnly = true;
            // 
            // colUserName
            // 
            this.colUserName.DataPropertyName = "UserName";
            this.colUserName.HeaderText = "المستخدم";
            this.colUserName.MinimumWidth = 6;
            this.colUserName.Name = "colUserName";
            this.colUserName.ReadOnly = true;
            // 
            // tabBatches
            // 
            this.tabBatches.Controls.Add(this.splitBatches);
            this.tabBatches.Location = new System.Drawing.Point(4, 30);
            this.tabBatches.Margin = new System.Windows.Forms.Padding(4);
            this.tabBatches.Name = "tabBatches";
            this.tabBatches.Padding = new System.Windows.Forms.Padding(6);
            this.tabBatches.Size = new System.Drawing.Size(1168, 370);
            this.tabBatches.TabIndex = 1;
            this.tabBatches.Text = "عمليات جماعية";
            this.tabBatches.UseVisualStyleBackColor = true;
            // 
            // splitBatches
            // 
            this.splitBatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitBatches.Location = new System.Drawing.Point(6, 6);
            this.splitBatches.Margin = new System.Windows.Forms.Padding(4);
            this.splitBatches.Name = "splitBatches";
            this.splitBatches.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitBatches.Panel1
            // 
            this.splitBatches.Panel1.Controls.Add(this.dgvBatches);
            this.splitBatches.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // splitBatches.Panel2
            // 
            this.splitBatches.Panel2.Controls.Add(this.dgvBatchItems);
            this.splitBatches.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitBatches.Size = new System.Drawing.Size(1156, 358);
            this.splitBatches.SplitterDistance = 167;
            this.splitBatches.TabIndex = 0;
            // 
            // dgvBatches
            // 
            this.dgvBatches.AllowUserToAddRows = false;
            this.dgvBatches.AllowUserToDeleteRows = false;
            this.dgvBatches.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBatches.BackgroundColor = System.Drawing.Color.White;
            this.dgvBatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBatches.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBatchId,
            this.created_at,
            this.created_by,
            this.reason,
            this.reference,
            this.note,
            this.items_count,
            this.total_qty_change});
            this.dgvBatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBatches.EnableHeadersVisualStyles = false;
            this.dgvBatches.Location = new System.Drawing.Point(0, 0);
            this.dgvBatches.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBatches.MultiSelect = false;
            this.dgvBatches.Name = "dgvBatches";
            this.dgvBatches.ReadOnly = true;
            this.dgvBatches.RowHeadersVisible = false;
            this.dgvBatches.RowHeadersWidth = 51;
            this.dgvBatches.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBatches.Size = new System.Drawing.Size(1156, 167);
            this.dgvBatches.TabIndex = 0;
            // 
            // colBatchId
            // 
            this.colBatchId.DataPropertyName = "id";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colBatchId.DefaultCellStyle = dataGridViewCellStyle6;
            this.colBatchId.HeaderText = "رقم العملية";
            this.colBatchId.MinimumWidth = 6;
            this.colBatchId.Name = "colBatchId";
            this.colBatchId.ReadOnly = true;
            // 
            // created_at
            // 
            this.created_at.DataPropertyName = "created_at";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.created_at.DefaultCellStyle = dataGridViewCellStyle7;
            this.created_at.HeaderText = "التاريخ";
            this.created_at.MinimumWidth = 6;
            this.created_at.Name = "created_at";
            this.created_at.ReadOnly = true;
            // 
            // created_by
            // 
            this.created_by.DataPropertyName = "created_by";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.created_by.DefaultCellStyle = dataGridViewCellStyle8;
            this.created_by.HeaderText = "المستخدم";
            this.created_by.MinimumWidth = 6;
            this.created_by.Name = "created_by";
            this.created_by.ReadOnly = true;
            // 
            // reason
            // 
            this.reason.DataPropertyName = "reason";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.reason.DefaultCellStyle = dataGridViewCellStyle9;
            this.reason.HeaderText = "السبب";
            this.reason.MinimumWidth = 6;
            this.reason.Name = "reason";
            this.reason.ReadOnly = true;
            // 
            // reference
            // 
            this.reference.DataPropertyName = "reference";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.reference.DefaultCellStyle = dataGridViewCellStyle10;
            this.reference.HeaderText = "المرجع";
            this.reference.MinimumWidth = 6;
            this.reference.Name = "reference";
            this.reference.ReadOnly = true;
            // 
            // note
            // 
            this.note.DataPropertyName = "note";
            this.note.HeaderText = "ملاحظات";
            this.note.MinimumWidth = 6;
            this.note.Name = "note";
            this.note.ReadOnly = true;
            // 
            // items_count
            // 
            this.items_count.DataPropertyName = "items_count";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.Format = "N0";
            this.items_count.DefaultCellStyle = dataGridViewCellStyle11;
            this.items_count.HeaderText = "عدد الأصناف";
            this.items_count.MinimumWidth = 6;
            this.items_count.Name = "items_count";
            this.items_count.ReadOnly = true;
            // 
            // total_qty_change
            // 
            this.total_qty_change.DataPropertyName = "total_qty_change";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.Format = "N0";
            this.total_qty_change.DefaultCellStyle = dataGridViewCellStyle12;
            this.total_qty_change.HeaderText = "إجمالي التغير";
            this.total_qty_change.MinimumWidth = 6;
            this.total_qty_change.Name = "total_qty_change";
            this.total_qty_change.ReadOnly = true;
            // 
            // dgvBatchItems
            // 
            this.dgvBatchItems.AllowUserToAddRows = false;
            this.dgvBatchItems.AllowUserToDeleteRows = false;
            this.dgvBatchItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBatchItems.BackgroundColor = System.Drawing.Color.White;
            this.dgvBatchItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBatchItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBatchItemId,
            this.batch_id,
            this.product_id,
            this.product_name,
            this.qty_before,
            this.qty_after,
            this.qty_change});
            this.dgvBatchItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBatchItems.EnableHeadersVisualStyles = false;
            this.dgvBatchItems.Location = new System.Drawing.Point(0, 0);
            this.dgvBatchItems.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBatchItems.MultiSelect = false;
            this.dgvBatchItems.Name = "dgvBatchItems";
            this.dgvBatchItems.ReadOnly = true;
            this.dgvBatchItems.RowHeadersVisible = false;
            this.dgvBatchItems.RowHeadersWidth = 51;
            this.dgvBatchItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBatchItems.Size = new System.Drawing.Size(1156, 187);
            this.dgvBatchItems.TabIndex = 0;
            // 
            // colBatchItemId
            // 
            this.colBatchItemId.DataPropertyName = "id";
            this.colBatchItemId.HeaderText = "رقم السطر";
            this.colBatchItemId.MinimumWidth = 6;
            this.colBatchItemId.Name = "colBatchItemId";
            this.colBatchItemId.ReadOnly = true;
            this.colBatchItemId.Visible = false;
            // 
            // batch_id
            // 
            this.batch_id.DataPropertyName = "batch_id";
            this.batch_id.HeaderText = "رقم العملية";
            this.batch_id.MinimumWidth = 6;
            this.batch_id.Name = "batch_id";
            this.batch_id.ReadOnly = true;
            this.batch_id.Visible = false;
            // 
            // product_id
            // 
            this.product_id.DataPropertyName = "product_id";
            this.product_id.HeaderText = "رقم المنتج";
            this.product_id.MinimumWidth = 6;
            this.product_id.Name = "product_id";
            this.product_id.ReadOnly = true;
            this.product_id.Visible = false;
            // 
            // product_name
            // 
            this.product_name.DataPropertyName = "product_name";
            this.product_name.HeaderText = "اسم المنتج";
            this.product_name.MinimumWidth = 6;
            this.product_name.Name = "product_name";
            this.product_name.ReadOnly = true;
            // 
            // qty_before
            // 
            this.qty_before.DataPropertyName = "qty_before";
            this.qty_before.HeaderText = "الكمية قبل";
            this.qty_before.MinimumWidth = 6;
            this.qty_before.Name = "qty_before";
            this.qty_before.ReadOnly = true;
            // 
            // qty_after
            // 
            this.qty_after.DataPropertyName = "qty_after";
            this.qty_after.HeaderText = "الكمية بعد";
            this.qty_after.MinimumWidth = 6;
            this.qty_after.Name = "qty_after";
            this.qty_after.ReadOnly = true;
            // 
            // qty_change
            // 
            this.qty_change.DataPropertyName = "qty_change";
            this.qty_change.HeaderText = "التغير";
            this.qty_change.MinimumWidth = 6;
            this.qty_change.Name = "qty_change";
            this.qty_change.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(502, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "حركة المخزون";
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Location = new System.Drawing.Point(517, 469);
            this.btnExportCsv.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(117, 41);
            this.btnExportCsv.TabIndex = 3;
            this.btnExportCsv.Text = "تصدير CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.Location = new System.Drawing.Point(642, 469);
            this.btnExportPdf.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(117, 41);
            this.btnExportPdf.TabIndex = 4;
            this.btnExportPdf.Text = "تصدير PDF";
            this.btnExportPdf.UseVisualStyleBackColor = true;
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(767, 469);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(117, 41);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(396, 469);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(117, 41);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "خروج";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Stock_History
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1193, 531);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnExportPdf);
            this.Controls.Add(this.btnExportCsv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabMain);
            this.Font = new System.Drawing.Font("Tahoma", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Stock_History";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "حركة المخزون";
            this.Load += new System.EventHandler(this.Stock_History_Load);
            this.tabMain.ResumeLayout(false);
            this.tabHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.tabBatches.ResumeLayout(false);
            this.splitBatches.Panel1.ResumeLayout(false);
            this.splitBatches.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitBatches)).EndInit();
            this.splitBatches.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatches)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatchItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabHistory;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQtyChange;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReason;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChangeDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserName;
        private System.Windows.Forms.TabPage tabBatches;
        private System.Windows.Forms.SplitContainer splitBatches;
        private System.Windows.Forms.DataGridView dgvBatches;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBatchId;
        private System.Windows.Forms.DataGridView dgvBatchItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBatchItemId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn created_at;
        private System.Windows.Forms.DataGridViewTextBoxColumn created_by;
        private System.Windows.Forms.DataGridViewTextBoxColumn reason;
        private System.Windows.Forms.DataGridViewTextBoxColumn reference;
        private System.Windows.Forms.DataGridViewTextBoxColumn note;
        private System.Windows.Forms.DataGridViewTextBoxColumn items_count;
        private System.Windows.Forms.DataGridViewTextBoxColumn total_qty_change;
        private System.Windows.Forms.DataGridViewTextBoxColumn batch_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn product_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn product_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn qty_before;
        private System.Windows.Forms.DataGridViewTextBoxColumn qty_after;
        private System.Windows.Forms.DataGridViewTextBoxColumn qty_change;
    }
}
