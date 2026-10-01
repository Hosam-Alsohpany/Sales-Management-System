namespace Sales.Forms
{
    partial class ProductUnitsForm
    {
        private System.ComponentModel.IContainer components = null;

        // ─── ToolStrip ─────────────────────────────────────────────────
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton tsBtnPickProduct;
        private System.Windows.Forms.ToolStripSeparator tsSep1;
        private System.Windows.Forms.ToolStripButton tsBtnAddUnit;
        private System.Windows.Forms.ToolStripButton tsBtnChangeUnit;
        private System.Windows.Forms.ToolStripButton tsBtnDelete;
        private System.Windows.Forms.ToolStripSeparator tsSep2;
        private System.Windows.Forms.ToolStripButton tsBtnSave;
        private System.Windows.Forms.ToolStripButton tsBtnReload;
        private System.Windows.Forms.ToolStripSeparator tsSep3;
        private System.Windows.Forms.ToolStripButton tsBtnExportCsv;
        private System.Windows.Forms.ToolStripButton tsBtnExportPdf;
        private System.Windows.Forms.ToolStripButton tsBtnPrint;
        private System.Windows.Forms.ToolStripLabel tsLblLock;
        private System.Windows.Forms.ToolStripLabel tsLblDirty;

        // ─── Product Info Group ────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpProductInfo;
        private System.Windows.Forms.PictureBox picProduct;
        private System.Windows.Forms.Label lblIdTitle;
        private System.Windows.Forms.TextBox txtProductId;
        private System.Windows.Forms.Label lblNameTitle;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label lblProductCode;
        private System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Label lblProductBarcode;
        private System.Windows.Forms.TextBox txtProductBarcode;
        private System.Windows.Forms.Label lblProductBaseUnit;
        private System.Windows.Forms.TextBox txtProductBaseUnit;
        private System.Windows.Forms.Label lblProductBasePrice;
        private System.Windows.Forms.TextBox txtProductBasePrice;

        // ─── Search / Barcode Panel ────────────────────────────────────
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Label lblBarcodeFor;
        private System.Windows.Forms.TextBox txtBarcodeInput;
        private System.Windows.Forms.CheckBox chkBarcodeDefault;
        private System.Windows.Forms.Button btnAddBarcode;
        private System.Windows.Forms.Button btnUnitsMaster;

        // ─── SplitContainer ───────────────────────────────────────────
        private System.Windows.Forms.SplitContainer splitMain;

        // ─── DataGridView ─────────────────────────────────────────────
        private System.Windows.Forms.DataGridView dgvProductUnits;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevelNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPackSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFactor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefaultBarcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSellPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCostPrice;
        // Hidden columns (used for data binding only)
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colParentId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colParentUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBarcodeType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisplayOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHierarchyLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIsBaseUnit;

        // ─── Conversion Preview Panel ──────────────────────────────────
        private System.Windows.Forms.Panel panelConversionPreview;
        private System.Windows.Forms.Label lblCurrentUnit;
        private System.Windows.Forms.Label lblConversionTitle;
        private System.Windows.Forms.TextBox txtConversions;
        private System.Windows.Forms.Label lblFinalConversion;
        private System.Windows.Forms.TextBox txtFinalConversion;
        private System.Windows.Forms.Label lblPricesTitle;
        private System.Windows.Forms.Label lblSellTitle;
        private System.Windows.Forms.TextBox txtSellPriceUnit;
        private System.Windows.Forms.Label lblCostTitle;
        private System.Windows.Forms.TextBox txtCostPriceUnit;

        // ─── Legacy hidden controls (kept for .cs references) ─────────
        private System.Windows.Forms.Panel panelProductInfo;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnAddUnit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRecalculate;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Label lblTransactionLock;
        private System.Windows.Forms.Button btnExportCsv;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tsBtnPickProduct = new System.Windows.Forms.ToolStripButton();
            this.tsSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnAddUnit = new System.Windows.Forms.ToolStripButton();
            this.tsBtnChangeUnit = new System.Windows.Forms.ToolStripButton();
            this.tsBtnDelete = new System.Windows.Forms.ToolStripButton();
            this.tsSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnSave = new System.Windows.Forms.ToolStripButton();
            this.tsBtnReload = new System.Windows.Forms.ToolStripButton();
            this.tsSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtnExportCsv = new System.Windows.Forms.ToolStripButton();
            this.tsBtnExportPdf = new System.Windows.Forms.ToolStripButton();
            this.tsBtnPrint = new System.Windows.Forms.ToolStripButton();
            this.tsLblLock = new System.Windows.Forms.ToolStripLabel();
            this.tsLblDirty = new System.Windows.Forms.ToolStripLabel();
            this.grpProductInfo = new System.Windows.Forms.GroupBox();
            this.picProduct = new System.Windows.Forms.PictureBox();
            this.lblIdTitle = new System.Windows.Forms.Label();
            this.txtProductId = new System.Windows.Forms.TextBox();
            this.lblNameTitle = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.lblProductCode = new System.Windows.Forms.Label();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.lblProductBarcode = new System.Windows.Forms.Label();
            this.txtProductBarcode = new System.Windows.Forms.TextBox();
            this.lblProductBaseUnit = new System.Windows.Forms.Label();
            this.txtProductBaseUnit = new System.Windows.Forms.TextBox();
            this.lblProductBasePrice = new System.Windows.Forms.Label();
            this.txtProductBasePrice = new System.Windows.Forms.TextBox();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.lblBarcodeFor = new System.Windows.Forms.Label();
            this.txtBarcodeInput = new System.Windows.Forms.TextBox();
            this.chkBarcodeDefault = new System.Windows.Forms.CheckBox();
            this.btnAddBarcode = new System.Windows.Forms.Button();
            this.btnUnitsMaster = new System.Windows.Forms.Button();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.dgvProductUnits = new System.Windows.Forms.DataGridView();
            this.colLevelNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPackSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFactor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDefaultBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSellPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCostPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colParentId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colParentUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBarcodeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisplayOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHierarchyLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsBaseUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelConversionPreview = new System.Windows.Forms.Panel();
            this.lblCurrentUnit = new System.Windows.Forms.Label();
            this.lblConversionTitle = new System.Windows.Forms.Label();
            this.txtConversions = new System.Windows.Forms.TextBox();
            this.lblFinalConversion = new System.Windows.Forms.Label();
            this.txtFinalConversion = new System.Windows.Forms.TextBox();
            this.lblPricesTitle = new System.Windows.Forms.Label();
            this.lblSellTitle = new System.Windows.Forms.Label();
            this.txtSellPriceUnit = new System.Windows.Forms.TextBox();
            this.lblCostTitle = new System.Windows.Forms.Label();
            this.txtCostPriceUnit = new System.Windows.Forms.TextBox();
            this.panelProductInfo = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnAddUnit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRecalculate = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.lblTransactionLock = new System.Windows.Forms.Label();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.toolStripMain.SuspendLayout();
            this.grpProductInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picProduct)).BeginInit();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductUnits)).BeginInit();
            this.panelConversionPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnPickProduct,
            this.tsSep1,
            this.tsBtnAddUnit,
            this.tsBtnChangeUnit,
            this.tsBtnDelete,
            this.tsSep2,
            this.tsBtnSave,
            this.tsBtnReload,
            this.tsSep3,
            this.tsBtnExportCsv,
            this.tsBtnExportPdf,
            this.tsBtnPrint,
            this.tsLblLock,
            this.tsLblDirty});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStripMain.Size = new System.Drawing.Size(1201, 27);
            this.toolStripMain.TabIndex = 0;
            // 
            // tsBtnPickProduct
            // 
            this.tsBtnPickProduct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnPickProduct.Name = "tsBtnPickProduct";
            this.tsBtnPickProduct.Size = new System.Drawing.Size(78, 24);
            this.tsBtnPickProduct.Text = "اختيار منتج";
            this.tsBtnPickProduct.Click += new System.EventHandler(this.btnPickProduct_Click);
            // 
            // tsSep1
            // 
            this.tsSep1.Name = "tsSep1";
            this.tsSep1.Size = new System.Drawing.Size(6, 27);
            // 
            // tsBtnAddUnit
            // 
            this.tsBtnAddUnit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnAddUnit.Name = "tsBtnAddUnit";
            this.tsBtnAddUnit.Size = new System.Drawing.Size(89, 24);
            this.tsBtnAddUnit.Text = "اضافة وحدة";
            this.tsBtnAddUnit.Click += new System.EventHandler(this.btnAddUnit_Click);
            // 
            // tsBtnChangeUnit
            // 
            this.tsBtnChangeUnit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnChangeUnit.Name = "tsBtnChangeUnit";
            this.tsBtnChangeUnit.Size = new System.Drawing.Size(85, 24);
            this.tsBtnChangeUnit.Text = "تغيير الوحدة";
            this.tsBtnChangeUnit.ForeColor = System.Drawing.Color.DarkBlue;
            this.tsBtnChangeUnit.Click += new System.EventHandler(this.btnChangeUnit_Click);
            // 
            // tsBtnDelete
            // 
            this.tsBtnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnDelete.Name = "tsBtnDelete";
            this.tsBtnDelete.Size = new System.Drawing.Size(45, 24);
            this.tsBtnDelete.Text = "حذف";
            this.tsBtnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tsSep2
            // 
            this.tsSep2.Name = "tsSep2";
            this.tsSep2.Size = new System.Drawing.Size(6, 27);
            // 
            // tsBtnSave
            // 
            this.tsBtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnSave.Enabled = false;
            this.tsBtnSave.Name = "tsBtnSave";
            this.tsBtnSave.Size = new System.Drawing.Size(43, 24);
            this.tsBtnSave.Text = "حفظ";
            this.tsBtnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tsBtnReload
            // 
            this.tsBtnReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnReload.Name = "tsBtnReload";
            this.tsBtnReload.Size = new System.Drawing.Size(54, 24);
            this.tsBtnReload.Text = "تحديث";
            this.tsBtnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // tsSep3
            // 
            this.tsSep3.Name = "tsSep3";
            this.tsSep3.Size = new System.Drawing.Size(6, 27);
            // 
            // tsBtnExportCsv
            // 
            this.tsBtnExportCsv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnExportCsv.Name = "tsBtnExportCsv";
            this.tsBtnExportCsv.Size = new System.Drawing.Size(82, 24);
            this.tsBtnExportCsv.Text = "تصدير CSV";
            this.tsBtnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            this.tsBtnExportPdf.Text = "تصدير PDF"; this.tsBtnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            this.tsBtnPrint.Text = "طباعة"; this.tsBtnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // tsLblLock
            // 
            this.tsLblLock.ForeColor = System.Drawing.Color.OrangeRed;
            this.tsLblLock.Name = "tsLblLock";
            this.tsLblLock.Size = new System.Drawing.Size(323, 24);
            this.tsLblLock.Text = "يوجد حركات - قيم القديمة محفوظة في snapshots";
            this.tsLblLock.Visible = false;
            // 
            // tsLblDirty
            // 
            this.tsLblDirty.ForeColor = System.Drawing.Color.DarkOrange;
            this.tsLblDirty.Name = "tsLblDirty";
            this.tsLblDirty.Size = new System.Drawing.Size(186, 24);
            this.tsLblDirty.Text = "● يوجد تغييرات غير محفوظة";
            this.tsLblDirty.Visible = false;
            // 
            // grpProductInfo
            // 
            this.grpProductInfo.Controls.Add(this.picProduct);
            this.grpProductInfo.Controls.Add(this.lblIdTitle);
            this.grpProductInfo.Controls.Add(this.txtProductId);
            this.grpProductInfo.Controls.Add(this.lblNameTitle);
            this.grpProductInfo.Controls.Add(this.txtProductName);
            this.grpProductInfo.Controls.Add(this.lblProductCode);
            this.grpProductInfo.Controls.Add(this.txtProductCode);
            this.grpProductInfo.Controls.Add(this.lblProductBarcode);
            this.grpProductInfo.Controls.Add(this.txtProductBarcode);
            this.grpProductInfo.Controls.Add(this.lblProductBaseUnit);
            this.grpProductInfo.Controls.Add(this.txtProductBaseUnit);
            this.grpProductInfo.Controls.Add(this.lblProductBasePrice);
            this.grpProductInfo.Controls.Add(this.txtProductBasePrice);
            this.grpProductInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpProductInfo.Location = new System.Drawing.Point(0, 27);
            this.grpProductInfo.Name = "grpProductInfo";
            this.grpProductInfo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.grpProductInfo.Size = new System.Drawing.Size(1201, 100);
            this.grpProductInfo.TabIndex = 1;
            this.grpProductInfo.TabStop = false;
            this.grpProductInfo.Text = "بيانات المنتج المحدد";
            // 
            // picProduct
            // 
            this.picProduct.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picProduct.Location = new System.Drawing.Point(10, 18);
            this.picProduct.Name = "picProduct";
            this.picProduct.Size = new System.Drawing.Size(72, 72);
            this.picProduct.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picProduct.TabIndex = 0;
            this.picProduct.TabStop = false;
            // 
            // lblIdTitle
            // 
            this.lblIdTitle.AutoSize = true;
            this.lblIdTitle.Location = new System.Drawing.Point(1040, 24);
            this.lblIdTitle.Name = "lblIdTitle";
            this.lblIdTitle.Size = new System.Drawing.Size(62, 21);
            this.lblIdTitle.TabIndex = 0;
            this.lblIdTitle.Text = "المعرف:";
            // 
            // txtProductId
            // 
            this.txtProductId.Location = new System.Drawing.Point(910, 21);
            this.txtProductId.Name = "txtProductId";
            this.txtProductId.ReadOnly = true;
            this.txtProductId.Size = new System.Drawing.Size(120, 29);
            this.txtProductId.TabIndex = 1;
            this.txtProductId.TabStop = false;
            // 
            // lblNameTitle
            // 
            this.lblNameTitle.AutoSize = true;
            this.lblNameTitle.Location = new System.Drawing.Point(813, 26);
            this.lblNameTitle.Name = "lblNameTitle";
            this.lblNameTitle.Size = new System.Drawing.Size(80, 21);
            this.lblNameTitle.TabIndex = 2;
            this.lblNameTitle.Text = "اسم المنتج:";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(597, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(210, 29);
            this.txtProductName.TabIndex = 3;
            this.txtProductName.TabStop = false;
            // 
            // lblProductCode
            // 
            this.lblProductCode.AutoSize = true;
            this.lblProductCode.Location = new System.Drawing.Point(515, 25);
            this.lblProductCode.Name = "lblProductCode";
            this.lblProductCode.Size = new System.Drawing.Size(42, 21);
            this.lblProductCode.TabIndex = 4;
            this.lblProductCode.Text = "SKU:";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(370, 18);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.ReadOnly = true;
            this.txtProductCode.Size = new System.Drawing.Size(130, 29);
            this.txtProductCode.TabIndex = 5;
            this.txtProductCode.TabStop = false;
            // 
            // lblProductBarcode
            // 
            this.lblProductBarcode.AutoSize = true;
            this.lblProductBarcode.Location = new System.Drawing.Point(1040, 58);
            this.lblProductBarcode.Name = "lblProductBarcode";
            this.lblProductBarcode.Size = new System.Drawing.Size(61, 21);
            this.lblProductBarcode.TabIndex = 6;
            this.lblProductBarcode.Text = "الباركود:";
            // 
            // txtProductBarcode
            // 
            this.txtProductBarcode.Location = new System.Drawing.Point(870, 55);
            this.txtProductBarcode.Name = "txtProductBarcode";
            this.txtProductBarcode.ReadOnly = true;
            this.txtProductBarcode.Size = new System.Drawing.Size(160, 29);
            this.txtProductBarcode.TabIndex = 7;
            this.txtProductBarcode.TabStop = false;
            // 
            // lblProductBaseUnit
            // 
            this.lblProductBaseUnit.AutoSize = true;
            this.lblProductBaseUnit.Location = new System.Drawing.Point(683, 66);
            this.lblProductBaseUnit.Name = "lblProductBaseUnit";
            this.lblProductBaseUnit.Size = new System.Drawing.Size(115, 21);
            this.lblProductBaseUnit.TabIndex = 8;
            this.lblProductBaseUnit.Text = "الوحدة الأساسية:";
            // 
            // txtProductBaseUnit
            // 
            this.txtProductBaseUnit.Location = new System.Drawing.Point(547, 61);
            this.txtProductBaseUnit.Name = "txtProductBaseUnit";
            this.txtProductBaseUnit.ReadOnly = true;
            this.txtProductBaseUnit.Size = new System.Drawing.Size(130, 29);
            this.txtProductBaseUnit.TabIndex = 9;
            this.txtProductBaseUnit.TabStop = false;
            // 
            // lblProductBasePrice
            // 
            this.lblProductBasePrice.AutoSize = true;
            this.lblProductBasePrice.Location = new System.Drawing.Point(442, 61);
            this.lblProductBasePrice.Name = "lblProductBasePrice";
            this.lblProductBasePrice.Size = new System.Drawing.Size(99, 21);
            this.lblProductBasePrice.TabIndex = 10;
            this.lblProductBasePrice.Text = "سعر الأساسي:";
            // 
            // txtProductBasePrice
            // 
            this.txtProductBasePrice.Location = new System.Drawing.Point(306, 61);
            this.txtProductBasePrice.Name = "txtProductBasePrice";
            this.txtProductBasePrice.ReadOnly = true;
            this.txtProductBasePrice.Size = new System.Drawing.Size(130, 29);
            this.txtProductBasePrice.TabIndex = 11;
            this.txtProductBasePrice.TabStop = false;
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.lblSearch);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.btnClearSearch);
            this.panelSearch.Controls.Add(this.lblBarcodeFor);
            this.panelSearch.Controls.Add(this.txtBarcodeInput);
            this.panelSearch.Controls.Add(this.chkBarcodeDefault);
            this.panelSearch.Controls.Add(this.btnAddBarcode);
            this.panelSearch.Controls.Add(this.btnUnitsMaster);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 127);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panelSearch.Size = new System.Drawing.Size(1201, 42);
            this.panelSearch.TabIndex = 2;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(1155, 11);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(42, 21);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "بحث:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(961, 9);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(188, 29);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSearch.Location = new System.Drawing.Point(886, 5);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(55, 34);
            this.btnClearSearch.TabIndex = 2;
            this.btnClearSearch.Text = "مسح";
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // lblBarcodeFor
            // 
            this.lblBarcodeFor.AutoSize = true;
            this.lblBarcodeFor.Location = new System.Drawing.Point(727, 9);
            this.lblBarcodeFor.Name = "lblBarcodeFor";
            this.lblBarcodeFor.Size = new System.Drawing.Size(153, 21);
            this.lblBarcodeFor.TabIndex = 3;
            this.lblBarcodeFor.Text = "باركود الوحدة المحددة:";
            this.lblBarcodeFor.Click += new System.EventHandler(this.lblBarcodeFor_Click);
            // 
            // txtBarcodeInput
            // 
            this.txtBarcodeInput.Location = new System.Drawing.Point(506, 7);
            this.txtBarcodeInput.Name = "txtBarcodeInput";
            this.txtBarcodeInput.Size = new System.Drawing.Size(215, 29);
            this.txtBarcodeInput.TabIndex = 4;
            // 
            // chkBarcodeDefault
            // 
            this.chkBarcodeDefault.AutoSize = true;
            this.chkBarcodeDefault.Location = new System.Drawing.Point(415, 10);
            this.chkBarcodeDefault.Name = "chkBarcodeDefault";
            this.chkBarcodeDefault.Size = new System.Drawing.Size(85, 25);
            this.chkBarcodeDefault.TabIndex = 5;
            this.chkBarcodeDefault.Text = "افتراضي";
            // 
            // btnAddBarcode
            // 
            this.btnAddBarcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddBarcode.Location = new System.Drawing.Point(305, 6);
            this.btnAddBarcode.Name = "btnAddBarcode";
            this.btnAddBarcode.Size = new System.Drawing.Size(105, 36);
            this.btnAddBarcode.TabIndex = 6;
            this.btnAddBarcode.Text = "اضافة باركود";
            this.btnAddBarcode.Click += new System.EventHandler(this.BtnAddBarcode_Click);
            // 
            // btnUnitsMaster
            // 
            this.btnUnitsMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnitsMaster.Location = new System.Drawing.Point(120, 6);
            this.btnUnitsMaster.Name = "btnUnitsMaster";
            this.btnUnitsMaster.Size = new System.Drawing.Size(180, 36);
            this.btnUnitsMaster.TabIndex = 7;
            this.btnUnitsMaster.Text = "قاموس الوحدات";
            this.btnUnitsMaster.Click += new System.EventHandler(this.btnUnitsMaster_Click);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMain.Location = new System.Drawing.Point(0, 169);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.dgvProductUnits);
            this.splitMain.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.panelConversionPreview);
            this.splitMain.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitMain.Panel2MinSize = 230;
            this.splitMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitMain.Size = new System.Drawing.Size(1201, 431);
            this.splitMain.SplitterDistance = 951;
            this.splitMain.TabIndex = 3;
            // 
            // dgvProductUnits
            // 
            this.dgvProductUnits.AllowUserToAddRows = false;
            this.dgvProductUnits.AllowUserToDeleteRows = false;
            this.dgvProductUnits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductUnits.BackgroundColor = System.Drawing.Color.White;
            this.dgvProductUnits.ColumnHeadersHeight = 32;
            this.dgvProductUnits.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLevelNumber,
            this.colUnit,
            this.colPackSize,
            this.colFactor,
            this.colDefaultBarcode,
            this.colSellPrice,
            this.colCostPrice,
            this.colId,
            this.colParentId,
            this.colParentUnit,
            this.colBarcodeType,
            this.colDisplayOrder,
            this.colHierarchyLevel,
            this.colIsBaseUnit});
            this.dgvProductUnits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductUnits.Location = new System.Drawing.Point(0, 0);
            this.dgvProductUnits.Name = "dgvProductUnits";
            this.dgvProductUnits.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvProductUnits.RowHeadersVisible = false;
            this.dgvProductUnits.RowHeadersWidth = 51;
            this.dgvProductUnits.RowTemplate.Height = 30;
            this.dgvProductUnits.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductUnits.Size = new System.Drawing.Size(951, 431);
            this.dgvProductUnits.TabIndex = 0;
            this.dgvProductUnits.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProductUnits_CellEndEdit);
            this.dgvProductUnits.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvProductUnits_CellPainting);
            this.dgvProductUnits.SelectionChanged += new System.EventHandler(this.dgvProductUnits_SelectionChanged);
            // 
            // colLevelNumber
            // 
            this.colLevelNumber.DataPropertyName = "level_number";
            this.colLevelNumber.FillWeight = 30F;
            this.colLevelNumber.HeaderText = "#";
            this.colLevelNumber.MinimumWidth = 6;
            this.colLevelNumber.Name = "colLevelNumber";
            this.colLevelNumber.ReadOnly = true;
            this.colLevelNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colUnit
            // 
            this.colUnit.DataPropertyName = "unit_name";
            this.colUnit.FillWeight = 110F;
            this.colUnit.HeaderText = "الوحدة";
            this.colUnit.MinimumWidth = 6;
            this.colUnit.Name = "colUnit";
            this.colUnit.ReadOnly = true;
            this.colUnit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colPackSize
            // 
            this.colPackSize.DataPropertyName = "pack_size";
            this.colPackSize.FillWeight = 95F;
            this.colPackSize.HeaderText = "العبوة (من السابق)";
            this.colPackSize.MinimumWidth = 6;
            this.colPackSize.Name = "colPackSize";
            this.colPackSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colFactor
            // 
            this.colFactor.DataPropertyName = "factor";
            this.colFactor.FillWeight = 90F;
            this.colFactor.HeaderText = "المعامل الإجمالي";
            this.colFactor.MinimumWidth = 6;
            this.colFactor.Name = "colFactor";
            this.colFactor.ReadOnly = true;
            this.colFactor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colDefaultBarcode
            // 
            this.colDefaultBarcode.DataPropertyName = "default_barcode";
            this.colDefaultBarcode.FillWeight = 120F;
            this.colDefaultBarcode.HeaderText = "الباركود";
            this.colDefaultBarcode.MinimumWidth = 6;
            this.colDefaultBarcode.Name = "colDefaultBarcode";
            this.colDefaultBarcode.ReadOnly = true;
            this.colDefaultBarcode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colSellPrice
            // 
            this.colSellPrice.DataPropertyName = "sell_price";
            this.colSellPrice.FillWeight = 85F;
            this.colSellPrice.HeaderText = "سعر البيع";
            this.colSellPrice.MinimumWidth = 6;
            this.colSellPrice.Name = "colSellPrice";
            this.colSellPrice.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCostPrice
            // 
            this.colCostPrice.DataPropertyName = "cost_price";
            this.colCostPrice.FillWeight = 85F;
            this.colCostPrice.HeaderText = "سعر التكلفة";
            this.colCostPrice.MinimumWidth = 6;
            this.colCostPrice.Name = "colCostPrice";
            this.colCostPrice.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colId
            // 
            this.colId.DataPropertyName = "id";
            this.colId.FillWeight = 1F;
            this.colId.MinimumWidth = 6;
            this.colId.Name = "colId";
            this.colId.Visible = false;
            // 
            // colParentId
            // 
            this.colParentId.DataPropertyName = "parent_product_unit_id";
            this.colParentId.FillWeight = 1F;
            this.colParentId.MinimumWidth = 6;
            this.colParentId.Name = "colParentId";
            this.colParentId.Visible = false;
            // 
            // colParentUnit
            // 
            this.colParentUnit.DataPropertyName = "parent_unit_name";
            this.colParentUnit.FillWeight = 1F;
            this.colParentUnit.MinimumWidth = 6;
            this.colParentUnit.Name = "colParentUnit";
            this.colParentUnit.Visible = false;
            // 
            // colBarcodeType
            // 
            this.colBarcodeType.DataPropertyName = "default_barcode_type";
            this.colBarcodeType.FillWeight = 1F;
            this.colBarcodeType.MinimumWidth = 6;
            this.colBarcodeType.Name = "colBarcodeType";
            this.colBarcodeType.Visible = false;
            // 
            // colDisplayOrder
            // 
            this.colDisplayOrder.DataPropertyName = "display_order";
            this.colDisplayOrder.FillWeight = 1F;
            this.colDisplayOrder.MinimumWidth = 6;
            this.colDisplayOrder.Name = "colDisplayOrder";
            this.colDisplayOrder.Visible = false;
            // 
            // colHierarchyLevel
            // 
            this.colHierarchyLevel.DataPropertyName = "hierarchy_level";
            this.colHierarchyLevel.FillWeight = 1F;
            this.colHierarchyLevel.MinimumWidth = 6;
            this.colHierarchyLevel.Name = "colHierarchyLevel";
            this.colHierarchyLevel.Visible = false;
            // 
            // colIsBaseUnit
            // 
            this.colIsBaseUnit.DataPropertyName = "is_base_unit";
            this.colIsBaseUnit.FillWeight = 1F;
            this.colIsBaseUnit.MinimumWidth = 6;
            this.colIsBaseUnit.Name = "colIsBaseUnit";
            this.colIsBaseUnit.Visible = false;
            // 
            // panelConversionPreview
            // 
            this.panelConversionPreview.Controls.Add(this.lblCurrentUnit);
            this.panelConversionPreview.Controls.Add(this.lblConversionTitle);
            this.panelConversionPreview.Controls.Add(this.txtConversions);
            this.panelConversionPreview.Controls.Add(this.lblFinalConversion);
            this.panelConversionPreview.Controls.Add(this.txtFinalConversion);
            this.panelConversionPreview.Controls.Add(this.lblPricesTitle);
            this.panelConversionPreview.Controls.Add(this.lblSellTitle);
            this.panelConversionPreview.Controls.Add(this.txtSellPriceUnit);
            this.panelConversionPreview.Controls.Add(this.lblCostTitle);
            this.panelConversionPreview.Controls.Add(this.txtCostPriceUnit);
            this.panelConversionPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConversionPreview.Location = new System.Drawing.Point(0, 0);
            this.panelConversionPreview.Name = "panelConversionPreview";
            this.panelConversionPreview.Padding = new System.Windows.Forms.Padding(8);
            this.panelConversionPreview.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panelConversionPreview.Size = new System.Drawing.Size(246, 431);
            this.panelConversionPreview.TabIndex = 0;
            // 
            // lblCurrentUnit
            // 
            this.lblCurrentUnit.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCurrentUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCurrentUnit.Location = new System.Drawing.Point(8, 8);
            this.lblCurrentUnit.Name = "lblCurrentUnit";
            this.lblCurrentUnit.Size = new System.Drawing.Size(230, 26);
            this.lblCurrentUnit.TabIndex = 0;
            this.lblCurrentUnit.Text = "معاينة الوحدة المحددة";
            // 
            // lblConversionTitle
            // 
            this.lblConversionTitle.Location = new System.Drawing.Point(25, 34);
            this.lblConversionTitle.Name = "lblConversionTitle";
            this.lblConversionTitle.Size = new System.Drawing.Size(214, 25);
            this.lblConversionTitle.TabIndex = 1;
            this.lblConversionTitle.Text = "معادلة التحويل:";
            // 
            // txtConversions
            // 
            this.txtConversions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConversions.BackColor = System.Drawing.Color.White;
            this.txtConversions.Location = new System.Drawing.Point(8, 62);
            this.txtConversions.Multiline = true;
            this.txtConversions.Name = "txtConversions";
            this.txtConversions.ReadOnly = true;
            this.txtConversions.Size = new System.Drawing.Size(227, 52);
            this.txtConversions.TabIndex = 2;
            this.txtConversions.TabStop = false;
            // 
            // lblFinalConversion
            // 
            this.lblFinalConversion.Location = new System.Drawing.Point(8, 122);
            this.lblFinalConversion.Name = "lblFinalConversion";
            this.lblFinalConversion.Size = new System.Drawing.Size(214, 18);
            this.lblFinalConversion.TabIndex = 3;
            this.lblFinalConversion.Text = "المعامل الإجمالي:";
            // 
            // txtFinalConversion
            // 
            this.txtFinalConversion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFinalConversion.BackColor = System.Drawing.Color.LightYellow;
            this.txtFinalConversion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.txtFinalConversion.Location = new System.Drawing.Point(8, 142);
            this.txtFinalConversion.Name = "txtFinalConversion";
            this.txtFinalConversion.ReadOnly = true;
            this.txtFinalConversion.Size = new System.Drawing.Size(235, 32);
            this.txtFinalConversion.TabIndex = 4;
            this.txtFinalConversion.TabStop = false;
            this.txtFinalConversion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPricesTitle
            // 
            this.lblPricesTitle.Location = new System.Drawing.Point(12, 177);
            this.lblPricesTitle.Name = "lblPricesTitle";
            this.lblPricesTitle.Size = new System.Drawing.Size(214, 18);
            this.lblPricesTitle.TabIndex = 5;
            this.lblPricesTitle.Text = "الأسعار:";
            // 
            // lblSellTitle
            // 
            this.lblSellTitle.AutoSize = true;
            this.lblSellTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblSellTitle.Location = new System.Drawing.Point(8, 206);
            this.lblSellTitle.Name = "lblSellTitle";
            this.lblSellTitle.Size = new System.Drawing.Size(74, 21);
            this.lblSellTitle.TabIndex = 6;
            this.lblSellTitle.Text = "سعر البيع:";
            // 
            // txtSellPriceUnit
            // 
            this.txtSellPriceUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSellPriceUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtSellPriceUnit.ForeColor = System.Drawing.Color.DarkGreen;
            this.txtSellPriceUnit.Location = new System.Drawing.Point(8, 224);
            this.txtSellPriceUnit.Name = "txtSellPriceUnit";
            this.txtSellPriceUnit.ReadOnly = true;
            this.txtSellPriceUnit.Size = new System.Drawing.Size(227, 30);
            this.txtSellPriceUnit.TabIndex = 7;
            this.txtSellPriceUnit.TabStop = false;
            this.txtSellPriceUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblCostTitle
            // 
            this.lblCostTitle.AutoSize = true;
            this.lblCostTitle.ForeColor = System.Drawing.Color.DarkRed;
            this.lblCostTitle.Location = new System.Drawing.Point(8, 258);
            this.lblCostTitle.Name = "lblCostTitle";
            this.lblCostTitle.Size = new System.Drawing.Size(89, 21);
            this.lblCostTitle.TabIndex = 8;
            this.lblCostTitle.Text = "سعر التكلفة:";
            // 
            // txtCostPriceUnit
            // 
            this.txtCostPriceUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCostPriceUnit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtCostPriceUnit.ForeColor = System.Drawing.Color.DarkRed;
            this.txtCostPriceUnit.Location = new System.Drawing.Point(8, 276);
            this.txtCostPriceUnit.Name = "txtCostPriceUnit";
            this.txtCostPriceUnit.ReadOnly = true;
            this.txtCostPriceUnit.Size = new System.Drawing.Size(227, 30);
            this.txtCostPriceUnit.TabIndex = 9;
            this.txtCostPriceUnit.TabStop = false;
            this.txtCostPriceUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelProductInfo
            // 
            this.panelProductInfo.Location = new System.Drawing.Point(0, 0);
            this.panelProductInfo.Name = "panelProductInfo";
            this.panelProductInfo.Size = new System.Drawing.Size(200, 100);
            this.panelProductInfo.TabIndex = 4;
            this.panelProductInfo.Visible = false;
            // 
            // panelButtons
            // 
            this.panelButtons.Location = new System.Drawing.Point(0, 0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(200, 100);
            this.panelButtons.TabIndex = 5;
            this.panelButtons.Visible = false;
            // 
            // btnAddUnit
            // 
            this.btnAddUnit.Location = new System.Drawing.Point(0, 0);
            this.btnAddUnit.Name = "btnAddUnit";
            this.btnAddUnit.Size = new System.Drawing.Size(75, 23);
            this.btnAddUnit.TabIndex = 0;
            this.btnAddUnit.Visible = false;
            this.btnAddUnit.Click += new System.EventHandler(this.btnAddUnit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(0, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRecalculate
            // 
            this.btnRecalculate.Location = new System.Drawing.Point(0, 0);
            this.btnRecalculate.Name = "btnRecalculate";
            this.btnRecalculate.Size = new System.Drawing.Size(75, 23);
            this.btnRecalculate.TabIndex = 0;
            this.btnRecalculate.Visible = false;
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(0, 0);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 0;
            this.btnReload.Visible = false;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // lblTransactionLock
            // 
            this.lblTransactionLock.Location = new System.Drawing.Point(0, 0);
            this.lblTransactionLock.Name = "lblTransactionLock";
            this.lblTransactionLock.Size = new System.Drawing.Size(100, 23);
            this.lblTransactionLock.TabIndex = 0;
            this.lblTransactionLock.Visible = false;
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Location = new System.Drawing.Point(0, 0);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(75, 23);
            this.btnExportCsv.TabIndex = 0;
            this.btnExportCsv.Visible = false;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // ProductUnitsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1201, 600);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.grpProductInfo);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.panelProductInfo);
            this.Controls.Add(this.panelButtons);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.MinimumSize = new System.Drawing.Size(960, 500);
            this.Name = "ProductUnitsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ادارة وحدات المنتج";
            this.Load += new System.EventHandler(this.ProductUnitsForm_Load);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.grpProductInfo.ResumeLayout(false);
            this.grpProductInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picProduct)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductUnits)).EndInit();
            this.panelConversionPreview.ResumeLayout(false);
            this.panelConversionPreview.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
