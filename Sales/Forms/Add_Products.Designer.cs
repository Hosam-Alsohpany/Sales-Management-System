namespace Sales.Forms
{
    partial class Add_Products
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Add_Products));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkHasExpiry = new System.Windows.Forms.CheckBox();
            this.chkHasImage = new System.Windows.Forms.CheckBox();
            this.picProduct = new System.Windows.Forms.PictureBox();
            this.btnUploadImage = new System.Windows.Forms.Button();
            this.btnRemoveImage = new System.Windows.Forms.Button();
            this.dtPickerValidity = new System.Windows.Forms.DateTimePicker();
            this.txtMinQty = new System.Windows.Forms.TextBox();
            this.lblMinQty = new System.Windows.Forms.Label();
            this.txtCostPrice = new System.Windows.Forms.TextBox();
            this.lblCostPrice = new System.Windows.Forms.Label();
            this.txtSku = new System.Windows.Forms.TextBox();
            this.lblSku = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbCategories = new System.Windows.Forms.ComboBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.addBtn = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picProduct)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkHasExpiry);
            this.groupBox1.Controls.Add(this.chkHasImage);
            this.groupBox1.Controls.Add(this.picProduct);
            this.groupBox1.Controls.Add(this.btnUploadImage);
            this.groupBox1.Controls.Add(this.btnRemoveImage);
            this.groupBox1.Controls.Add(this.dtPickerValidity);
            this.groupBox1.Controls.Add(this.txtMinQty);
            this.groupBox1.Controls.Add(this.lblMinQty);
            this.groupBox1.Controls.Add(this.txtCostPrice);
            this.groupBox1.Controls.Add(this.lblCostPrice);
            this.groupBox1.Controls.Add(this.txtSku);
            this.groupBox1.Controls.Add(this.lblSku);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cmbCategories);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPrice);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtNote);
            this.groupBox1.Controls.Add(this.txtQty);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(898, 571);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // chkHasExpiry
            // 
            this.chkHasExpiry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkHasExpiry.AutoSize = true;
            this.chkHasExpiry.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.chkHasExpiry.Location = new System.Drawing.Point(239, 277);
            this.chkHasExpiry.Name = "chkHasExpiry";
            this.chkHasExpiry.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkHasExpiry.Size = new System.Drawing.Size(131, 26);
            this.chkHasExpiry.TabIndex = 151;
            this.chkHasExpiry.Text = "منتج له صلاحية";
            this.chkHasExpiry.UseVisualStyleBackColor = true;
            this.chkHasExpiry.CheckedChanged += new System.EventHandler(this.chkHasExpiry_CheckedChanged);
            // 
            // chkHasImage
            // 
            this.chkHasImage.AutoSize = true;
            this.chkHasImage.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.chkHasImage.Location = new System.Drawing.Point(237, 322);
            this.chkHasImage.Name = "chkHasImage";
            this.chkHasImage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkHasImage.Size = new System.Drawing.Size(100, 26);
            this.chkHasImage.TabIndex = 152;
            this.chkHasImage.Text = "لديه صورة";
            this.chkHasImage.UseVisualStyleBackColor = true;
            this.chkHasImage.CheckedChanged += new System.EventHandler(this.chkHasImage_CheckedChanged);
            // 
            // picProduct
            // 
            this.picProduct.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picProduct.Location = new System.Drawing.Point(71, 322);
            this.picProduct.Name = "picProduct";
            this.picProduct.Size = new System.Drawing.Size(120, 120);
            this.picProduct.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picProduct.TabIndex = 153;
            this.picProduct.TabStop = false;
            this.picProduct.Visible = false;
            // 
            // btnUploadImage
            // 
            this.btnUploadImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUploadImage.Location = new System.Drawing.Point(197, 362);
            this.btnUploadImage.Name = "btnUploadImage";
            this.btnUploadImage.Size = new System.Drawing.Size(80, 30);
            this.btnUploadImage.TabIndex = 154;
            this.btnUploadImage.Text = "اختيار صورة";
            this.btnUploadImage.UseVisualStyleBackColor = false;
            this.btnUploadImage.Visible = false;
            this.btnUploadImage.Click += new System.EventHandler(this.btnUploadImage_Click);
            // 
            // btnRemoveImage
            // 
            this.btnRemoveImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveImage.Location = new System.Drawing.Point(197, 412);
            this.btnRemoveImage.Name = "btnRemoveImage";
            this.btnRemoveImage.Size = new System.Drawing.Size(80, 30);
            this.btnRemoveImage.TabIndex = 155;
            this.btnRemoveImage.Text = "إزالة صورة";
            this.btnRemoveImage.UseVisualStyleBackColor = false;
            this.btnRemoveImage.Visible = false;
            this.btnRemoveImage.Click += new System.EventHandler(this.btnRemoveImage_Click);
            // 
            // dtPickerValidity
            // 
            this.dtPickerValidity.Location = new System.Drawing.Point(33, 273);
            this.dtPickerValidity.Name = "dtPickerValidity";
            this.dtPickerValidity.Size = new System.Drawing.Size(200, 30);
            this.dtPickerValidity.TabIndex = 146;
            this.dtPickerValidity.Visible = false;
            // 
            // txtMinQty
            // 
            this.txtMinQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinQty.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtMinQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMinQty.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.txtMinQty.Location = new System.Drawing.Point(350, 455);
            this.txtMinQty.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMinQty.Multiline = true;
            this.txtMinQty.Name = "txtMinQty";
            this.txtMinQty.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMinQty.Size = new System.Drawing.Size(378, 29);
            this.txtMinQty.TabIndex = 149;
            // 
            // lblMinQty
            // 
            this.lblMinQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMinQty.AutoSize = true;
            this.lblMinQty.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblMinQty.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMinQty.Location = new System.Drawing.Point(761, 462);
            this.lblMinQty.Name = "lblMinQty";
            this.lblMinQty.Size = new System.Drawing.Size(117, 22);
            this.lblMinQty.TabIndex = 150;
            this.lblMinQty.Text = "الحد الأدنى للكميه";
            // 
            // txtCostPrice
            // 
            this.txtCostPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCostPrice.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtCostPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCostPrice.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.txtCostPrice.Location = new System.Drawing.Point(414, 314);
            this.txtCostPrice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCostPrice.Multiline = true;
            this.txtCostPrice.Name = "txtCostPrice";
            this.txtCostPrice.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtCostPrice.Size = new System.Drawing.Size(378, 29);
            this.txtCostPrice.TabIndex = 147;
            // 
            // lblCostPrice
            // 
            this.lblCostPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCostPrice.AutoSize = true;
            this.lblCostPrice.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblCostPrice.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCostPrice.Location = new System.Drawing.Point(794, 318);
            this.lblCostPrice.Name = "lblCostPrice";
            this.lblCostPrice.Size = new System.Drawing.Size(80, 22);
            this.lblCostPrice.TabIndex = 148;
            this.lblCostPrice.Text = "سعر التكلفة";
            // 
            // txtSku
            // 
            this.txtSku.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSku.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtSku.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSku.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.txtSku.Location = new System.Drawing.Point(360, 149);
            this.txtSku.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSku.Multiline = true;
            this.txtSku.Name = "txtSku";
            this.txtSku.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtSku.Size = new System.Drawing.Size(368, 29);
            this.txtSku.TabIndex = 146;
            // 
            // lblSku
            // 
            this.lblSku.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSku.AutoSize = true;
            this.lblSku.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblSku.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSku.Location = new System.Drawing.Point(746, 156);
            this.lblSku.Name = "lblSku";
            this.lblSku.Size = new System.Drawing.Size(129, 22);
            this.lblSku.TabIndex = 152;
            this.lblSku.Text = "كود داخلي (SKU)";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.txtName.Location = new System.Drawing.Point(414, 115);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Multiline = true;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(368, 29);
            this.txtName.TabIndex = 145;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 13F, System.Drawing.FontStyle.Bold);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(435, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 25);
            this.label6.TabIndex = 144;
            this.label6.Text = "صنف :";
            // 
            // cmbCategories
            // 
            this.cmbCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategories.FormattingEnabled = true;
            this.cmbCategories.Location = new System.Drawing.Point(104, 27);
            this.cmbCategories.Name = "cmbCategories";
            this.cmbCategories.Size = new System.Drawing.Size(283, 31);
            this.cmbCategories.TabIndex = 143;
            this.cmbCategories.SelectedIndexChanged += new System.EventHandler(this.cmbCategories_SelectedIndexChanged);
            // 
            // lblDate
            // 
            this.lblDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDate.Location = new System.Drawing.Point(104, 229);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(51, 22);
            this.lblDate.TabIndex = 142;
            this.lblDate.Text = "التاريخ";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(794, 268);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 22);
            this.label1.TabIndex = 141;
            this.label1.Text = "ثمن المنتج";
            // 
            // txtPrice
            // 
            this.txtPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrice.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtPrice.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.txtPrice.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtPrice.Location = new System.Drawing.Point(414, 268);
            this.txtPrice.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.txtPrice.MaximumSize = new System.Drawing.Size(467, 27);
            this.txtPrice.MinimumSize = new System.Drawing.Size(116, 21);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(378, 25);
            this.txtPrice.TabIndex = 140;
            this.txtPrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPrice_KeyDown);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(259, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 22);
            this.label7.TabIndex = 136;
            this.label7.Text = "تاريخ الإضافة";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtUserName.Enabled = false;
            this.txtUserName.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(593, 28);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtUserName.Size = new System.Drawing.Size(200, 26);
            this.txtUserName.TabIndex = 135;
            this.txtUserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtUserName.WordWrap = false;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(803, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 22);
            this.label4.TabIndex = 30;
            this.label4.Text = "المستخدم:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 13F, System.Drawing.FontStyle.Bold);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(259, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 25);
            this.label3.TabIndex = 29;
            this.label3.Text = "تفاصيل المنتج";
            // 
            // txtNote
            // 
            this.txtNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNote.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNote.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.txtNote.Location = new System.Drawing.Point(54, 113);
            this.txtNote.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNote.Size = new System.Drawing.Size(283, 86);
            this.txtNote.TabIndex = 13;
            this.txtNote.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNote_KeyDown);
            // 
            // txtQty
            // 
            this.txtQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQty.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtQty.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.txtQty.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtQty.Location = new System.Drawing.Point(414, 225);
            this.txtQty.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.txtQty.MaximumSize = new System.Drawing.Size(467, 27);
            this.txtQty.MinimumSize = new System.Drawing.Size(116, 21);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(368, 25);
            this.txtQty.TabIndex = 28;
            this.txtQty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQty_KeyDown);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(793, 229);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 22);
            this.label2.TabIndex = 27;
            this.label2.Text = "الكمية المخزنة";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold);
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(803, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 22);
            this.label5.TabIndex = 23;
            this.label5.Text = "اسم المنتج";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.addBtn);
            this.groupBox2.Controls.Add(this.btnUpdate);
            this.groupBox2.Controls.Add(this.deleteBtn);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(898, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(7);
            this.groupBox2.Size = new System.Drawing.Size(201, 224);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // addBtn
            // 
            this.addBtn.BackColor = System.Drawing.Color.White;
            this.addBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.addBtn.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.addBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.addBtn.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            this.addBtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.addBtn.ForeColor = System.Drawing.Color.Black;
            this.addBtn.Image = ((System.Drawing.Image)(resources.GetObject("addBtn.Image")));
            this.addBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.addBtn.Location = new System.Drawing.Point(7, 144);
            this.addBtn.Margin = new System.Windows.Forms.Padding(7, 11, 7, 11);
            this.addBtn.Name = "addBtn";
            this.addBtn.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.addBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.addBtn.Size = new System.Drawing.Size(187, 55);
            this.addBtn.TabIndex = 22;
            this.addBtn.Text = "إضافة";
            this.addBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addBtn.UseCompatibleTextRendering = true;
            this.addBtn.UseVisualStyleBackColor = false;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.Window;
            this.btnUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.btnUpdate.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUpdate.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.ForeColor = System.Drawing.Color.DodgerBlue;
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnUpdate.Location = new System.Drawing.Point(7, 87);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(7, 11, 7, 11);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.btnUpdate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnUpdate.Size = new System.Drawing.Size(187, 57);
            this.btnUpdate.TabIndex = 25;
            this.btnUpdate.Text = "تعديل";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.UseCompatibleTextRendering = true;
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // deleteBtn
            // 
            this.deleteBtn.BackColor = System.Drawing.Color.White;
            this.deleteBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.deleteBtn.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.deleteBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.deleteBtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.deleteBtn.ForeColor = System.Drawing.Color.Black;
            this.deleteBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.deleteBtn.Location = new System.Drawing.Point(7, 30);
            this.deleteBtn.Margin = new System.Windows.Forms.Padding(7, 11, 7, 11);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.deleteBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.deleteBtn.Size = new System.Drawing.Size(187, 57);
            this.deleteBtn.TabIndex = 24;
            this.deleteBtn.Text = "خروج";
            this.deleteBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deleteBtn.UseCompatibleTextRendering = true;
            this.deleteBtn.UseVisualStyleBackColor = false;
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
            // 
            // Add_Products
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 571);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Add_Products";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "اضافه منتج جديد      ";
            this.Load += new System.EventHandler(this.Add_Products_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picProduct)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkHasExpiry;
        private System.Windows.Forms.CheckBox chkHasImage;
        private System.Windows.Forms.PictureBox picProduct;
        private System.Windows.Forms.Button btnUploadImage;
        private System.Windows.Forms.Button btnRemoveImage;
        private System.Windows.Forms.DateTimePicker dtPickerValidity;
        private System.Windows.Forms.TextBox txtMinQty;
        private System.Windows.Forms.Label lblMinQty;
        private System.Windows.Forms.TextBox txtCostPrice;
        private System.Windows.Forms.Label lblCostPrice;
        private System.Windows.Forms.TextBox txtSku;
        private System.Windows.Forms.Label lblSku;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbCategories;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button deleteBtn;
    }
}