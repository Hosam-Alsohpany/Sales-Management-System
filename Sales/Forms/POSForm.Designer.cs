namespace Sales.Forms
{
    partial class POSForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(POSForm));
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.btnPay = new System.Windows.Forms.ToolStripButton();
            this.btnPost = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveLine = new System.Windows.Forms.ToolStripButton();
            this.btnDecLine = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.dgvLines = new System.Windows.Forms.DataGridView();
            this.colProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHiddenProductId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHiddenBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblScan = new System.Windows.Forms.Label();
            this.txtScan = new System.Windows.Forms.TextBox();
            this.panelHeaderRight = new System.Windows.Forms.Panel();
            this.lblAlert = new System.Windows.Forms.Label();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblCashierValue = new System.Windows.Forms.Label();
            this.lblCashier = new System.Windows.Forms.Label();
            this.lstSuggestions = new System.Windows.Forms.ListBox();
            this.panelCustomer = new System.Windows.Forms.Panel();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.lblCustomerValue = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblCustomerBalance = new System.Windows.Forms.Label();
            this.btnPickCustomer = new System.Windows.Forms.Button();
            this.lstRecentOrders = new System.Windows.Forms.ListBox();
            this.tlpSide = new System.Windows.Forms.TableLayoutPanel();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.txtSubtotal = new System.Windows.Forms.TextBox();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.txtDiscountTotal = new System.Windows.Forms.TextBox();
            this.lblNet = new System.Windows.Forms.Label();
            this.txtNetTotal = new System.Windows.Forms.TextBox();
            this.lblPaid = new System.Windows.Forms.Label();
            this.txtPaid = new System.Windows.Forms.TextBox();
            this.lblChange = new System.Windows.Forms.Label();
            this.txtChange = new System.Windows.Forms.TextBox();
            this.toolStripTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).BeginInit();
            this.tlpHeader.SuspendLayout();
            this.panelHeaderRight.SuspendLayout();
            this.panelCustomer.SuspendLayout();
            this.tlpSide.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripTop
            // 
            this.toolStripTop.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPay,
            this.btnPost,
            this.btnRemoveLine,
            this.btnDecLine,
            this.btnClear});
            this.toolStripTop.Location = new System.Drawing.Point(0, 0);
            this.toolStripTop.Name = "toolStripTop";
            this.toolStripTop.Size = new System.Drawing.Size(1361, 27);
            this.toolStripTop.TabIndex = 0;
            this.toolStripTop.Text = "toolStrip1";
            // 
            // btnPay
            // 
            this.btnPay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(57, 24);
            this.btnPay.Text = "F4 دفع/اعتماد";
            // 
            // btnPost
            // 
            this.btnPost.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(79, 24);
            this.btnPost.Visible = false;
            // 
            // btnRemoveLine
            // 
            this.btnRemoveLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemoveLine.Name = "btnRemoveLine";
            this.btnRemoveLine.Size = new System.Drawing.Size(108, 24);
            this.btnRemoveLine.Text = "Del حذف سطر";
            // 
            // btnDecLine
            // 
            this.btnDecLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDecLine.Name = "btnDecLine";
            this.btnDecLine.Size = new System.Drawing.Size(73, 24);
            this.btnDecLine.Text = "F7 إنقاص";
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(108, 24);
            this.btnClear.Text = "Esc مسح السلة";
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 27);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.dgvLines);
            this.splitMain.Panel1.Controls.Add(this.tlpHeader);
            this.splitMain.Panel1.Controls.Add(this.lstSuggestions);
            this.splitMain.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.tlpSide);
            this.splitMain.Panel2.Controls.Add(this.lstRecentOrders);
            this.splitMain.Panel2.Controls.Add(this.panelCustomer);
            this.splitMain.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitMain.Size = new System.Drawing.Size(1361, 607);
            this.splitMain.SplitterDistance = 930;
            this.splitMain.TabIndex = 1;
            // 
            // dgvLines
            // 
            this.dgvLines.AllowUserToAddRows = false;
            this.dgvLines.AllowUserToDeleteRows = false;
            this.dgvLines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProduct,
            this.colUnit,
            this.colQty,
            this.colPrice,
            this.colDisc,
            this.colTotal,
            this.colHiddenProductId,
            this.colHiddenBarcode});
            this.dgvLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLines.Location = new System.Drawing.Point(0, 90);
            this.dgvLines.MultiSelect = false;
            this.dgvLines.Name = "dgvLines";
            this.dgvLines.ReadOnly = true;
            this.dgvLines.RowHeadersVisible = false;
            this.dgvLines.RowHeadersWidth = 51;
            this.dgvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLines.Size = new System.Drawing.Size(930, 517);
            this.dgvLines.TabIndex = 1;
            // 
            // colProduct
            // 
            this.colProduct.DataPropertyName = "ProductName";
            this.colProduct.HeaderText = "المنتج";
            this.colProduct.MinimumWidth = 6;
            this.colProduct.Name = "colProduct";
            this.colProduct.ReadOnly = true;
            // 
            // colUnit
            // 
            this.colUnit.DataPropertyName = "UnitName";
            this.colUnit.HeaderText = "الوحدة";
            this.colUnit.MinimumWidth = 6;
            this.colUnit.Name = "colUnit";
            this.colUnit.ReadOnly = true;
            // 
            // colQty
            // 
            this.colQty.DataPropertyName = "Qty";
            this.colQty.HeaderText = "الكمية";
            this.colQty.MinimumWidth = 6;
            this.colQty.Name = "colQty";
            this.colQty.ReadOnly = true;
            // 
            // colPrice
            // 
            this.colPrice.DataPropertyName = "Price";
            this.colPrice.HeaderText = "السعر";
            this.colPrice.MinimumWidth = 6;
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;
            // 
            // colDisc
            // 
            this.colDisc.DataPropertyName = "Discount";
            this.colDisc.HeaderText = "خصم";
            this.colDisc.MinimumWidth = 6;
            this.colDisc.Name = "colDisc";
            this.colDisc.ReadOnly = true;
            // 
            // colTotal
            // 
            this.colTotal.DataPropertyName = "LineTotal";
            this.colTotal.HeaderText = "الإجمالي";
            this.colTotal.MinimumWidth = 6;
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            // 
            // colHiddenProductId
            // 
            this.colHiddenProductId.DataPropertyName = "ProductId";
            this.colHiddenProductId.HeaderText = "ProductId";
            this.colHiddenProductId.MinimumWidth = 6;
            this.colHiddenProductId.Name = "colHiddenProductId";
            this.colHiddenProductId.ReadOnly = true;
            this.colHiddenProductId.Visible = false;
            // 
            // colHiddenBarcode
            // 
            this.colHiddenBarcode.DataPropertyName = "ScanCode";
            this.colHiddenBarcode.HeaderText = "Barcode";
            this.colHiddenBarcode.MinimumWidth = 6;
            this.colHiddenBarcode.Name = "colHiddenBarcode";
            this.colHiddenBarcode.ReadOnly = true;
            this.colHiddenBarcode.Visible = false;
            // 
            // tlpHeader
            // 
            this.tlpHeader.ColumnCount = 2;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpHeader.Controls.Add(this.lblScan, 0, 0);
            this.tlpHeader.Controls.Add(this.txtScan, 0, 1);
            this.tlpHeader.Controls.Add(this.panelHeaderRight, 1, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 2;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tlpHeader.Size = new System.Drawing.Size(930, 90);
            this.tlpHeader.TabIndex = 0;
            // 
            // lblScan
            // 
            this.lblScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblScan.Location = new System.Drawing.Point(282, 0);
            this.lblScan.Name = "lblScan";
            this.lblScan.Size = new System.Drawing.Size(645, 28);
            this.lblScan.TabIndex = 0;
            this.lblScan.Text = "Barcode";
            this.lblScan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtScan
            // 
            this.txtScan.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtScan.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtScan.Location = new System.Drawing.Point(282, 31);
            this.txtScan.Name = "txtScan";
            this.txtScan.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtScan.Size = new System.Drawing.Size(645, 39);
            this.txtScan.TabIndex = 1;
            // 
            // panelHeaderRight
            // 
            this.panelHeaderRight.Controls.Add(this.lblAlert);
            this.panelHeaderRight.Controls.Add(this.lblStatusValue);
            this.panelHeaderRight.Controls.Add(this.lblStatus);
            this.panelHeaderRight.Controls.Add(this.lblCashierValue);
            this.panelHeaderRight.Controls.Add(this.lblCashier);
            this.panelHeaderRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHeaderRight.Location = new System.Drawing.Point(3, 3);
            this.panelHeaderRight.Name = "panelHeaderRight";
            this.tlpHeader.SetRowSpan(this.panelHeaderRight, 2);
            this.panelHeaderRight.Size = new System.Drawing.Size(273, 84);
            this.panelHeaderRight.TabIndex = 2;
            // 
            // lblAlert
            // 
            this.lblAlert.BackColor = System.Drawing.Color.Silver;
            this.lblAlert.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAlert.Location = new System.Drawing.Point(3, 52);
            this.lblAlert.Name = "lblAlert";
            this.lblAlert.Size = new System.Drawing.Size(267, 29);
            this.lblAlert.TabIndex = 4;
            this.lblAlert.Text = "جاهز";
            this.lblAlert.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Location = new System.Drawing.Point(78, 29);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(54, 20);
            this.lblStatusValue.TabIndex = 3;
            this.lblStatusValue.Text = "DRAFT";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(223, 32);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 20);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "الحالة:";
            // 
            // lblCashierValue
            // 
            this.lblCashierValue.AutoSize = true;
            this.lblCashierValue.Location = new System.Drawing.Point(78, 7);
            this.lblCashierValue.Name = "lblCashierValue";
            this.lblCashierValue.Size = new System.Drawing.Size(15, 20);
            this.lblCashierValue.TabIndex = 1;
            this.lblCashierValue.Text = "-";
            // 
            // lblCashier
            // 
            this.lblCashier.AutoSize = true;
            this.lblCashier.Location = new System.Drawing.Point(213, 7);
            this.lblCashier.Name = "lblCashier";
            this.lblCashier.Size = new System.Drawing.Size(57, 20);
            this.lblCashier.TabIndex = 0;
            this.lblCashier.Text = "الكاشير:";
            // 
            // lstSuggestions
            // 
            this.lstSuggestions.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lstSuggestions.FormattingEnabled = true;
            this.lstSuggestions.IntegralHeight = false;
            this.lstSuggestions.ItemHeight = 28;
            this.lstSuggestions.Location = new System.Drawing.Point(282, 90);
            this.lstSuggestions.Name = "lstSuggestions";
            this.lstSuggestions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lstSuggestions.Size = new System.Drawing.Size(645, 120);
            this.lstSuggestions.TabIndex = 2;
            this.lstSuggestions.Visible = false;
            // 
            // panelCustomer
            // 
            this.panelCustomer.Controls.Add(this.btnPickCustomer);
            this.panelCustomer.Controls.Add(this.lblCustomerBalance);
            this.panelCustomer.Controls.Add(this.lblBalance);
            this.panelCustomer.Controls.Add(this.lblCustomerValue);
            this.panelCustomer.Controls.Add(this.lblCustomer);
            this.panelCustomer.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCustomer.Location = new System.Drawing.Point(0, 0);
            this.panelCustomer.Name = "panelCustomer";
            this.panelCustomer.Size = new System.Drawing.Size(427, 72);
            this.panelCustomer.TabIndex = 2;
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(8, 8);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(48, 20);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "العميل:";
            // 
            // lblCustomerValue
            // 
            this.lblCustomerValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCustomerValue.Location = new System.Drawing.Point(70, 6);
            this.lblCustomerValue.Name = "lblCustomerValue";
            this.lblCustomerValue.Size = new System.Drawing.Size(200, 22);
            this.lblCustomerValue.TabIndex = 1;
            this.lblCustomerValue.Text = "—";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Location = new System.Drawing.Point(8, 38);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(52, 20);
            this.lblBalance.TabIndex = 2;
            this.lblBalance.Text = "الرصيد:";
            // 
            // lblCustomerBalance
            // 
            this.lblCustomerBalance.Location = new System.Drawing.Point(70, 36);
            this.lblCustomerBalance.Name = "lblCustomerBalance";
            this.lblCustomerBalance.Size = new System.Drawing.Size(120, 22);
            this.lblCustomerBalance.TabIndex = 3;
            this.lblCustomerBalance.Text = "0";
            // 
            // btnPickCustomer
            // 
            this.btnPickCustomer.Location = new System.Drawing.Point(280, 20);
            this.btnPickCustomer.Name = "btnPickCustomer";
            this.btnPickCustomer.Size = new System.Drawing.Size(90, 30);
            this.btnPickCustomer.TabIndex = 4;
            this.btnPickCustomer.Text = "F3 اختيار";
            this.btnPickCustomer.UseVisualStyleBackColor = true;
            this.btnPickCustomer.Click += new System.EventHandler(this.btnPickCustomer_Click);
            // 
            // lstRecentOrders
            // 
            this.lstRecentOrders.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lstRecentOrders.FormattingEnabled = true;
            this.lstRecentOrders.IntegralHeight = false;
            this.lstRecentOrders.ItemHeight = 20;
            this.lstRecentOrders.Location = new System.Drawing.Point(0, 512);
            this.lstRecentOrders.Name = "lstRecentOrders";
            this.lstRecentOrders.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lstRecentOrders.Size = new System.Drawing.Size(427, 95);
            this.lstRecentOrders.TabIndex = 3;
            this.lstRecentOrders.DoubleClick += new System.EventHandler(this.lstRecentOrders_DoubleClick);
            // 
            // tlpSide
            // 
            this.tlpSide.ColumnCount = 2;
            this.tlpSide.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.57845F));
            this.tlpSide.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.42155F));
            this.tlpSide.Controls.Add(this.lblSubtotal, 0, 0);
            this.tlpSide.Controls.Add(this.txtSubtotal, 1, 0);
            this.tlpSide.Controls.Add(this.lblDiscount, 0, 1);
            this.tlpSide.Controls.Add(this.txtDiscountTotal, 1, 1);
            this.tlpSide.Controls.Add(this.lblNet, 0, 2);
            this.tlpSide.Controls.Add(this.txtNetTotal, 1, 2);
            this.tlpSide.Controls.Add(this.lblPaid, 0, 3);
            this.tlpSide.Controls.Add(this.txtPaid, 1, 3);
            this.tlpSide.Controls.Add(this.lblChange, 0, 4);
            this.tlpSide.Controls.Add(this.txtChange, 1, 4);
            this.tlpSide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSide.Location = new System.Drawing.Point(0, 0);
            this.tlpSide.Name = "tlpSide";
            this.tlpSide.RowCount = 6;
            this.tlpSide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpSide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpSide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpSide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpSide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpSide.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSide.Size = new System.Drawing.Size(427, 607);
            this.tlpSide.TabIndex = 0;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubtotal.Location = new System.Drawing.Point(262, 0);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(162, 50);
            this.lblSubtotal.TabIndex = 0;
            this.lblSubtotal.Text = "الإجمالي:";
            this.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSubtotal.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtSubtotal.Location = new System.Drawing.Point(3, 3);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.ReadOnly = true;
            this.txtSubtotal.Size = new System.Drawing.Size(253, 39);
            this.txtSubtotal.TabIndex = 1;
            this.txtSubtotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblDiscount
            // 
            this.lblDiscount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDiscount.Location = new System.Drawing.Point(262, 50);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(162, 50);
            this.lblDiscount.TabIndex = 2;
            this.lblDiscount.Text = "خصم:";
            this.lblDiscount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDiscountTotal
            // 
            this.txtDiscountTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDiscountTotal.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtDiscountTotal.Location = new System.Drawing.Point(3, 53);
            this.txtDiscountTotal.Name = "txtDiscountTotal";
            this.txtDiscountTotal.Size = new System.Drawing.Size(253, 39);
            this.txtDiscountTotal.TabIndex = 3;
            this.txtDiscountTotal.Text = "0";
            this.txtDiscountTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNet
            // 
            this.lblNet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNet.Location = new System.Drawing.Point(262, 100);
            this.lblNet.Name = "lblNet";
            this.lblNet.Size = new System.Drawing.Size(162, 50);
            this.lblNet.TabIndex = 4;
            this.lblNet.Text = "الصافي:";
            this.lblNet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNetTotal
            // 
            this.txtNetTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNetTotal.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtNetTotal.Location = new System.Drawing.Point(3, 103);
            this.txtNetTotal.Name = "txtNetTotal";
            this.txtNetTotal.ReadOnly = true;
            this.txtNetTotal.Size = new System.Drawing.Size(253, 39);
            this.txtNetTotal.TabIndex = 5;
            this.txtNetTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPaid
            // 
            this.lblPaid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPaid.Location = new System.Drawing.Point(262, 150);
            this.lblPaid.Name = "lblPaid";
            this.lblPaid.Size = new System.Drawing.Size(162, 50);
            this.lblPaid.TabIndex = 6;
            this.lblPaid.Text = "المدفوع:";
            this.lblPaid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPaid
            // 
            this.txtPaid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPaid.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtPaid.Location = new System.Drawing.Point(3, 153);
            this.txtPaid.Name = "txtPaid";
            this.txtPaid.Size = new System.Drawing.Size(253, 39);
            this.txtPaid.TabIndex = 7;
            this.txtPaid.Text = "0";
            this.txtPaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblChange
            // 
            this.lblChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblChange.Location = new System.Drawing.Point(262, 200);
            this.lblChange.Name = "lblChange";
            this.lblChange.Size = new System.Drawing.Size(162, 50);
            this.lblChange.TabIndex = 8;
            this.lblChange.Text = "الباقي:";
            this.lblChange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtChange
            // 
            this.txtChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtChange.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtChange.Location = new System.Drawing.Point(3, 203);
            this.txtChange.Name = "txtChange";
            this.txtChange.ReadOnly = true;
            this.txtChange.Size = new System.Drawing.Size(253, 39);
            this.txtChange.TabIndex = 9;
            this.txtChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // POSForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1361, 634);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStripTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "POSForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "POS - نقطة بيع";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).EndInit();
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            this.panelHeaderRight.ResumeLayout(false);
            this.panelHeaderRight.PerformLayout();
            this.panelCustomer.ResumeLayout(false);
            this.panelCustomer.PerformLayout();
            this.tlpSide.ResumeLayout(false);
            this.tlpSide.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripButton btnPay;
        private System.Windows.Forms.ToolStripButton btnPost;
        private System.Windows.Forms.ToolStripButton btnRemoveLine;
        private System.Windows.Forms.ToolStripButton btnDecLine;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblScan;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.ListBox lstSuggestions;
        private System.Windows.Forms.Panel panelHeaderRight;
        private System.Windows.Forms.Label lblCashier;
        private System.Windows.Forms.Label lblCashierValue;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Label lblAlert;
        private System.Windows.Forms.DataGridView dgvLines;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisc;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHiddenProductId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHiddenBarcode;
        private System.Windows.Forms.TableLayoutPanel tlpSide;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.TextBox txtSubtotal;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.TextBox txtDiscountTotal;
        private System.Windows.Forms.Label lblNet;
        private System.Windows.Forms.TextBox txtNetTotal;
        private System.Windows.Forms.Label lblPaid;
        private System.Windows.Forms.TextBox txtPaid;
        private System.Windows.Forms.Label lblChange;
        private System.Windows.Forms.TextBox txtChange;
        private System.Windows.Forms.Panel panelCustomer;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.Label lblCustomerValue;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Label lblCustomerBalance;
        private System.Windows.Forms.Button btnPickCustomer;
        private System.Windows.Forms.ListBox lstRecentOrders;
    }
}
