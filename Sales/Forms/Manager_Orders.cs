// ============================================================
 // الملف    : Manager_Orders.cs
 // الغرض    : شاشة إنشاء/تعديل فاتورة بيع: إدخال المنتجات (سطر/سطر) ثم حفظها عبر OrderRepository مع حساب المدفوع/المتبقي
 // يتعامل مع: Repositories/OrderRepository.cs (SaveOrder/UpdateOrder) + Repositories/ProductRepository.cs (جلب المنتج) + Customers_List.cs (اختيار عميل)
 // الجداول  : Products, Orders, Order_Details, Payments, StockHistory (عبر الـ Repositories)
 // ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class Manager_Orders : Form
    {
        // _FullName: اسم المستخدم/البائع الذي تم تمريره من الشاشة الرئيسية (يُستخدم في created_by)
        private string _FullName;
        private string _draftRequestId;
        // idOrder: رقم الفاتورة عند وضع التعديل (Mode=Edit)
        public int idOrder;
        // dt: جدول بيانات وسيط يمثل تفاصيل الفاتورة على الشاشة (يُعرض في DataGridView)
        public DataTable dt = new DataTable();

        // تعريف المخازن (Repositories)
        private readonly ProductRepository _productRepo = new ProductRepository();
        private readonly OrderRepository _orderRepo = new OrderRepository();
        private CategoryRepository _categoryRepo = new CategoryRepository();

        // كاش بسيط لأسماء الأصناف لتجنب الاستعلام المتكرر عند إدخال عدة منتجات
        private Dictionary<int, string> _categoryNameById = new Dictionary<int, string>();

        // تخزين مؤقت لتفاصيل/ملاحظة المنتج الحالي حتى نضعها داخل صف الجدول عند الإضافة
        private string _currentProductNote = string.Empty;

        // عند تعديل سطر من الجدول: نحفظ رقم الصف حتى لا نضيف سطر جديد بالخطأ
        private int _editingRowIndex = -1;

        private List<ProductUnitLookup> _currentProductUnits = new List<ProductUnitLookup>();
        private ProductUnitLookup _selectedProductUnit;

        // OrderMode: يحدد هل الشاشة في وضع إضافة فاتورة جديدة أم تعديل فاتورة موجودة
        public enum OrderMode { Add, Edit }
        public OrderMode Mode = OrderMode.Add;

        // وسوم بسيطة نُخزن بها الخصم داخل note (بدون تعديل قاعدة البيانات)
        private const string DiscountTagPrefix = "[خصم:";
        private const string DiscountTagSuffix = "]";

        private readonly ToolTip _toolTip = new ToolTip();

        private sealed class UndoState
        {
            public DataTable Table;
            public int EditingRowIndex;
            public string StatusText;
        }

        private readonly Stack<UndoState> _undoStack = new Stack<UndoState>();
        private const int MaxUndoStates = 50;
        private Timer _statusTimer;

        private bool IsProductLoadedForEntry
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtID.Text)) return false;
                    if (string.IsNullOrWhiteSpace(txtLabel.Text)) return false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public Manager_Orders(string FullName)
        {
            InitializeComponent();
            _FullName = FullName;

            UiTheme.ApplyToForm(this);
        }

        /// <summary>
        /// Manager_Orders_Load: تهيئة مصدر البيانات للجدول وتنسيق الأعمدة وربط أحداث الحسابات
        /// المدخلات : sender/eventargs من WinForms
        /// المخرجات : لا يوجد
        /// التدفق   : Form.Load → SetDataSRC/ResizeDGV → (ربط txtPaid_TextChanged) → تهيئة اسم البائع
        /// الأخطاء  : لا يوجد (عمليات UI فقط)
        /// </summary>
        private void Manager_Orders_Load(object sender, EventArgs e)
        {
            SetDataSRC();
            ResizeDGV();
            txtPaid.TextChanged += txtPaid_TextChanged;

            try
            {
                EnsureStatusTimer();
                SetStatus("جاهز", isError: false, autoClearMs: 0);
            }
            catch
            {
            }

            try
            {
                KeyPreview = true;
                KeyDown -= Manager_Orders_KeyDown;
                KeyDown += Manager_Orders_KeyDown;
            }
            catch
            {
            }

            try
            {
                dataGridView_Product.CellBeginEdit -= dataGridView_Product_CellBeginEdit;
                dataGridView_Product.CellBeginEdit += dataGridView_Product_CellBeginEdit;
            }
            catch
            {
            }

            // ── صلاحيات الفواتير للمستخدم العادي ──
            // 1) ReadOnlyForNormalUser: يمنع أي تعديل/حفظ
            // 2) AllowNormalUserCreateOrders: السماح بإنشاء فاتورة جديدة
            // 3) AllowNormalUserEditOrders: السماح بتعديل فاتورة موجودة
            try
            {
                if (!SessionManager.IsAdmin)
                {
                    bool readOnly = AppSettingsManager.GetBool(AppSettingsManager.Keys.ReadOnlyForNormalUser, false);
                    bool allowCreate = AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserCreateOrders, true);
                    bool allowEdit = AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserEditOrders, false);

                    if (readOnly)
                    {
                        DisableOrderEditingUi();
                    }
                    else
                    {
                        if (Mode == OrderMode.Add && !allowCreate)
                            DisableOrderEditingUi();

                        if (Mode == OrderMode.Edit && !allowEdit)
                            DisableOrderEditingUi();
                    }
                }
            }
            catch
            {
                // تجاهل
            }

            // تهيئة حقول الفاتورة التفصيلية (خصم/صافي) + تهيئة عنوانها داخل الواجهة
            try
            {
                // label15/label16 موجودة في Designer بدون نص → نضع لها تسميات مفهومة
                label15.Text = "الخصم";
                label16.Text = "الصافي";

                // الخصم يجب أن يكون قابل للإدخال فقط عند السماح
                textBox11.ReadOnly = !CanEditOrdersNow();
                if (string.IsNullOrWhiteSpace(textBox11.Text))
                    textBox11.Text = "0";

                // الصافي للعرض فقط
                textBox12.ReadOnly = true;

                // إعادة حساب الصافي/المتبقي عند تغيير الخصم
                textBox11.TextChanged += (s, e2) =>
                {
                    if (!CanEditOrdersNow()) return;
                    CalculAmount();
                };
            }
            catch
            {
                // تجاهل: حماية لو تغيرت أسماء العناصر في Designer
            }

            // تحميل كاش الأصناف مرة واحدة
            try
            {
                _categoryNameById = _categoryRepo.GetAllCategories()
                    .Where(c => c != null)
                    .GroupBy(c => c.Id)
                    .ToDictionary(g => g.Key, g => g.First().Name ?? string.Empty);
            }
            catch
            {
                _categoryNameById = new Dictionary<int, string>();
            }

            if (Mode == OrderMode.Add)
            {
                txtSaleMan.Text = _FullName;
            }

            EnsureUnitsUi();

            try
            {
                txtPrice.KeyDown -= txtPrice_KeyDown;
                txtPrice.KeyDown += txtPrice_KeyDown;
            }
            catch
            {
            }

            try
            {
                if (txtPrice != null)
                    txtPrice.ReadOnly = !CanEditOrdersNow();
            }
            catch
            {
            }
        }

        private void EnsureStatusTimer()
        {
            if (_statusTimer != null) return;

            _statusTimer = new Timer();
            _statusTimer.Interval = 2500;
            _statusTimer.Tick += (s, e) =>
            {
                try
                {
                    _statusTimer.Stop();
                    if (lblStatus != null) lblStatus.Text = "جاهز";
                }
                catch
                {
                }
            };
        }

        private void SetStatus(string text, bool isError, int autoClearMs)
        {
            if (lblStatus == null) return;

            lblStatus.Text = string.IsNullOrWhiteSpace(text) ? "جاهز" : text;
            try
            {
                lblStatus.ForeColor = isError ? Color.Firebrick : Color.Black;
            }
            catch
            {
            }

            if (autoClearMs > 0)
            {
                EnsureStatusTimer();
                try
                {
                    _statusTimer.Interval = Math.Max(500, autoClearMs);
                    _statusTimer.Stop();
                    _statusTimer.Start();
                }
                catch
                {
                }
            }
        }

        private void PushUndoState(string statusText)
        {
            try
            {
                if (dt == null) return;

                while (_undoStack.Count >= MaxUndoStates)
                {
                    var keep = _undoStack.Take(MaxUndoStates - 1).Reverse().ToArray();
                    _undoStack.Clear();
                    for (int i = 0; i < keep.Length; i++)
                        _undoStack.Push(keep[i]);
                }

                _undoStack.Push(new UndoState
                {
                    Table = dt.Copy(),
                    EditingRowIndex = _editingRowIndex,
                    StatusText = statusText
                });
            }
            catch
            {
            }
        }

        private void UndoLast()
        {
            if (!EnsureCanEditOrdersNowAndShowMessage()) return;

            if (_undoStack.Count == 0)
            {
                SetStatus("لا يوجد تراجع", isError: false, autoClearMs: 2000);
                return;
            }

            try
            {
                var s = _undoStack.Pop();
                if (s == null || s.Table == null) return;

                dt = s.Table;
                _editingRowIndex = s.EditingRowIndex;
                try
                {
                    if (dataGridView_Product != null)
                        dataGridView_Product.DataSource = dt;
                }
                catch
                {
                }

                try { ResizeDGV(); } catch { }
                try { CalculAmount(); } catch { }

                SetStatus("تم التراجع", isError: false, autoClearMs: 2500);
            }
            catch
            {
            }
        }

        private void CancelCurrentEntryOrEdit()
        {
            try
            {
                if (_editingRowIndex >= 0)
                {
                    _editingRowIndex = -1;
                    txtClear();
                    SetStatus("تم إلغاء وضع التعديل", isError: false, autoClearMs: 2000);
                    try { txtID.Focus(); } catch { }
                    return;
                }

                if (!string.IsNullOrWhiteSpace(txtID.Text) || !string.IsNullOrWhiteSpace(txtQty.Text) || !string.IsNullOrWhiteSpace(txtLabel.Text))
                {
                    txtClear();
                    SetStatus("تم إلغاء الإدخال", isError: false, autoClearMs: 1500);
                }

                try { txtID.Focus(); } catch { }
            }
            catch
            {
            }
        }

        private bool TryGetEntryProductId(out int productId)
        {
            productId = 0;
            try
            {
                if (!int.TryParse((txtID.Text ?? string.Empty).Trim(), out productId))
                    return false;
                return productId > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool TryGetEntryQty(out decimal qtyUnit)
        {
            qtyUnit = 0m;
            try
            {
                if (!SalesNumberFormat.TryParseDecimal((txtQty.Text ?? string.Empty), out qtyUnit))
                    return false;
                return qtyUnit > 0m;
            }
            catch
            {
                return false;
            }
        }

        private bool TryGetEntryPrice(out decimal price)
        {
            price = 0m;
            try
            {
                if (!SalesNumberFormat.TryParsePrice((txtPrice.Text ?? string.Empty), out price))
                    return false;
                return price > 0m;
            }
            catch
            {
                return false;
            }
        }

        private bool TryAddOrUpdateLineFromEntry(bool invokedFromPrice)
        {
            if (!EnsureCanEditOrdersNowAndShowMessage()) return false;

            if (!IsProductLoadedForEntry)
            {
                SetStatus("اختر منتج أولاً", isError: true, autoClearMs: 2500);
                try { txtID.Focus(); } catch { }
                return false;
            }

            if (!TryGetEntryProductId(out int productId))
            {
                SetStatus("رقم المنتج غير صحيح", isError: true, autoClearMs: 2500);
                try { txtID.Focus(); txtID.SelectAll(); } catch { }
                return false;
            }

            if (!TryGetEntryQty(out decimal qtyUnitRequired))
            {
                SetStatus("أدخل كمية صحيحة", isError: true, autoClearMs: 2500);
                try { txtQty.Focus(); txtQty.SelectAll(); } catch { }
                return false;
            }

            var pu = _selectedProductUnit;
            if (pu == null)
            {
                SetStatus("اختر الوحدة أولاً", isError: true, autoClearMs: 2500);
                try { if (cmbUnits != null) cmbUnits.Focus(); } catch { }
                return false;
            }

            if (!TryGetEntryPrice(out decimal price))
            {
                SetStatus("السعر غير صحيح", isError: true, autoClearMs: 2500);
                try { txtPrice.Focus(); txtPrice.SelectAll(); } catch { }
                return false;
            }

            decimal baseQtyRequiredDec = qtyUnitRequired * (pu.Factor <= 0 ? 1m : pu.Factor);
            int baseQtyRequired = Convert.ToInt32(Math.Round(baseQtyRequiredDec, 0, MidpointRounding.AwayFromZero));
            if (baseQtyRequired <= 0)
            {
                SetStatus("الكمية غير صحيحة", isError: true, autoClearMs: 2500);
                try { txtQty.Focus(); txtQty.SelectAll(); } catch { }
                return false;
            }

            Product p = null;
            try { p = _productRepo.GetProductById(productId); } catch { }
            if (p == null)
            {
                SetStatus("المنتج غير موجود", isError: true, autoClearMs: 2500);
                try { txtID.Focus(); txtID.SelectAll(); } catch { }
                return false;
            }

            if (p.DisplayQty < baseQtyRequired)
            {
                SetStatus("المخزون غير كاف", isError: true, autoClearMs: 3000);
                try { txtQty.Focus(); txtQty.SelectAll(); } catch { }
                return false;
            }

            try
            {
                decimal existingQty = 0m;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i == _editingRowIndex) continue;
                    var r = dt.Rows[i];
                    if (r == null || r.RowState == DataRowState.Deleted) continue;
                    int existingProductId = Convert.ToInt32(r["المعرف"]);
                    if (existingProductId != productId) continue;

                    decimal rowBase = 0m;
                    try { rowBase = Convert.ToDecimal(r["base_qty"]); }
                    catch { rowBase = Convert.ToDecimal(r["الكمية"]); }

                    existingQty += rowBase;
                }

                decimal requiredTotal = existingQty + baseQtyRequired;
                if (requiredTotal > p.DisplayQty)
                {
                    SetStatus("الكمية داخل الفاتورة تتجاوز المخزون", isError: true, autoClearMs: 3500);
                    try { txtQty.Focus(); txtQty.SelectAll(); } catch { }
                    return false;
                }
            }
            catch
            {
            }

            PushUndoState(_editingRowIndex >= 0 ? "تعديل سطر" : "إضافة سطر");

            decimal amount = qtyUnitRequired * price;
            decimal baseQty = qtyUnitRequired * (pu.Factor <= 0 ? 1m : pu.Factor);

            int updatedRowIndex = -1;

            if (_editingRowIndex >= 0 && _editingRowIndex < dt.Rows.Count)
            {
                DataRow row = dt.Rows[_editingRowIndex];
                row["المعرف"] = productId;
                row["المنتج"] = txtLabel.Text;
                row["تفاصيل"] = _currentProductNote;
                row["الصنف"] = txtCategories.Text;
                row["الكمية"] = qtyUnitRequired;
                row["السعر"] = price;
                row["المبلغ"] = amount;

                row["product_unit_id"] = pu.ProductUnitId;
                row["unit_name"] = pu.UnitName;
                row["factor"] = pu.Factor;
                row["qty_unit"] = qtyUnitRequired;
                row["base_qty"] = baseQty;
                row["cost_price"] = pu.CostPrice;

                updatedRowIndex = _editingRowIndex;
                _editingRowIndex = -1;
            }
            else
            {
                DataRow row = dt.NewRow();
                row["المعرف"] = productId;
                row["المنتج"] = txtLabel.Text;
                row["تفاصيل"] = _currentProductNote;
                row["الصنف"] = txtCategories.Text;
                row["الكمية"] = qtyUnitRequired;
                row["السعر"] = price;
                row["المبلغ"] = amount;

                row["product_unit_id"] = pu.ProductUnitId;
                row["unit_name"] = pu.UnitName;
                row["factor"] = pu.Factor;
                row["qty_unit"] = qtyUnitRequired;
                row["base_qty"] = baseQty;
                row["cost_price"] = pu.CostPrice;
                dt.Rows.Add(row);
                updatedRowIndex = dt.Rows.Count - 1;
            }

            try { CalculAmount(); } catch { }

            txtClear();

            try
            {
                if (dataGridView_Product != null && updatedRowIndex >= 0 && updatedRowIndex < dataGridView_Product.Rows.Count)
                {
                    dataGridView_Product.ClearSelection();
                    dataGridView_Product.Rows[updatedRowIndex].Selected = true;
                }
            }
            catch
            {
            }

            try { txtID.Focus(); } catch { }

            SetStatus("تم حفظ السطر", isError: false, autoClearMs: 1200);
            return true;
        }

        private bool CanEditOrdersNow()
        {
            try
            {
                if (SessionManager.IsAdmin) return true;

                if (AppSettingsManager.GetBool(AppSettingsManager.Keys.ReadOnlyForNormalUser, false))
                    return false;

                if (Mode == OrderMode.Add)
                    return AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserCreateOrders, true);

                return AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserEditOrders, false);
            }
            catch
            {
                return true;
            }
        }

        private bool EnsureCanEditOrdersNowAndShowMessage()
        {
            if (CanEditOrdersNow())
                return true;

            try
            {
                bool isReadOnly = AppSettingsManager.GetBool(AppSettingsManager.Keys.ReadOnlyForNormalUser, false);
                bool allowCreate = AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserCreateOrders, true);
                bool allowEdit = AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserEditOrders, false);

                string msg;
                if (isReadOnly)
                    msg = "الحساب الحالي بوضع قراءة فقط";
                else if (Mode == OrderMode.Add && !allowCreate)
                    msg = "الإنشاء متاح للمدير فقط";
                else
                    msg = "التعديل متاح للمدير فقط";

                MessageHelper.ShowWarning(msg);
            }
            catch
            {
                MessageHelper.ShowWarning("غير مصرح لك بالتعديل");
            }

            return false;
        }

        private void DisableOrderEditingUi()
        {
            try { btnSaveReceipt.Enabled = false; } catch { }
            try { btnRemove.Enabled = false; } catch { }
            try { btnRemoveAll.Enabled = false; } catch { }
            try { txtID.ReadOnly = true; } catch { }
            try { txtQty.ReadOnly = true; } catch { }
            try { txtPrice.ReadOnly = true; } catch { }
            try { txtPaid.ReadOnly = true; } catch { }
            try { txtDes.ReadOnly = true; } catch { }
            try { btnPickProduct.Enabled = false; } catch { }
            try { btnDisplayCustomers.Enabled = false; } catch { }
            try { if (cmbUnits != null) cmbUnits.Enabled = false; } catch { }
        }

        private void EnsureUnitsUi()
        {
            try
            {
                if (cmbUnits == null) return;

                cmbUnits.SelectedValueChanged -= cmbUnits_SelectedValueChanged;
                cmbUnits.SelectedValueChanged += cmbUnits_SelectedValueChanged;
            }
            catch
            {
            }
        }

        private void cmbUnits_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbUnits == null) return;

            try
            {
                _selectedProductUnit = cmbUnits.SelectedItem as ProductUnitLookup;
                if (_selectedProductUnit == null) return;

                txtPrice.Text = SalesNumberFormat.FormatPrice(_selectedProductUnit.SellPrice);

                if (SalesNumberFormat.TryParseDecimal(txtQty.Text, out decimal qtyUnit) && qtyUnit > 0)
                {
                    txtTotal.Text = SalesNumberFormat.FormatPrice(qtyUnit * _selectedProductUnit.SellPrice);
                }

                txtQty.Focus();
                txtQty.SelectAll();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Manager_Orders_KeyDown: ربط الضغط على Ctrl+S لحفظ الفاتورة
        /// المدخلات : sender/eventargs من WinForms
        /// المخرجات : لا يوجد
        /// التدفق   : UI(Ctrl+S) → btnSaveReceipt_Click
        /// الأخطاء  : لا يوجد
        /// </summary>
        private void Manager_Orders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;

            if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                try
                {
                    txtID.Focus();
                    txtID.SelectAll();
                }
                catch
                {
                }

                return;
            }

            if (e.KeyCode == Keys.F5)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                try
                {
                    _categoryNameById = _categoryRepo.GetAllCategories()
                        .Where(c => c != null)
                        .GroupBy(c => c.Id)
                        .ToDictionary(g => g.Key, g => g.First().Name ?? string.Empty);

                    SetStatus("تم تحديث الأصناف", isError: false, autoClearMs: 2000);
                }
                catch
                {
                    try { _categoryNameById = new Dictionary<int, string>(); } catch { }
                    SetStatus("فشل تحديث الأصناف", isError: true, autoClearMs: 2500);
                }

                try { txtID.Focus(); txtID.SelectAll(); } catch { }
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                CancelCurrentEntryOrEdit();
                return;
            }

            if (e.Control && e.KeyCode == Keys.Z)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                UndoLast();
                return;
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                if (!EnsureCanEditOrdersNowAndShowMessage()) return;

                try
                {
                    btnSaveReceipt_Click(btnSaveReceipt, EventArgs.Empty);
                }
                catch
                {
                }
            }
        }

        private void dataGridView_Product_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!EnsureCanEditOrdersNowAndShowMessage())
            {
                if (e != null) e.Cancel = true;
            }
        }

        /// <summary>
        /// SetDataSRC: تجهيز DataTable بأعمدة عربية وربطه بـ DataGridView لتمثيل تفاصيل الفاتورة على الشاشة
        /// المدخلات : لا يوجد
        /// المخرجات : لا يوجد
        /// التدفق   : Load → SetDataSRC → (DataTable dt) → dataGridView_Product.DataSource
        /// الأخطاء  : لا يوجد
        /// </summary>
        public void SetDataSRC()
        {
            if (dt.Columns.Count == 0)
            {
                // ── أسماء الأعمدة هنا هي التي نستخدمها لاحقاً في الحساب والتحويل إلى OrderDetail ──
                dt.Columns.Add("المعرف", typeof(int));
                dt.Columns.Add("المنتج", typeof(string));

                dt.Columns.Add("تفاصيل", typeof(string));

                dt.Columns.Add("الصنف", typeof(string));
               
                dt.Columns.Add("الكمية", typeof(decimal));
                dt.Columns.Add("السعر", typeof(decimal));
                dt.Columns.Add("المبلغ", typeof(decimal));

                // Hidden snapshot columns (unit-based sales)
                dt.Columns.Add("product_unit_id", typeof(int));
                dt.Columns.Add("unit_name", typeof(string));
                dt.Columns.Add("factor", typeof(decimal));
                dt.Columns.Add("qty_unit", typeof(decimal));
                dt.Columns.Add("base_qty", typeof(decimal));
                dt.Columns.Add("cost_price", typeof(decimal));
            }

            dataGridView_Product.AutoGenerateColumns = false;
            dataGridView_Product.DataSource = dt;
        }

        /// <summary>
        /// ResizeDGV: تنسيق عرض الأعمدة والأرقام داخل DataGridView لعرض الفاتورة بشكل مقروء
        /// المدخلات : لا يوجد
        /// المخرجات : لا يوجد
        /// التدفق   : Load → ResizeDGV → dataGridView_Product.Columns
        /// الأخطاء  : لا يوجد (مع حماية إذا لم تُنشأ الأعمدة بعد)
        /// </summary>
        public void ResizeDGV()
        {
            if (dataGridView_Product == null) return;
            if (!dataGridView_Product.IsHandleCreated) return;
            if (dataGridView_Product.Columns == null || dataGridView_Product.Columns.Count == 0) return;

            // تنسيقات رقمية
            try
            {
                if (dataGridView_Product.Columns.Contains("الكمية") && dataGridView_Product.Columns["الكمية"] != null)
                    dataGridView_Product.Columns["الكمية"].DefaultCellStyle.Format = "N0";
                if (dataGridView_Product.Columns.Contains("السعر") && dataGridView_Product.Columns["السعر"] != null)
                    dataGridView_Product.Columns["السعر"].DefaultCellStyle.Format = "N0";
                if (dataGridView_Product.Columns.Contains("المبلغ") && dataGridView_Product.Columns["المبلغ"] != null)
                    dataGridView_Product.Columns["المبلغ"].DefaultCellStyle.Format = "N0";

                if (dataGridView_Product.Columns.Contains("qty_unit") && dataGridView_Product.Columns["qty_unit"] != null)
                    dataGridView_Product.Columns["qty_unit"].DefaultCellStyle.Format = "N0";
                if (dataGridView_Product.Columns.Contains("base_qty") && dataGridView_Product.Columns["base_qty"] != null)
                    dataGridView_Product.Columns["base_qty"].DefaultCellStyle.Format = "N0";
            }
            catch
            {
            }

            // Hide snapshot columns
            try
            {
                string[] hidden = new[] { "product_unit_id", "unit_name", "factor", "qty_unit", "base_qty", "cost_price" };
                foreach (var c in hidden)
                {
                    if (dataGridView_Product.Columns.Contains(c) && dataGridView_Product.Columns[c] != null)
                        dataGridView_Product.Columns[c].Visible = false;
                }
            }
            catch
            {
            }

            // أحجام الأعمدة (مرنة حسب وجود الأعمدة)
            try
            {
                if (dataGridView_Product.Columns.Contains("المعرف") && dataGridView_Product.Columns["المعرف"] != null)
                    dataGridView_Product.Columns["المعرف"].Width = 100;
                if (dataGridView_Product.Columns.Contains("المنتج") && dataGridView_Product.Columns["المنتج"] != null)
                    dataGridView_Product.Columns["المنتج"].Width = 200;
                if (dataGridView_Product.Columns.Contains("تفاصيل") && dataGridView_Product.Columns["تفاصيل"] != null)
                    dataGridView_Product.Columns["تفاصيل"].Width = 250;

                if (dataGridView_Product.Columns.Contains("الصنف") && dataGridView_Product.Columns["الصنف"] != null)
                    dataGridView_Product.Columns["الصنف"].Width = 150;

                if (dataGridView_Product.Columns.Contains("الكمية") && dataGridView_Product.Columns["الكمية"] != null)
                    dataGridView_Product.Columns["الكمية"].Width = 90;
                if (dataGridView_Product.Columns.Contains("السعر") && dataGridView_Product.Columns["السعر"] != null)
                    dataGridView_Product.Columns["السعر"].Width = 100;
                if (dataGridView_Product.Columns.Contains("المبلغ") && dataGridView_Product.Columns["المبلغ"] != null)
                    dataGridView_Product.Columns["المبلغ"].Width = 120;
            }
            catch
            {
            }
        }

        // ===================== جلب بيانات المنتج =====================
        /// <summary>
        /// txtID_KeyDown: عند إدخال رقم المنتج والضغط Enter يتم جلب بيانات المنتج من ProductRepository وتعبئة الاسم/السعر
        /// المدخلات : sender/eventargs (KeyDown)
        /// المخرجات : لا يوجد
        /// التدفق   : UI(txtID) → ProductRepository.GetProductById → تعبئة txtLabel/txtPrice → نقل التركيز للكمية
        /// الأخطاء  : إدخال غير رقمي، منتج غير موجود، أخطاء مستودع المنتجات
        /// </summary>
        private void txtID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;
            e.Handled = true;

            if (!EnsureCanEditOrdersNowAndShowMessage()) return;

            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                SetStatus("أدخل رقم المنتج", isError: true, autoClearMs: 2500);
                try { txtID.Focus(); } catch { }
                return;
            }

            // التحقق من التكرار: منع إضافة نفس المنتج مرتين داخل نفس الفاتورة
            foreach (DataGridViewRow row in dataGridView_Product.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == txtID.Text)
                {
                    // إذا كنا في وضع تعديل سطر موجود، فالسماح بنفس المنتج لنفس السطر
                    if (_editingRowIndex >= 0 && row.Index == _editingRowIndex)
                        break;

                    SetStatus("هذا المنتج موجود مسبقًا", isError: true, autoClearMs: 3000);
                    txtID.Clear();
                    txtID.Focus();
                    return;
                }
            }

            try
            {
                int pId;
                if (!int.TryParse(txtID.Text, out pId)) return;

                // استخدام المخزن بدلاً من SQL المباشر
                var product = _productRepo.GetProductById(pId);

                if (product != null)
                {
                    // حفظ تفاصيل المنتج مؤقتاً لنضعها لاحقاً داخل سطر الجدول
                    _currentProductNote = product.Note ?? string.Empty;
                    try { txtNotProduct.Text = _currentProductNote; } catch { }

                    txtPrice.Text = product.Price.ToString("G29"); // تنسيق عادي

                    // عرض اسم الصنف داخل txtCategories (إن كان موجوداً)
                    string categoryNameForDisplay = string.Empty;
                    try
                    {
                        if (product.CategoryId.HasValue && _categoryNameById.TryGetValue(product.CategoryId.Value, out string catName))
                        {
                            txtCategories.Text = catName;
                            categoryNameForDisplay = catName;
                        }
                        else
                            txtCategories.Text = string.Empty;
                    }
                    catch
                    {
                        // تجاهل
                    }

                    // عرض اسم المنتج وبجواره اسم الصنف بين قوسين (حسب طلبك)
                    // تم الإبقاء على اسم المنتج فقط داخل txtLabel لتفادي تكرار الصنف داخل عمود المنتج في الجدول
                    txtLabel.Text = product.Label;

                    LoadProductUnitsForCurrentProduct(pId);

                    try
                    {
                        txtQty.Focus();
                        txtQty.SelectAll();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    SetStatus("المنتج غير موجود", isError: true, autoClearMs: 3000);
                    txtClear();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("حدث خطأ", ex.Message);
            }
        }

        private void LoadProductUnitsForCurrentProduct(int productId)
        {
            try
            {
                _currentProductUnits = _productRepo.GetProductUnits(productId) ?? new List<ProductUnitLookup>();

                if (cmbUnits == null)
                    return;

                cmbUnits.SelectedValueChanged -= cmbUnits_SelectedValueChanged;
                cmbUnits.DataSource = null;

                if (_currentProductUnits.Count == 0)
                {
                    cmbUnits.Items.Clear();
                    _selectedProductUnit = null;
                    txtPrice.Text = "0";
                }
                else
                {
                    cmbUnits.DataSource = _currentProductUnits;
                    cmbUnits.DisplayMember = nameof(ProductUnitLookup.UnitName);
                    cmbUnits.ValueMember = nameof(ProductUnitLookup.ProductUnitId);
                    cmbUnits.SelectedIndex = 0;
                    _selectedProductUnit = cmbUnits.SelectedItem as ProductUnitLookup;
                    if (_selectedProductUnit != null)
                        txtPrice.Text = SalesNumberFormat.FormatPrice(_selectedProductUnit.SellPrice);
                }

                cmbUnits.SelectedValueChanged += cmbUnits_SelectedValueChanged;
            }
            catch
            {
                _currentProductUnits = new List<ProductUnitLookup>();
                _selectedProductUnit = null;
                try
                {
                    if (cmbUnits != null)
                    {
                        cmbUnits.DataSource = null;
                        cmbUnits.Items.Clear();
                    }
                }
                catch { }
            }
        }

        // ===================== حفظ الفاتورة =====================
        /// <summary>
        /// btnSaveReceipt_Click: تحويل سطور DataTable إلى Order/OrderDetail ثم الحفظ (Add) أو التعديل (Edit) عبر OrderRepository
        /// المدخلات : sender/eventargs (Click)
        /// المخرجات : لا يوجد
        /// التدفق   : UI(DataTable dt) → بناء Order/Details → OrderRepository.SaveOrder/UpdateOrder → رسائل نجاح/خطأ
        /// الأخطاء  : سطور فارغة، تحويل أنواع (decimal/int)، أخطاء حفظ DB أو نقص مخزون (يظهر من OrderRepository)
        /// </summary>
        private void btnSaveReceipt_Click(object sender, EventArgs e)
        {
            if (!EnsureCanEditOrdersNowAndShowMessage()) return;

            var errors = new List<string>();

            if (dt.Rows.Count == 0)
            {
                errors.Add("لا توجد منتجات في الفاتورة");
            }

            try
            {
                // إعداد بيانات العميل والملاحظات
                int? custId = string.IsNullOrWhiteSpace(txtCustID.Text) ? (int?)null : Convert.ToInt32(txtCustID.Text);
                string note = string.IsNullOrWhiteSpace(txtDes.Text) ? "فاتورة مبيعات" : txtDes.Text;
                string createdBy = txtSaleMan.Text;

                // إعدادات النظام
                bool requireCustomer = AppSettingsManager.GetBool(AppSettingsManager.Keys.RequireCustomerInInvoice, false);
                bool allowPartialPayment = AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowPartialPayment, false);

                if (requireCustomer && !custId.HasValue)
                {
                    errors.Add("اختيار العميل إلزامي");
                }

                // حساب الإجمالي
                decimal total = 0;
                // قائمة التفاصيل
                List<OrderDetail> details = new List<OrderDetail>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var r = dt.Rows[i];
                    if (r == null || r.RowState == DataRowState.Deleted) continue;

                    decimal itemTotal = Convert.ToDecimal(r["المبلغ"]);
                    total += itemTotal;

                    decimal qtyUnit = 0m;
                    decimal baseQty = 0m;
                    int? productUnitId = null;
                    string unitName = string.Empty;
                    decimal? factor = null;
                    decimal? costPrice = null;

                    try { qtyUnit = Convert.ToDecimal(r["qty_unit"]); } catch { }
                    try { baseQty = Convert.ToDecimal(r["base_qty"]); } catch { }
                    try { productUnitId = Convert.ToInt32(r["product_unit_id"]); } catch { productUnitId = null; }
                    try { unitName = r["unit_name"] == DBNull.Value ? string.Empty : (r["unit_name"].ToString()); } catch { unitName = string.Empty; }
                    try { factor = Convert.ToDecimal(r["factor"]); } catch { factor = null; }
                    try { costPrice = Convert.ToDecimal(r["cost_price"]); } catch { costPrice = null; }

                    if (baseQty <= 0)
                    {
                        // Legacy fallback
                        baseQty = Convert.ToDecimal(r["الكمية"]);
                        qtyUnit = Convert.ToDecimal(r["الكمية"]);
                        factor = 1m;
                    }

                    details.Add(new OrderDetail
                    {
                        ProductId = Convert.ToInt32(r["المعرف"]),
                        ProductName = r["المنتج"]?.ToString(),
                        ProductNameSnapshot = r["المنتج"]?.ToString(),
                        Price = Convert.ToDecimal(r["السعر"]),
                        SellPriceSnapshot = Convert.ToDecimal(r["السعر"]),
                        Total = Convert.ToDecimal(r["المبلغ"]),
                        ProductUnitId = productUnitId,
                        UnitNameSnapshot = unitName,
                        FactorSnapshot = factor,
                        QtyUnit = qtyUnit,
                        BaseQty = baseQty,
                        CostPrice = costPrice
                    });
                }

                // تحقق منطقي من التفاصيل
                if (details.Count == 0)
                {
                    errors.Add("لا توجد تفاصيل صالحة داخل الفاتورة");
                }
                else
                {
                    for (int i = 0; i < details.Count; i++)
                    {
                        var d = details[i];
                        if (d == null) continue;
                        if (d.ProductId <= 0)
                            errors.Add("رقم المنتج غير صحيح في أحد السطور");
                        if (!d.BaseQty.HasValue || d.BaseQty.Value <= 0m)
                            errors.Add("الكمية يجب أن تكون أكبر من صفر في أحد السطور");
                        if (d.Price <= 0)
                            errors.Add("السعر يجب أن يكون أكبر من صفر في أحد السطور");
                    }
                }

                // ── الخصم: قيمة رقمية تُخصم من إجمالي الفاتورة (بدون تعديل DB schema) ──
                decimal discount = 0;
                try
                {
                    SalesNumberFormat.TryParsePrice((textBox11.Text ?? "0"), out discount);
                }
                catch
                {
                    discount = 0;
                }

                if (discount < 0) discount = 0;
                if (discount > total) discount = total;

                var invoiceTotals = OrderPricingHelper.Compute(total, discount);
                decimal netTotal = invoiceTotals.GrandTotal;

                // تجهيز كائن الفاتورة
                if (Mode == OrderMode.Add && string.IsNullOrWhiteSpace(_draftRequestId))
                    _draftRequestId = Guid.NewGuid().ToString();

                Order order = new Order
                {
                    Id = Mode == OrderMode.Edit ? idOrder : 0,
                    RequestId = Mode == OrderMode.Add ? _draftRequestId : null,
                    OrderDate = Mode == OrderMode.Edit ? dtOrder.Value : DateTime.Now,
                    CustomerId = custId,
                    Note = note,
                    Discount = invoiceTotals.Discount,
                    TaxAmount = invoiceTotals.TaxAmount,
                    Total = netTotal,
                    CreatedBy = createdBy,
                    Details = details
                };

                // الحفظ أو التعديل عبر المخزن
                decimal paidAmount = 0;
                bool paidHasError = false;

                if (string.IsNullOrWhiteSpace(txtPaid.Text))
                {
                    errors.Add("لم تقم بإدخال قيمة للمدفوع");
                    paidHasError = true;
                }
                else if (!SalesNumberFormat.TryParsePrice(txtPaid.Text, out paidAmount))
                {
                    errors.Add("قيمة المدفوع غير صحيحة");
                    paidHasError = true;
                }

                ValidationHelper.ValidateNonNegativeNumber(paidAmount, "المدفوع", errors);

                if (Mode == OrderMode.Add && !paidHasError)
                {
                    // شرط: إذا الدفع الجزئي غير مسموح → يجب أن يكون المدفوع = الصافي
                    if (!allowPartialPayment && paidAmount != netTotal)
                    {
                        errors.Add("يجب أن يكون المدفوع مساويًا للصافي");
                        paidHasError = true;
                    }

                    // إذا الدفع الجزئي مسموح: نكتفي بمنع المدفوع أكبر من الصافي
                    if (allowPartialPayment && paidAmount > netTotal)
                    {
                        errors.Add("لا يمكن أن يكون المدفوع أكبر من الصافي");
                        paidHasError = true;
                    }
                }

                if (errors.Count > 0)
                {
                    MessageHelper.ShowValidationErrors(errors);
                    if (paidHasError)
                    {
                        txtPaid.Focus();
                        txtPaid.SelectAll();
                    }
                    return;
                }
                // في وضع التعديل: المدفوع للعرض (يتم تحميله من Payments)

                if (Mode == OrderMode.Add)
                {
                    _orderRepo.SaveOrder(order, paidAmount);
                    MessageHelper.ShowSuccess("تم حفظ الفاتورة بنجاح");
                }
                else
                {
                    _orderRepo.UpdateOrder(order, paidAmount, _FullName);
                    MessageHelper.ShowSuccess("تم تعديل الفاتورة بنجاح");
                    Close();
                }

                if (Mode == OrderMode.Add)
                {
                    dt.Rows.Clear();
                    txtClear();
                    _draftRequestId = null;
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ أثناء الحفظ", ex.Message);
            }
        }

        // ===================== حساب المتبقي =====================
        /// <summary>
        /// txtPaid_TextChanged: عند تغيير قيمة المدفوع يتم إعادة حساب المتبقي
        /// المدخلات : sender/eventargs
        /// المخرجات : لا يوجد
        /// التدفق   : txtPaid تغيير → CalculAmount → تحديث txtRest
        /// الأخطاء  : لا يوجد (CalculAmount محمية بـ try/catch)
        /// </summary>
        private void txtPaid_TextChanged(object sender, EventArgs e)
        {
            CalculAmount();
        }

        // ===================== دوال مساعدة =====================
        /// <summary>
        /// CalculAmount: حساب إجمالي السطر الحالي (للعرض) + إجمالي الفاتورة + المتبقي بعد خصم المدفوع
        /// المدخلات : لا يوجد
        /// المخرجات : لا يوجد
        /// التدفق   : UI(txtQty/txtPrice/dataGridView_Product/txtPaid) → حسابات → txtTotal/txtRest
        /// الأخطاء  : تحويل أرقام/تنسيق، لذلك الكود محاط بـ try/catch لتجنب كسر الشاشة
        /// </summary>
        void CalculAmount()
        {
            try
            {
                // 1. حساب إجمالي السطر الحالي (إذا كان التركيز عليه)
                if (!string.IsNullOrWhiteSpace(txtQty.Text) && !string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    if (SalesNumberFormat.TryParseDecimal(txtQty.Text, out decimal qty) &&
                        SalesNumberFormat.TryParsePrice(txtPrice.Text, out decimal price))
                    {
                        txtTotal.Text = SalesNumberFormat.FormatPrice(qty * price);
                    }
                }

                // 2. حساب إجمالي الفاتورة من الجدول (للمتبقي)
                decimal totalInvoice = 0;
                foreach (DataGridViewRow row in dataGridView_Product.Rows)
                {
                    if (row.Cells["المبلغ"].Value != null)
                    {
                        totalInvoice += Convert.ToDecimal(row.Cells["المبلغ"].Value);
                    }
                }
                // إضافة السطر الحالي إذا لم يُضف بعد (اختياري، لكن الأسلم الاعتماد على الجدول فقط)

                // 2.1 الخصم (قيمة)
                decimal discount = 0;
                try
                {
                    SalesNumberFormat.TryParsePrice((textBox11.Text ?? "0"), out discount);
                }
                catch
                {
                    discount = 0;
                }
                if (discount < 0) discount = 0;
                if (discount > totalInvoice) discount = totalInvoice;

                var pricing = OrderPricingHelper.Compute(totalInvoice, discount);
                decimal net = pricing.GrandTotal;
                try
                {
                    textBox12.Text = SalesNumberFormat.FormatPrice(net);
                }
                catch
                {
                    // تجاهل
                }

                // 3. حساب المتبقي
                decimal paid = 0;
                if (!string.IsNullOrWhiteSpace(txtPaid.Text))
                {
                    SalesNumberFormat.TryParsePrice(txtPaid.Text, out paid);
                }

                decimal rest = net - paid;
                txtRest.Text = SalesNumberFormat.FormatPrice(rest);
            }
            catch { }
        }

        /// <summary>
        /// txtClear: تصفير حقول إدخال المنتج الحالي (لا يمس جدول الفاتورة)
        /// المدخلات : لا يوجد
        /// المخرجات : لا يوجد
        /// التدفق   : بعد إضافة سطر/فشل بحث → txtClear → جاهزية لإدخال منتج جديد
        /// </summary>
        void txtClear()
        {
            txtID.Clear();
            txtLabel.Clear();
            txtQty.Clear();
            txtPrice.Clear();
            txtTotal.Clear();
            txtCategories.Clear();
            try { txtNotProduct.Clear(); } catch { }
            _currentProductNote = string.Empty;
            _editingRowIndex = -1;

            _selectedProductUnit = null;
            try
            {
                if (cmbUnits != null)
                    cmbUnits.SelectedIndex = -1;
            }
            catch { }
        }

        // ===================== إضافة للجدول (UI) =====================
        /// <summary>
        /// txtQty_KeyDown: عند الضغط Enter بعد إدخال الكمية يتم إنشاء DataRow وإضافته لجدول الفاتورة ثم تحديث المتبقي
        /// المدخلات : sender/eventargs (KeyDown)
        /// المخرجات : لا يوجد
        /// التدفق   : UI(txtID/txtQty/txtPrice) → dt.Rows.Add → dataGridView_Product → CalculAmount
        /// الأخطاء  : تحويل أرقام (decimal.Parse) إذا كانت القيم غير صالحة
        /// </summary>
        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                if (!EnsureCanEditOrdersNowAndShowMessage()) return;

                if (!IsProductLoadedForEntry)
                {
                    SetStatus("اختر منتج أولاً", isError: true, autoClearMs: 2500);
                    try { txtID.Focus(); } catch { }
                    return;
                }

                if (!TryGetEntryQty(out decimal q) || q <= 0m)
                {
                    SetStatus("أدخل كمية صحيحة", isError: true, autoClearMs: 2500);
                    try { txtQty.Focus(); txtQty.SelectAll(); } catch { }
                    return;
                }

                bool priceEditable = false;
                try { priceEditable = (txtPrice != null && !txtPrice.ReadOnly); } catch { priceEditable = false; }

                if (priceEditable)
                {
                    try
                    {
                        txtPrice.Focus();
                        txtPrice.SelectAll();
                    }
                    catch
                    {
                    }
                    return;
                }

                TryAddOrUpdateLineFromEntry(invokedFromPrice: false);
            }

            if (e.KeyCode == Keys.Right) txtID.Focus();
        }

        private void txtPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;
            e.Handled = true;

            TryAddOrUpdateLineFromEntry(invokedFromPrice: true);
        }

        public void SetEditPaidAmount(decimal paidAmount)
        {
            try
            {
                txtPaid.Text = paidAmount.ToString("G29");
            }
            catch
            {
            }

            CalculAmount();
        }

        // ===================== الحذف من الجدول =====================
        /// <summary>
        /// btnRemove_Click: حذف السطر المحدد من جدول تفاصيل الفاتورة ثم تحديث المتبقي
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (!EnsureCanEditOrdersNowAndShowMessage()) return;

            if (dataGridView_Product.CurrentRow != null && !dataGridView_Product.CurrentRow.IsNewRow)
            {
                PushUndoState("حذف سطر");
                dataGridView_Product.Rows.RemoveAt(dataGridView_Product.CurrentRow.Index);
                CalculAmount(); // تحديث المتبقي بعد الحذف
                SetStatus("تم حذف السطر", isError: false, autoClearMs: 1500);
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            if (!EnsureCanEditOrdersNowAndShowMessage()) return;

            if (dt != null && dt.Rows.Count > 0)
                PushUndoState("حذف جميع السطور");
            dt.Rows.Clear();
            CalculAmount(); // تحديث المتبقي
            SetStatus("تم حذف كل السطور", isError: false, autoClearMs: 1500);
        }

        // ===================== الأحداث الأخرى =====================
        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            // حساب سطر
            if (SalesNumberFormat.TryParseDecimal(txtQty.Text, out decimal qty) &&
                SalesNumberFormat.TryParsePrice(txtPrice.Text, out decimal price))
            {
                txtTotal.Text = SalesNumberFormat.FormatPrice(qty * price);
            }
        }
        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            // حساب سطر
            if (SalesNumberFormat.TryParseDecimal(txtQty.Text, out decimal qty) &&
                SalesNumberFormat.TryParsePrice(txtPrice.Text, out decimal price))
            {
                txtTotal.Text = SalesNumberFormat.FormatPrice(qty * price);
            }
        }
        private void btnClose_Click(object sender, EventArgs e) => Close();
        private void Manager_Orders_Shown(object sender, EventArgs e) => txtID.Focus();

        private void btnPickProduct_Click(object sender, EventArgs e)
        {
            if (!EnsureCanEditOrdersNowAndShowMessage()) return;

            using (var frm = new ProductPickerForm())
            {
                if (frm.ShowDialog(this) == DialogResult.OK && frm.SelectedProductId.HasValue)
                {
                    txtID.Text = frm.SelectedProductId.Value.ToString();

                    // Trigger the same flow as pressing Enter in txtID
                    txtID_KeyDown(txtID, new KeyEventArgs(Keys.Enter));
                }
            }
        }

        private void btnDisplayCustomers_Click(object sender, EventArgs e)
        {
            // ── فتح شاشة اختيار العملاء وإرجاع بيانات العميل المختار (DialogResult.OK) ──
            using (Customers_List frm = new Customers_List())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtCustID.Text = frm.SelectedCustomerID;
                    txtCustFullName.Text = frm.SelectedCustomerName;
                    txtTel.Text = frm.SelectedCustomerTel;
                }
            }
        }

        // للدعم الفني (تعبئة بيانات للتعديل من الخارج)
        /// <summary>
        /// TextOrders: ضبط عنوان الفورم ونص زر الحفظ (يُستخدم عادةً عند فتح الشاشة بوضع التعديل)
        /// </summary>
        public void TextOrders(string TextForm, string TxtBtnSave)
        {
            Text = TextForm;
            btnSaveReceipt.Text = TxtBtnSave;
        }

        /// <summary>
        /// SetEditDataOrders: تعبئة بيانات رأس الفاتورة عند الدخول بوضع التعديل (بدون تفاصيل المنتجات)
        /// المدخلات : تاريخ الفاتورة + وصف + اسم البائع + بيانات العميل
        /// المخرجات : لا يوجد
        /// التدفق   : شاشة البحث/الإدارة → SetEditDataOrders → تعبئة عناصر UI
        /// </summary>
        public void SetEditDataOrders(DateTime date, string textDes, string textSaleMan,
            int customerID, string customerFullName, string customerTel, decimal discount = 0m)
        {
            dtOrder.Value = date;
            if (discount <= 0m)
                discount = OrderDiscountHelper.ExtractDiscount(textDes);
            try
            {
                textBox11.Text = SalesNumberFormat.FormatPrice(discount);
            }
            catch
            {
            }

            txtDes.Text = OrderDiscountHelper.RemoveDiscount(textDes);
            txtSaleMan.Text = textSaleMan;
            txtCustID.Text = customerID == 0 ? "" : customerID.ToString();
            txtCustFullName.Text = customerFullName;
            txtTel.Text = customerTel;
        }

        private decimal TryExtractDiscount(string noteText)
        {
            return OrderDiscountHelper.ExtractDiscount(noteText);
        }

        private string StripDiscountTag(string noteText)
        {
            return OrderDiscountHelper.RemoveDiscount(noteText);
        }

        private void dataGridView_Product_DoubleClick(object sender, EventArgs e)
        {
            if (!EnsureCanEditOrdersNowAndShowMessage()) return;

            // منطق استرجاع الصف للتعديل
            if (dataGridView_Product.CurrentRow == null || dataGridView_Product.CurrentRow.IsNewRow) return;

            try
            {
                var row = dataGridView_Product.CurrentRow;

                // تفعيل وضع تعديل السطر الحالي
                _editingRowIndex = row.Index;

                txtID.Text = row.Cells["المعرف"].Value?.ToString() ?? string.Empty;
                txtLabel.Text = row.Cells["المنتج"].Value?.ToString() ?? string.Empty;

                if (row.Cells["الصنف"] != null)
                    txtCategories.Text = row.Cells["الصنف"].Value?.ToString() ?? string.Empty;

                if (row.Cells["تفاصيل"] != null)
                    _currentProductNote = row.Cells["تفاصيل"].Value?.ToString() ?? string.Empty;
                try { txtNotProduct.Text = _currentProductNote; } catch { }

                EnsureUnitsUi();

                int pid;
                if (int.TryParse((txtID.Text ?? string.Empty).Trim(), out pid) && pid > 0)
                {
                    LoadProductUnitsForCurrentProduct(pid);

                    int puid;
                    object puidObj = null;
                    try { puidObj = row.Cells["product_unit_id"].Value; } catch { puidObj = null; }
                    if (puidObj != null && int.TryParse(puidObj.ToString(), out puid))
                    {
                        try
                        {
                            if (cmbUnits != null && cmbUnits.DataSource != null)
                            {
                                for (int i = 0; i < cmbUnits.Items.Count; i++)
                                {
                                    var item = cmbUnits.Items[i] as ProductUnitLookup;
                                    if (item != null && item.ProductUnitId == puid)
                                    {
                                        cmbUnits.SelectedIndex = i;
                                        _selectedProductUnit = item;
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

                // Prefer qty_unit if present
                object qtyUnitObj = null;
                try { qtyUnitObj = row.Cells["qty_unit"].Value; } catch { qtyUnitObj = null; }
                if (qtyUnitObj != null)
                    txtQty.Text = qtyUnitObj.ToString();
                else
                    txtQty.Text = row.Cells["الكمية"].Value?.ToString() ?? string.Empty;

                // Price: if we have a selected unit, keep it consistent
                if (_selectedProductUnit != null)
                    txtPrice.Text = SalesNumberFormat.FormatPrice(_selectedProductUnit.SellPrice);
                else if (row.Cells["السعر"].Value != null && SalesNumberFormat.TryParsePrice(row.Cells["السعر"].Value.ToString(), out decimal rowPrice))
                    txtPrice.Text = SalesNumberFormat.FormatPrice(rowPrice);
                else
                    txtPrice.Text = SalesNumberFormat.ToEnglishDigits(row.Cells["السعر"].Value?.ToString() ?? string.Empty);

                if (row.Cells["المبلغ"].Value != null && SalesNumberFormat.TryParsePrice(row.Cells["المبلغ"].Value.ToString(), out decimal lineTotal))
                    txtTotal.Text = SalesNumberFormat.FormatPrice(lineTotal);
                else
                    txtTotal.Text = SalesNumberFormat.ToEnglishDigits(row.Cells["المبلغ"].Value?.ToString() ?? string.Empty);

                txtQty.Focus();
            }
            catch
            {
            }
        }

       
        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void txtCategories_TextChanged(object sender, EventArgs e)
        {

        }



        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال/الأحداث : (واجهة)                 ║
        // ║  الجداول           : Orders, Order_Details    ║
        // ║  يستدعيه           : main.cs / Search_Manager_Orders.cs ║
        // ║  يستدعي            : ProductRepository.cs + OrderRepository.cs + Customers_List.cs ║
        // ╚══════════════════════════════════════════════╝
    }
}
