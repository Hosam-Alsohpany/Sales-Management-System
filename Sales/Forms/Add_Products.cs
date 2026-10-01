// ============================================================
// الملف    : Add_Products.cs
// الغرض    : نافذة إضافة/تعديل منتج مع اختيار الصنف من ComboBox وربطها بمستودعات البيانات
// يتعامل مع: Manager_Products.cs + ProductRepository.cs + CategoryRepository.cs
// الجداول  : Products, Categories, StockHistory (عبر المستودعات)
// ============================================================

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using Sales.Services;
using Sales.Utilities;
using Sales.Validators;

namespace Sales.Forms
{
    public partial class Add_Products : Form
    {
        private readonly string _userName;

        private string _createdByForEdit;
        private string _createdAtForEdit;
        private int? _expectedOldQtyForEdit;

        /// <summary>مسار الصورة الفعلي المحفوظ في قاعدة البيانات أو بعد الرفع.</summary>
        private string _persistedImagePath;

        /// <summary>لقطات المخزون عند فتح التعديل (كمية مقيّسة + منزلة عشرية).</summary>
        private long _snapshotQtyScaled;
        private int _snapshotQtyScalePow10;

        public int id_product;

        private int? _selectedCategoryId;
        private bool _isLoadingCategories;

        private readonly ProductRepository _productRepo = new ProductRepository();
        private readonly CategoryRepository _categoryRepo = new CategoryRepository();

        private ComboBox _cmbProductType;
        private Label _lblProductType;

        public enum ProductMode { Add, Edit }
        public ProductMode Mode = ProductMode.Add;

        public Add_Products(string userName)
        {
            InitializeComponent();
            _userName = userName;
            try
            {
                UiTheme.ApplyToForm(this);
                SalesUiBootstrap.WireForm(this);
            }
            catch { }
        }

        public void addmode(string txtform)
        {
            Text = txtform;
            ApplyAddEditButtonVisibility();
        }

        /// <summary>إظهار زر الإضافة أو التعديل فقط حسب الوضع لتجنب الحفظ على المعرف 0 بالخطأ.</summary>
        private void ApplyAddEditButtonVisibility()
        {
            try
            {
                bool isEdit = Mode == ProductMode.Edit;
                if (addBtn != null) addBtn.Visible = !isEdit;
                if (btnUpdate != null) btnUpdate.Visible = isEdit;
            }
            catch
            {
            }
        }

        public void SetEditData(string sku, string name, int qty, decimal price, decimal costPrice, decimal minQty, string expiryDate, string note, string createdBy, string createdAt, int? categoryId, string imagePath, long? qtyScaledSnapshot = null, int? qtyScalePow10Snapshot = null)
        {
            try { if (txtSku != null) txtSku.Text = sku ?? string.Empty; } catch { }
            _snapshotQtyScaled = qtyScaledSnapshot ?? qty;
            _snapshotQtyScalePow10 = qtyScalePow10Snapshot ?? 0;
            if (_snapshotQtyScalePow10 < 0) _snapshotQtyScalePow10 = 0;
            if (_snapshotQtyScalePow10 > 6) _snapshotQtyScalePow10 = 6;

            SetEditData(name, qty, price, costPrice, minQty, expiryDate, note, createdBy, createdAt, categoryId, imagePath);
        }

        public void SetEditData(string name, int qty, decimal price, decimal costPrice, decimal minQty, string expiryDate, string note, string createdBy, string createdAt, int? categoryId, string imagePath)
        {
            txtName.Text = name;
            txtQty.Text = SalesNumberFormat.FormatQuantity(qty);
            txtPrice.Text = SalesNumberFormat.FormatPrice(price);
            try { txtCostPrice.Text = SalesNumberFormat.FormatPrice(costPrice); } catch { }
            try { txtMinQty.Text = SalesNumberFormat.FormatQuantity(minQty); } catch { }
            txtNote.Text = note;

            _createdByForEdit = createdBy;
            _createdAtForEdit = createdAt;
            _expectedOldQtyForEdit = qty;

            txtUserName.Text = createdBy;
            try { lblDate.Text = createdAt; } catch { }
            
            // Load product image — حفظ المسار الحقيقي لقاعدة البيانات
            try
            {
                DisposeProductImage();
                _persistedImagePath = null;
                if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath.Trim()))
                {
                    string path = imagePath.Trim();
                    picProduct.Image = System.Drawing.Image.FromFile(path);
                    chkHasImage.Checked = true;
                    ShowImageControls(true);
                    _persistedImagePath = path;
                }
                else
                {
                    picProduct.Image = null;
                    chkHasImage.Checked = false;
                    ShowImageControls(false);
                }
            }
            catch
            {
                DisposeProductImage();
                _persistedImagePath = null;
                chkHasImage.Checked = false;
                ShowImageControls(false);
            }

