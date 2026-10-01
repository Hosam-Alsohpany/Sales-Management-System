// ============================================================
 // الملف    : Manager_Products.cs
 // الغرض    : شاشة إدارة المنتجات (عرض/بحث/إضافة/تعديل/حذف) وربطها مع ProductRepository و Add_Products
 // يتعامل مع: Repositories/ProductRepository.cs (CRUD + مخزون) + Forms/Add_Products.cs (نافذة إضافة/تعديل) + Models/Product.cs
 // الجداول  : Products (+ Categories عبر ProductRepository.GetAllProducts) + StockHistory (تسجيلات المخزون تتم داخل ProductRepository)
 // ============================================================

 using System;
 using System.Collections.Generic;
 using System.Data;
 using System.Drawing;
 using System.Linq;
 using System.Reflection;
 using System.Windows.Forms;
 using Sales.Models;             // استخدام النوافذ الجديدة
 using Sales.Repositories;       // استخدام المخازن الجديدة
 using System.Globalization;
 using Sales.Utilities;

 namespace Sales.Forms
 {
     public partial class Manager_Products : Form
     {
         private string _userName;
         private readonly bool _productPickerMode;

        /// <summary>عند فتح الشاشة بوضع اختيار منتج، تُملأ بعد نقر مزدوج على صف في الجدول.</summary>
        public Product PickedProduct { get; private set; }

        // استخدام القائمة بدلاً من DataTable
        // هذا يجعل التعامل مع البيانات أسرع وأكثر أماناً
         private List<Product> _allProducts;
         private ProductRepository _repo = new ProductRepository();

        // كلاس مساعد لتخزين معلومات أعمدة الجدول في combFilter
        private class ColumnFilterItem
        {
            public string HeaderText { get; set; }
            public string DataPropertyName { get; set; }
            public override string ToString() => HeaderText;
        }

        // علم لمنع الحلقة اللانهائية عند المزامنة بين combFilter والتشيك بوكسات
        private bool _syncingFilter = false;

         public Manager_Products(string userName, bool productPickerMode = false)
         {
             InitializeComponent();
             _userName = userName;
             _productPickerMode = productPickerMode;

             UiTheme.ApplyToForm(this);

             try
             {
                 KeyPreview = true;
                 KeyDown -= Manager_Products_KeyDown;
                 KeyDown += Manager_Products_KeyDown;
             }
             catch
             {
             }

             DataGridViewDateTimeFormatter.Apply(dataGridView1);
         }

         private void btnExportPdf_Click(object sender, EventArgs e)
         {
             DataGridViewExportHelper.ExportToPdf(dataGridView1, "المنتجات", "products.pdf", txtSearch?.Text);
         }

         private void btnPrint_Click(object sender, EventArgs e)
         {
             DataGridViewExportHelper.PrintGrid(dataGridView1, "المنتجات", txtSearch?.Text);
         }

         private void btnClearSearch_Click(object sender, EventArgs e)
         {
             try
             {
                 if (txtSearch != null) txtSearch.Text = string.Empty;
                 if (txtSearch != null)
                 {
                     txtSearch.Focus();
                     txtSearch.SelectAll();
                 }
             }
             catch
             {
             }
         }

         private void btnExportCsv_Click(object sender, EventArgs e)
         {
             try
             {
                 DataGridViewExportHelper.ExportToCsv(dataGridView1, "products.csv");
             }
             catch
             {
             }
         }

         private void Manager_Products_KeyDown(object sender, KeyEventArgs e)
         {
             if (e == null) return;

             try
             {
                 if (e.Control && e.KeyCode == Keys.F)
                 {
                     e.SuppressKeyPress = true;
                     if (txtSearch != null)
                     {
                         txtSearch.Focus();
                         txtSearch.SelectAll();
                     }
                     return;
                 }

                 if (e.KeyCode == Keys.Escape)
                 {
                     e.SuppressKeyPress = true;
                     if (txtSearch != null)
                         txtSearch.Text = string.Empty;
                     return;
                 }
             }
             catch
             {
             }
         }

         /// <summary>
         /// Manager_Products_Load: تحميل المنتجات من قاعدة البيانات وتثبيت اسم المستخدم وتنسيق عرض السعر
         /// المدخلات : sender/eventargs
         /// المخرجات : لا يوجد
         /// التدفق   : Form.Load → loadData → ProductRepository.GetAllProducts → dataGridView1
         /// الأخطاء  : أخطاء التحميل تظهر برسالة MessageBox داخل loadData
         /// </summary>
         private void Manager_Products_Load(object sender, EventArgs e)
         {
             loadData();
             txtUserName.Text = _userName;

             if (_productPickerMode)
             {
                 Text = "اختر منتجاً — نقر مزدوج على الصف";
                 try { toolStrip1.Visible = false; } catch { }
                 dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick_ProductPicker;
             }

             try
             {
                 if (dataGridView1.Columns["ColDefaultBarcode"] != null)
                     dataGridView1.Columns["ColDefaultBarcode"].Visible = true;
                 else if (dataGridView1.Columns["DefaultBarcode"] != null)
                     dataGridView1.Columns["DefaultBarcode"].Visible = true;
             }
             catch
             {
             }

             if (SessionManager.IsReadOnlyForCurrentUser)
             {
                 dataGridView1.ReadOnly = true;
                 dataGridView1.AllowUserToAddRows = false;
             }

            // تنسيق عمود السعر (إذا كان موجوداً)
            // تنسيق السعر يتم عبر DataGridViewNumberFormatter (أرقام إنجليزية + 5.000.00)
         }

         private void DataGridView1_CellDoubleClick_ProductPicker(object sender, DataGridViewCellEventArgs e)
         {
             if (e.RowIndex < 0) return;
             try
             {
                 if (dataGridView1.Rows[e.RowIndex].DataBoundItem is Product p)
                 {
                     PickedProduct = p;
                     DialogResult = DialogResult.OK;
                     Close();
                 }
             }
             catch
             {
             }
         }

         // ===================== دالة تحميل البيانات =====================
         /// <summary>
         /// loadData: جلب المنتجات كاملة وتخزينها في _allProducts ثم عرضها في DataGridView
         /// المدخلات : لا يوجد
         /// المخرجات : لا يوجد
         /// التدفق   : Manager_Products → loadData → ProductRepository.GetAllProducts → _allProducts → DataGridView
         /// الأخطاء  : أي Exception يتم عرضها للمستخدم برسالة (مع بقاء الشاشة تعمل)
         /// </summary>
         internal void loadData()
         {
             try
             {
                 // جلب المنتجات من المخزن (Repository)
                 _allProducts = _repo.GetAllProducts();

                 // منع توليد الأعمدة تلقائياً - نستخدم الأعمدة المُعرّفة في الـ Designer
                 dataGridView1.AutoGenerateColumns = false;
                 dataGridView1.DataSource = null;
                 dataGridView1.DataSource = _allProducts;

                 // ضمان ظهور عمود الصنف (CategoryName)
                 try
                 {
                     if (dataGridView1.Columns["ColCategoryName"] != null)
                         dataGridView1.Columns["ColCategoryName"].Visible = true;
                 }
                 catch { }

                 // مرونة ارتفاع الصفوف حسب المحتوى
                 try
                 {
                     dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                 }
                 catch { }

                 // تحديث combFilter ليعكس الأعمدة الظاهرة في الجدول
                 PopulateCombFilter();
             }
             catch (Exception ex)
             {
                 MessageHelper.ShowError("خطأ أثناء تحميل المنتجات", ex.Message);
             }
         }

        // ===================== ملء combFilter بأعمدة الجدول =====================
        /// <summary>
        /// PopulateCombFilter: يملأ combFilter بأسماء الأعمدة المرئية في dataGridView1
        /// - يُضاف عنصر "بحث عام" كأول اختيار
        /// - عند إضافة/حذف أعمدة يتم تحديثه تلقائياً عبر استدعاء loadData
        /// </summary>
        private void PopulateCombFilter()
        {
            try
            {
                // حفظ الاختيار الحالي قبل إعادة الملء
                string previousSelection = null;
                if (combFilter.SelectedItem is ColumnFilterItem prev)
                    previousSelection = prev.DataPropertyName;

                combFilter.Items.Clear();

                // عنصر أول: بحث عام حسب التشيك بوكس
                combFilter.Items.Add(new ColumnFilterItem
                {
                    HeaderText = "-- بحث عام --",
                    DataPropertyName = string.Empty
                });

                // إضافة كل عمود مرئي يحتوي على DataPropertyName
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    if (col.Visible && !string.IsNullOrEmpty(col.DataPropertyName))
                    {
                        combFilter.Items.Add(new ColumnFilterItem
                        {
                            HeaderText = col.HeaderText,
                            DataPropertyName = col.DataPropertyName
                        });
                    }
                }

                // إعادة تحديد الاختيار السابق إن وُجد، وإلا البداية
                bool restored = false;
                if (previousSelection != null)
                {
                    for (int i = 0; i < combFilter.Items.Count; i++)
                    {
                        if (combFilter.Items[i] is ColumnFilterItem item &&
                            item.DataPropertyName == previousSelection)
                        {
                            combFilter.SelectedIndex = i;
                            restored = true;
                            break;
                        }
                    }
                }
                if (!restored && combFilter.Items.Count > 0)
                    combFilter.SelectedIndex = 0;
            }
            catch { }
        }

         // ===================== البحث =====================
         /// <summary>
         /// txtSearch_TextChanged: فلترة قائمة المنتجات المعروضة حسب نص البحث (اسم/ملاحظات/معرف)
         /// المدخلات : sender/eventargs
         /// المخرجات : لا يوجد
         /// التدفق   : كتابة في txtSearch → LINQ Where على _allProducts → DataGridView.DataSource
         /// الأخطاء  : إذا كانت القائمة غير محملة (_allProducts=null) يتم الخروج مباشرة
         /// </summary>
         private void txtSearch_TextChanged(object sender, EventArgs e)
         {
             if (_allProducts == null) return;

            string text = (txtSearch.Text ?? string.Empty).Trim().ToLower();

            List<Product> filteredList;

            // ── أولاً: هل combFilter يختار عموداً محدداً؟ ──
            ColumnFilterItem selectedFilter = combFilter?.SelectedItem as ColumnFilterItem;
            bool hasColumnFilter = selectedFilter != null &&
                                   !string.IsNullOrEmpty(selectedFilter.DataPropertyName);

            if (hasColumnFilter)
            {
                // بحث في خاصية Product المقابلة للعمود المختار باستخدام Reflection
                string propName = selectedFilter.DataPropertyName;
                PropertyInfo prop = typeof(Product).GetProperty(propName);

                if (prop != null)
                {
                    filteredList = _allProducts.Where(p =>
                        p != null &&
                        (prop.GetValue(p)?.ToString() ?? string.Empty)
                            .ToLower().Contains(text)
                    ).ToList();
                }
                else
                {
                    // العمود لا يقابل خاصية مباشرة → بحث عام
                    filteredList = _allProducts.Where(p =>
                        p != null &&
                        ((p.Label ?? string.Empty).ToLower().Contains(text)) ||
                        ((p.Note ?? string.Empty).ToLower().Contains(text)) ||
                        p.Id.ToString().Contains(text)
                    ).ToList();
                }
            }
            else
            {
                // ── ثانياً: البحث عبر التشيك بوكس ──
                bool searchByProduct  = false;
                bool searchByCategory = false;
                bool searchByBarcode  = false;

                try { searchByProduct  = checkProduct   != null && checkProduct.Checked;   } catch { }
                try { searchByCategory = checkCategorie != null && checkCategorie.Checked; } catch { }
                try { searchByBarcode  = checkBarcode   != null && checkBarcode.Checked;   } catch { }

                // إذا لم يُختر أي وضع → بحث بالمنتج افتراضياً
                if (!searchByProduct && !searchByCategory && !searchByBarcode)
                    searchByProduct = true;

                if (searchByCategory)
                {
                    // بحث حسب اسم الصنف CategoryName
                    filteredList = _allProducts.Where(p =>
                        p != null &&
                        (p.CategoryName ?? string.Empty).ToLower().Contains(text)
                    ).ToList();
                }
                else if (searchByBarcode)
                {
                    // بحث حسب الباركود DefaultBarcode
                    filteredList = _allProducts.Where(p =>
                        p != null &&
                        (p.DefaultBarcode ?? string.Empty).ToLower().Contains(text)
                    ).ToList();
                }
                else
                {
                    // بحث بالاسم أو الملاحظات أو المعرف
                    filteredList = _allProducts.Where(p =>
                        p != null &&
                        (((p.Label ?? string.Empty).ToLower().Contains(text)) ||
                        ((p.Note  ?? string.Empty).ToLower().Contains(text)) ||
                        p.Id.ToString().Contains(text))
                    ).ToList();
                }
            }

            dataGridView1.DataSource = filteredList;
         }

         private void checkProduct_CheckedChanged(object sender, EventArgs e)
         {
             if (_syncingFilter) return;
             try
             {
                 if (checkProduct != null && checkProduct.Checked)
                 {
                     if (checkCategorie != null) checkCategorie.Checked = false;
                     if (checkBarcode   != null) checkBarcode.Checked   = false;
                     // مزامنة combFilter ليختار عمود اسم المنتج
                     SyncCombFilterToDataProperty("Label");
                 }
                 else
                 {
                     if ((checkCategorie == null || !checkCategorie.Checked) &&
                         (checkBarcode   == null || !checkBarcode.Checked))
                         checkProduct.Checked = true;
                 }
             }
             catch { }

             txtSearch_TextChanged(txtSearch, EventArgs.Empty);
             FocusSearch();
         }

         private void checkCategorie_CheckedChanged(object sender, EventArgs e)
         {
             if (_syncingFilter) return;
             try
             {
                 if (checkCategorie != null && checkCategorie.Checked)
                 {
                     if (checkProduct != null) checkProduct.Checked = false;
                     if (checkBarcode != null) checkBarcode.Checked = false;
                     // مزامنة combFilter ليختار عمود الصنف
                     SyncCombFilterToDataProperty("CategoryName");
                 }
                 else
                 {
                     if ((checkProduct == null || !checkProduct.Checked) &&
                         (checkBarcode  == null || !checkBarcode.Checked))
                         checkCategorie.Checked = true;
                 }
             }
             catch { }

             txtSearch_TextChanged(txtSearch, EventArgs.Empty);
             FocusSearch();
         }

         private void checkBarcode_CheckedChanged(object sender, EventArgs e)
         {
             if (_syncingFilter) return;
             try
             {
                 if (checkBarcode != null && checkBarcode.Checked)
                 {
                     if (checkProduct   != null) checkProduct.Checked   = false;
                     if (checkCategorie != null) checkCategorie.Checked = false;
                     // مزامنة combFilter ليختار عمود الباركود
                     SyncCombFilterToDataProperty("DefaultBarcode");
                 }
                 else
                 {
                     if ((checkProduct   == null || !checkProduct.Checked) &&
                         (checkCategorie  == null || !checkCategorie.Checked))
                         checkBarcode.Checked = true;
                 }
             }
             catch { }

             txtSearch_TextChanged(txtSearch, EventArgs.Empty);
             FocusSearch();
         }

         private void combFilter_SelectedIndexChanged(object sender, EventArgs e)
         {
             if (_syncingFilter) return;
             _syncingFilter = true;
             try
             {
                 // مزامنة التشيك بوكس حسب العمود المختار
                 var item = combFilter?.SelectedItem as ColumnFilterItem;
                 string prop = item?.DataPropertyName ?? string.Empty;

                 switch (prop)
                 {
                     case "Label":
                         if (checkProduct   != null) checkProduct.Checked   = true;
                         if (checkCategorie != null) checkCategorie.Checked = false;
                         if (checkBarcode   != null) checkBarcode.Checked   = false;
                         break;
                     case "CategoryName":
                         if (checkProduct   != null) checkProduct.Checked   = false;
                         if (checkCategorie != null) checkCategorie.Checked = true;
                         if (checkBarcode   != null) checkBarcode.Checked   = false;
                         break;
                     case "DefaultBarcode":
                         if (checkProduct   != null) checkProduct.Checked   = false;
                         if (checkCategorie != null) checkCategorie.Checked = false;
                         if (checkBarcode   != null) checkBarcode.Checked   = true;
                         break;
                     default:
                         // عمود آخر أو بحث عام → المنتج كافتراضي
                         if (checkProduct   != null) checkProduct.Checked   = true;
                         if (checkCategorie != null) checkCategorie.Checked = false;
                         if (checkBarcode   != null) checkBarcode.Checked   = false;
                         break;
                 }
             }
             catch { }
             finally { _syncingFilter = false; }

             txtSearch_TextChanged(txtSearch, EventArgs.Empty);
             FocusSearch();
         }

         // ===================== دوال مساعدة =====================

         /// <summary>يُحرّك التشيك بوكس لمزامنة combFilter مع الخاصية المطلوبة</summary>
         private void SyncCombFilterToDataProperty(string dataPropertyName)
         {
             _syncingFilter = true;
             try
             {
                 for (int i = 0; i < combFilter.Items.Count; i++)
                 {
                     if (combFilter.Items[i] is ColumnFilterItem item &&
                         item.DataPropertyName == dataPropertyName)
                     {
                         combFilter.SelectedIndex = i;
                         return;
                     }
                 }
                 // لم يُوجد العمود في combFilter → اختر "بحث عام"
                 if (combFilter.Items.Count > 0)
                     combFilter.SelectedIndex = 0;
             }
             catch { }
             finally { _syncingFilter = false; }
         }

         /// <summary>ينقل الفوكس إلى حقل البحث ويحدد محتواه</summary>
         private void FocusSearch()
         {
             try
             {
                 if (txtSearch != null && txtSearch.CanFocus)
                 {
                     txtSearch.Focus();
                     txtSearch.SelectAll();
                 }
             }
             catch { }
         }

         /// <summary>
         /// btnClose_Click: إغلاق شاشة إدارة المنتجات
         /// </summary>
         private void btnClose_Click(object sender, EventArgs e)
         {
             Close();
         }

         // ===================== إضافة منتج =====================
         /// <summary>
         /// btnِAddProduct_Click: فتح نافذة Add_Products بوضع الإضافة ثم إعادة تحميل البيانات عند نجاح الحفظ
         /// المدخلات : sender/eventargs
         /// المخرجات : لا يوجد
         /// التدفق   : Click → Add_Products(addmode) → DialogResult.OK → loadData
         /// الأخطاء  : الأخطاء الداخلية بالحفظ تُدار داخل Add_Products/ProductRepository
         /// </summary>
         private void btnِAddProduct_Click(object sender, EventArgs e)
         {
             if (SessionManager.IsReadOnlyForCurrentUser)
             {
                 MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                 return;
             }

            var frm = new Add_Products(_userName);
            frm.Mode = Add_Products.ProductMode.Add;
            frm.addmode("إضافة منتج جديد");

            if (frm.ShowDialog() == DialogResult.OK)
            {
                loadData(); // تحديث القائمة
            }
        }

         // ===================== تعديل منتج =====================
         /// <summary>
         /// btnEditProduct_Click: فتح نافذة Add_Products بوضع التعديل للمنتج المحدد ثم إعادة تحميل البيانات عند نجاح التعديل
         /// المدخلات : sender/eventargs
         /// المخرجات : لا يوجد
         /// التدفق   : اختيار صف → استخراج Product من DataBoundItem → SetEditData → DialogResult.OK → loadData
         /// الأخطاء  : عدم اختيار صف، أو أخطاء داخل Add_Products/Repository
         /// </summary>
         private void btnEditProduct_Click(object sender, EventArgs e)
         {
             if (SessionManager.IsReadOnlyForCurrentUser)
             {
                 MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                 return;
             }

            if (dataGridView1.CurrentRow == null)
            {
                MessageHelper.ShowWarning("اختر منتج أولاً");
               return;
            }

            // استخراج العنصر المحدد (الآن هو كائن Product وليس DataRow)
            Product selectedProduct = (Product)dataGridView1.CurrentRow.DataBoundItem;

            var frm = new Add_Products(_userName);
            frm.Mode = Add_Products.ProductMode.Edit;
            frm.id_product = selectedProduct.Id; // تمرير المعرف

            // دالة SetEditData تحتاج تعديل في الفورم الآخر لتقبل الكائن
            // سنمرر القيم يدوياً للتبسيط حالياً كما كان سابقاً
            frm.SetEditData(
                selectedProduct.Sku,
                selectedProduct.Label,
                selectedProduct.Qty,
                selectedProduct.Price,
                selectedProduct.CostPrice,
                selectedProduct.MinQty,
                selectedProduct.ExpiryDate,
                selectedProduct.Note,
                selectedProduct.CreatedBy,
                selectedProduct.CreatedAt,
                selectedProduct.CategoryId,
                selectedProduct.ImagePath,
                selectedProduct.QtyScaled,
                selectedProduct.QtyScalePow10);

            frm.addmode("تعديل المنتج (" + selectedProduct.Label + ")");

            if (frm.ShowDialog() == DialogResult.OK)
            {
                loadData();
            }
        }

         // ===================== حذف منتج =====================
         /// <summary>
         /// btnDeleteProduct_Click: حذف المنتج المحدد بعد التأكيد عبر ProductRepository.DeleteProduct (يتطلب Admin)
         /// المدخلات : sender/eventargs
         /// المخرجات : لا يوجد
         /// التدفق   : تأكيد المستخدم → ProductRepository.DeleteProduct → loadData
         /// الأخطاء  : عدم صلاحية (RequireAdmin) أو أخطاء SQLite تظهر كـ Exception
         /// </summary>
         private void btnDeleteProduct_Click(object sender, EventArgs e)
         {
             if (dataGridView1.CurrentRow == null) return;

             if (SessionManager.IsReadOnlyForCurrentUser)
             {
                 MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                 return;
             }

            if (!SessionManager.CanCurrentUserDelete)
            {
                MessageHelper.ShowUnauthorized("حذف المنتجات");
                return;
            }

            if (!MessageHelper.AskForConfirmation("هل أنت متأكد من الحذف؟"))
                return;

            try
            {
                Product selectedProduct = (Product)dataGridView1.CurrentRow.DataBoundItem;
                _repo.DeleteProduct(selectedProduct.Id);
                MessageHelper.ShowSuccess("تم حذف المنتج بنجاح");
                loadData();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ أثناء الحذف", ex.Message);
            }
         }

        private void btnProductUnits_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                new ProductUnitsForm().ShowDialog();
                return;
            }

            var selectedProduct = dataGridView1.CurrentRow.DataBoundItem as Product;
            if (selectedProduct == null)
            {
                new ProductUnitsForm().ShowDialog();
                return;
            }

            new ProductUnitsForm(selectedProduct.Id).ShowDialog();
        }

        private void btnManageCategories_Click(object sender, EventArgs e)
        {
            new Manager_Categories().ShowDialog();
        }

        private void btnManageUnits_Click(object sender, EventArgs e)
        {
            new UnitsForm().ShowDialog();
        }


        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  الدور          : إدارة المنتجات (UI)          ║
        // ║  يعتمد على       : ProductRepository + Add_Products ║
        // ║  أهم التدفقات   : Load/بحث/إضافة/تعديل/حذف     ║
        // ║  الجداول        : Products (+ Categories/StockHistory عبر المستودع) ║
        // ╚══════════════════════════════════════════════╝
    }
 }