            try
            {
                if (dtPickerValidity != null)
                {
                    if (string.IsNullOrWhiteSpace(expiryDate))
                    {
                        try
                        {
                            if (chkHasExpiry != null)
                                chkHasExpiry.Checked = false;
                        }
                        catch { }

                        try { dtPickerValidity.Visible = false; } catch { }
                    }
                    else
                    {
                        if (DateTime.TryParse(expiryDate, out var dt))
                        {
                            dtPickerValidity.Value = dt;
                            try
                            {
                                if (chkHasExpiry != null)
                                    chkHasExpiry.Checked = true;
                            }
                            catch { }

                            try { dtPickerValidity.Visible = true; } catch { }
                        }
                        else
                        {
                            try
                            {
                                if (chkHasExpiry != null)
                                    chkHasExpiry.Checked = false;
                            }
                            catch { }

                            try { dtPickerValidity.Visible = false; } catch { }
                        }
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (cmbCategories != null)
                {
                    _isLoadingCategories = true;
                    try
                    {
                        cmbCategories.DataSource = _categoryRepo.GetAllCategories();
                        cmbCategories.DisplayMember = "Name";
                        cmbCategories.ValueMember = "Id";

                        if (categoryId.HasValue)
                        {
                            cmbCategories.SelectedValue = categoryId.Value;
                            // قراءة القيمة الفعلية بعد التعيين (قد تختلف إذا لم يوجد الصنف)
                            if (cmbCategories.SelectedValue != null &&
                                int.TryParse(cmbCategories.SelectedValue.ToString(), out int actualId))
                                _selectedCategoryId = actualId;
                            else
                                _selectedCategoryId = categoryId.Value;
                        }
                        else
                        {
                            // إذا لم يكن هناك صنف محدد، استخدم أول صنف في القائمة
                            if (cmbCategories.SelectedValue != null &&
                                int.TryParse(cmbCategories.SelectedValue.ToString(), out int firstId))
                                _selectedCategoryId = firstId;
                        }
                    }
                    finally
                    {
                        _isLoadingCategories = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تحميل الأصناف: " + ex.Message);
            }
        }

        private void Add_Products_Load(object sender, EventArgs e)
        {
            try
            {
                txtUserName.Text = _userName;
                lblDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                ApplyAddEditButtonVisibility();
                EnsureProductTypeCombo();

                if (cmbCategories != null)
                {
                    _isLoadingCategories = true;
                    try
                    {
                        cmbCategories.DataSource = _categoryRepo.GetAllCategories();
                        cmbCategories.DisplayMember = "Name";
                        cmbCategories.ValueMember = "Id";

                        // تهيئة _selectedCategoryId من الاختيار الأول تلقائياً
                        if (cmbCategories.SelectedValue != null &&
                            int.TryParse(cmbCategories.SelectedValue.ToString(), out int firstCatId))
                        {
                            _selectedCategoryId = firstCatId;
                        }
                    }
                    finally
                    {
                        _isLoadingCategories = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء التحميل: " + ex.Message);
            }
        }

        private void EnsureProductTypeCombo()
        {
            if (_cmbProductType != null) return;
            if (groupBox1 == null) return;

            _lblProductType = new Label
            {
                AutoSize = true,
                Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold),
                Text = "نوع المنتج",
                RightToLeft = RightToLeft.Yes
            };
            _cmbProductType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                RightToLeft = RightToLeft.Yes,
                Font = new System.Drawing.Font("Times New Roman", 11.25F)
            };
            _cmbProductType.Items.AddRange(new object[]
            {
                new ComboItem("مادي", ProductTypeHelper.Physical),
                new ComboItem("موزون (باركود وزن)", ProductTypeHelper.Weighted),
                new ComboItem("خدمة (بدون مخزون)", ProductTypeHelper.Service)
            });
            _cmbProductType.DisplayMember = "Text";
            _cmbProductType.ValueMember = "Value";
            _cmbProductType.SelectedIndex = 0;

            _lblProductType.Location = new System.Drawing.Point(620, 318);
            _cmbProductType.Location = new System.Drawing.Point(380, 314);
            _cmbProductType.Size = new System.Drawing.Size(220, 30);

            groupBox1.Controls.Add(_lblProductType);
            groupBox1.Controls.Add(_cmbProductType);
        }

        private sealed class ComboItem
        {
            public string Text { get; }
            public string Value { get; }
            public ComboItem(string text, string value) { Text = text; Value = value; }
        }

        private string GetSelectedProductType()
        {
            if (_cmbProductType?.SelectedItem is ComboItem ci)
                return ProductTypeHelper.NormalizeProductType(ci.Value);
            return ProductTypeHelper.Physical;
        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            // تجاهل الحدث أثناء تحميل القائمة - سيتم تعيين _selectedCategoryId يدوياً بعد التحميل
            if (_isLoadingCategories) return;

            try
            {
                if (cmbCategories?.SelectedValue != null &&
                    int.TryParse(cmbCategories.SelectedValue.ToString(), out int catId))
                {
                    _selectedCategoryId = catId;
                }
                else
                {
                    // المستخدم اختار "لا يوجد صنف" أو قائمة فارغة
                    _selectedCategoryId = null;
                }
            }
            catch
            {
                _selectedCategoryId = null;
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(out int qty, out decimal price, out decimal costPrice, out decimal minQty, out string expiryDate))
                return;

            Product p = null;
            try
            {
                p = new Product
                {
                    Label = txtName.Text,
                    Sku = txtSku != null ? (txtSku.Text ?? string.Empty).Trim() : string.Empty,
                    ProductType = GetSelectedProductType(),
                    Qty = qty,
                    Price = price,
                    CostPrice = costPrice,
                    MinQty = minQty,
                    ExpiryDate = expiryDate,
                    Note = txtNote.Text,
                    CreatedBy = _userName,
                    CategoryId = _selectedCategoryId,
                    ImagePath = GetImagePath()
                };

                if (qty > 0)
                {
                    MessageBox.Show(
                        "تم تجاهل الكمية المدخلة عند الإنشاء: النظام ينشئ المنتج بمخزون صفر.\nلزيادة المخزون استخدم استلام بضاعة أو حركة مخزون معتمدة.",
                        "تنبيه مخزون",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                p.Qty = 0;

                int newProductId = _productRepo.AddProduct(p, null);

                MessageBox.Show("تمت الإضافة بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try
                {
                    if (MessageBox.Show("هل تريد فتح شاشة الوحدات والباركود الآن؟", "إدارة الوحدات", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        new ProductUnitsForm(newProductId).ShowDialog(this);
                    }
                }
                catch
                {
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء الإضافة: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(out int qty, out decimal price, out decimal costPrice, out decimal minQty, out string expiryDate))
                return;

            Product p = null;
            try
            {
                long scaled;
                int pow = _snapshotQtyScalePow10;
                if (pow < 0) pow = 0;
                if (pow > 6) pow = 6;
                if (pow == 0)
                {
                    scaled = qty;
                }
                else
                {
                    long mult = 1;
                    for (int i = 0; i < pow; i++)
                    {
                        if (mult > long.MaxValue / 10L)
                        {
                            mult = long.MaxValue;
                            break;
                        }
                        mult *= 10L;
                    }
                    try
                    {
                        scaled = checked((long)qty * mult);
                    }
                    catch (OverflowException)
                    {
                        scaled = long.MaxValue;
                    }
                }

                p = new Product
                {
                    Id = id_product,
                    Label = txtName.Text,
                    Sku = txtSku != null ? (txtSku.Text ?? string.Empty).Trim() : string.Empty,
                    ProductType = GetSelectedProductType(),
                    Qty = qty,
                    QtyScaled = scaled,
                    QtyScalePow10 = pow,
                    Price = price,
                    CostPrice = costPrice,
                    MinQty = minQty,
                    ExpiryDate = expiryDate,
                    Note = txtNote.Text,
                    CreatedBy = _createdByForEdit,
                    CategoryId = _selectedCategoryId,
                    ImagePath = GetImagePath()
                };

                _productRepo.UpdateProduct(p, _expectedOldQtyForEdit);

                MessageBox.Show("تم التعديل بنجاح", "نجاح");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != null && ex.Message.StartsWith("Concurrency conflict:", StringComparison.OrdinalIgnoreCase))
                {
                    var msg = "تم تعديل المخزون أثناء فتح شاشة التعديل.\n\n" +
                              ex.Message + "\n\n" +
                              "اختر: نعم لتجاوز التغيير (Override) أو لا لإلغاء العملية وإعادة تحميل البيانات.";

                    var res = MessageBox.Show(msg, "تعارض في التعديل", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res == DialogResult.Yes)
                    {
                        _productRepo.UpdateProduct(p, expectedOldQty: null);
                        MessageBox.Show("تم التعديل بنجاح", "نجاح");
                        DialogResult = DialogResult.OK;
                        Close();
                        return;
                    }

                    return;
                }

                MessageBox.Show("خطأ أثناء التعديل: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء التعديل: " + ex.Message);
            }
        }

        private bool ValidateInput(out int qty, out decimal price, out decimal costPrice, out decimal minQty, out string expiryDate)
        {
            qty = 0;
            price = 0;
            costPrice = 0;
            minQty = 0;
            expiryDate = null;

            if (!SalesNumberFormat.TryParseInteger(txtQty.Text, out qty))
            {
                MessageBox.Show("الكمية غير صحيحة");
                return false;
            }

            if (!SalesNumberFormat.TryParsePrice(txtPrice.Text, out price))
            {
                MessageBox.Show("السعر غير صحيح");
                return false;
            }

            string cleanCost = txtCostPrice != null ? (txtCostPrice.Text ?? string.Empty) : string.Empty;
            if (!string.IsNullOrWhiteSpace(cleanCost))
            {
                if (!SalesNumberFormat.TryParsePrice(cleanCost, out costPrice))
                {
                    MessageBox.Show("سعر التكلفة غير صحيح");
                    return false;
                }
            }

            string cleanMin = txtMinQty != null ? (txtMinQty.Text ?? string.Empty) : string.Empty;
            if (!string.IsNullOrWhiteSpace(cleanMin))
            {
                if (!SalesNumberFormat.TryParseDecimal(cleanMin, out minQty))
                {
                    MessageBox.Show("الحد الأدنى غير صحيح");
                    return false;
                }
            }

            if (!ProductValidator.TryValidate(txtName.Text.Trim(), qty, price, costPrice, minQty, out string vError))
            {
                MessageBox.Show(vError);
                return false;
            }

            try
            {
                if (dtPickerValidity != null)
                {
                    bool hasExpiry = false;
                    try { hasExpiry = chkHasExpiry != null && chkHasExpiry.Checked; } catch { hasExpiry = false; }
                    if (hasExpiry)
                        expiryDate = dtPickerValidity.Value.ToString("yyyy-MM-dd");
                }
            }
            catch
            {
            }

            return true;
        }

        private void deleteBtn_Click(object sender, EventArgs e) => Close();

        // ================== Navigation Events ==================
        private void txtName_KeyDown(object sender, KeyEventArgs e) { MoveFocus(e, txtQty); }
        private void txtQty_KeyDown(object sender, KeyEventArgs e) { MoveFocus(e, txtPrice); }
        private void txtPrice_KeyDown(object sender, KeyEventArgs e) { MoveFocus(e, txtNote); }

        private void txtNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Mode == ProductMode.Add) addBtn.PerformClick();
                else btnUpdate.PerformClick();

                e.SuppressKeyPress = true;
            }
        }

        private void MoveFocus(KeyEventArgs e, Control nextControl)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                nextControl.Focus();
                e.SuppressKeyPress = true;
            } 
        }

        private void chkHasExpiry_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtPickerValidity != null)
                {
                    dtPickerValidity.Visible = chkHasExpiry != null && chkHasExpiry.Checked;
                    if (dtPickerValidity.Visible)
                        dtPickerValidity.Focus();
                }
            }
            catch
            {
            }
        }

        private void chkHasImage_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHasImage != null && !chkHasImage.Checked)
                {
                    DisposeProductImage();
                    _persistedImagePath = null;
                }
            }
            catch { }

            ShowImageControls(chkHasImage != null && chkHasImage.Checked);
        }

        private void ShowImageControls(bool show)
        {
            try
            {
                if (picProduct != null) picProduct.Visible = show;
                if (btnUploadImage != null) btnUploadImage.Visible = show;
                if (btnRemoveImage != null) btnRemoveImage.Visible = show;
            }
            catch
            {
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "اختر صورة المنتج";
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    openFileDialog.FilterIndex = 1;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var imagePath = SaveProductImage(openFileDialog.FileName);
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            DisposeProductImage();
                            picProduct.Image = System.Drawing.Image.FromFile(imagePath);
                            _persistedImagePath = imagePath;
                            chkHasImage.Checked = true;
                            ShowImageControls(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الصورة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveImage_Click(object sender, EventArgs e)
        {
            try
            {
                DisposeProductImage();
                _persistedImagePath = null;
                chkHasImage.Checked = false;
                ShowImageControls(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إزالة الصورة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SaveProductImage(string sourceFilePath)
        {
            try
            {
                var imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Products");
                if (!System.IO.Directory.Exists(imagesFolder))
                {
                    System.IO.Directory.CreateDirectory(imagesFolder);
                }

                var fileName = System.IO.Path.GetFileName(sourceFilePath);
                var uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fileName}";
                var destinationPath = System.IO.Path.Combine(imagesFolder, uniqueFileName);

                System.IO.File.Copy(sourceFilePath, destinationPath, true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الصورة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        private static void DisposeProductImage(PictureBox box)
        {
            if (box == null) return;
            var old = box.Image;
            box.Image = null;
            old?.Dispose();
        }

        private void DisposeProductImage() => DisposeProductImage(picProduct);

        public string GetImagePath()
        {
            if (chkHasImage == null)
                return string.Empty;
            if (!chkHasImage.Checked)
                return string.Empty;
            return string.IsNullOrWhiteSpace(_persistedImagePath) ? string.Empty : _persistedImagePath.Trim();
        }
    }
}
