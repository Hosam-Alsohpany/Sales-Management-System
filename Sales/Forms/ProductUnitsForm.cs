// ============================================================
// ProductUnitsForm.cs
// إدارة وحدات المنتج - هرمية خطية مع cascade تلقائي
//
// القواعد المطبقة:
//  1. factor = ReadOnly دائماً (محسوب من parentFactor × packSize)
//  2. تحديث factor مرئي فوري عند تعديل pack_size (مثل Excel)
//  3. الحفظ في DB فقط عند الضغط على "حفظ"
//  4. كشف الحلقات (Cycles) بـ HashSet<int> الحقيقي
//  5. إضافة وحدة دائماً كآخر مستوى في السلسلة
//  6. تحذير (لا منع) عند تعديل منتج له حركات تاريخية
// ============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class ProductUnitsForm : Form
    {
        // ─── Repos ────────────────────────────────────────────────────
        private readonly ProductUnitsRepository          _repo    = new ProductUnitsRepository();
        private readonly ProductRepository               _productRepo = new ProductRepository();
        private readonly ProductUnitTransactionsRepository _txRepo = new ProductUnitTransactionsRepository();

        // ─── State ───────────────────────────────────────────────────
        private int?      _selectedProductId;
        private DataTable _editTable;        // نسخة العمل (in-memory)
        private DataView  _editView;         // مرتبط بـ DataGridView
        private bool      _isDirty;          // تغييرات غير محفوظة
        private bool      _isRecalculating;  // منع التكرار أثناء cascade
        private bool      _productHasTx;     // هل للمنتج حركات تاريخية

        // ─── الصفوف المرتبة هرمياً (مخبأة للـ cascade) ──────────────
        private List<DataRow> _hierarchyRows = new List<DataRow>();

        // ════════════════════════════════════════════════════════════
        // Constructors
        // ════════════════════════════════════════════════════════════
        public ProductUnitsForm()
        {
            InitializeComponent();
            try
            {
                UiTheme.ApplyToForm(this);
                SalesUiBootstrap.WireForm(this);
            }
            catch { }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgvProductUnits, "وحدات المنتج", "product_units.pdf", txtSearch?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgvProductUnits, "وحدات المنتج", txtSearch?.Text);
        }

        public ProductUnitsForm(int productId) : this()
        {
            _selectedProductId = productId;
        }

        // ════════════════════════════════════════════════════════════
        // Load
        // ════════════════════════════════════════════════════════════
        private void ProductUnitsForm_Load(object sender, EventArgs e)
        {
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "وحدات المنتج والباركود"))
                return;

            try
            {
                LoadProductInfo();
                LoadUnitsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء التحميل: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // اختيار منتج
        // ════════════════════════════════════════════════════════════
        private void btnPickProduct_Click(object sender, EventArgs e)
        {
            if (_isDirty &&
                MessageBox.Show("يوجد تغييرات غير محفوظة. هل تريد تجاهلها؟",
                    "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                string user = string.IsNullOrWhiteSpace(SessionManager.CurrentUsername)
                    ? "user" : SessionManager.CurrentUsername;
                using (Manager_Products frm = new Manager_Products(user, productPickerMode: true))
                {
                    if (frm.ShowDialog() == DialogResult.OK && frm.PickedProduct != null)
                    {
                        _selectedProductId = frm.PickedProduct.Id;
                        _isDirty = false;
                        LoadProductInfo();
                        LoadUnitsGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في اختيار المنتج: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // قاموس الوحدات
        // ════════════════════════════════════════════════════════════
        private void btnUnitsMaster_Click(object sender, EventArgs e)
        {
            using (UnitsForm f = new UnitsForm())
                f.ShowDialog(this);
        }

        // ════════════════════════════════════════════════════════════
        // تحميل بيانات المنتج
        // ════════════════════════════════════════════════════════════
        private void LoadProductInfo()
        {
            txtProductId.Text        = string.Empty;
            txtProductName.Text      = string.Empty;
            txtProductCode.Text      = string.Empty;
            txtProductBarcode.Text   = string.Empty;
            txtProductBaseUnit.Text  = string.Empty;
            txtProductBasePrice.Text = string.Empty;
            DisposePictureBoxImage(picProduct);
            _productHasTx = false;
            tsLblLock.Visible = false;

            if (!_selectedProductId.HasValue) return;

            try
            {
                var product = _productRepo.GetProductById(_selectedProductId.Value);
                if (product == null) return;

                txtProductId.Text      = product.Id.ToString();
                txtProductName.Text    = product.Label ?? string.Empty;
                txtProductCode.Text    = product.Sku ?? string.Empty;
                txtProductBarcode.Text = product.DefaultBarcode ?? string.Empty;

                try
                {
                    DisposePictureBoxImage(picProduct);
                    if (!string.IsNullOrEmpty(product.ImagePath) &&
                        System.IO.File.Exists(product.ImagePath))
                        picProduct.Image = Image.FromFile(product.ImagePath);
                }
                catch { DisposePictureBoxImage(picProduct); }

                // فحص الحركات التاريخية
                _productHasTx = _txRepo.HasAnyTransactions(_selectedProductId.Value);
                tsLblLock.Visible = _productHasTx;

                string pName = string.IsNullOrWhiteSpace(product.Label) ? "منتج" : product.Label;
                Text = "إدارة وحدات المنتج — " + pName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تحميل بيانات المنتج: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // تحميل جدول الوحدات مع بناء الهرمية
        // ════════════════════════════════════════════════════════════
        private void LoadUnitsGrid()
        {
            dgvProductUnits.DataSource = null;
            _editTable = null;
            _editView  = null;
            _hierarchyRows.Clear();
            SetDirty(false);

            if (!_selectedProductId.HasValue) return;

            try
            {
                DataTable raw = _repo.GetProductUnits(_selectedProductId.Value);

                // ── بناء الهرمية الخطية بكشف Cycles ──────────────────
                _hierarchyRows = BuildLinearHierarchy(raw);

                // ── بناء جدول العمل مع عمود level_number ─────────────
                _editTable = BuildEditTable(_hierarchyRows);
                _editView  = _editTable.DefaultView;

                dgvProductUnits.AutoGenerateColumns = false;
                dgvProductUnits.DataSource = _editView;

                // ── تحديث بطاقة المنتج العلوية ────────────────────────
                UpdateProductBaseUnitDisplay();
                UpdateConversionPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تحميل الوحدات: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // خوارزمية بناء الهرمية الخطية - HashSet cycle detection
        // ════════════════════════════════════════════════════════════
        private List<DataRow> BuildLinearHierarchy(DataTable raw)
        {
            if (raw == null || raw.Rows.Count == 0)
                return new List<DataRow>();

            var result  = new List<DataRow>();
            var visited = new HashSet<int>(); // كشف الحلقات الحقيقي

            // ── الجذر: الصف الذي ليس له parent ─────────────────────
            DataRow root = raw.AsEnumerable()
                .FirstOrDefault(r => r["parent_product_unit_id"] == DBNull.Value);

            if (root == null)
            {
                // لا يوجد جذر واضح → رتّب بـ display_order
                return raw.AsEnumerable()
                    .OrderBy(r => r["display_order"] == DBNull.Value
                                ? 0 : Convert.ToInt32(r["display_order"]))
                    .ToList();
            }

            // ── سير في السلسلة: root → child → grandchild → ... ────
            DataRow current = root;
            while (current != null)
            {
                int currentId = Convert.ToInt32(current["id"]);

                // كشف الحلقة - HashSet حقيقي وليس maxDepth
                if (visited.Contains(currentId))
                    throw new InvalidOperationException(
                        "تم اكتشاف دورة (Cycle) في هيكل الوحدات.\n" +
                        "تحقق من بيانات parent_product_unit_id في قاعدة البيانات.");

                visited.Add(currentId);
                result.Add(current);

                // البحث عن الابن الوحيد (linear chain)
                current = raw.AsEnumerable()
                    .FirstOrDefault(r =>
                        r["parent_product_unit_id"] != DBNull.Value &&
                        Convert.ToInt32(r["parent_product_unit_id"]) == currentId &&
                        !visited.Contains(Convert.ToInt32(r["id"])));
            }

            // ── إضافة أي صفوف يتيمة (أُعيد ترتيبها خارج السلسلة) ───
            foreach (DataRow r in raw.Rows)
            {
                int id = Convert.ToInt32(r["id"]);
                if (!visited.Contains(id))
                    result.Add(r);
            }

            return result;
        }

        // ════════════════════════════════════════════════════════════
        // بناء جدول العمل من الصفوف المرتبة (يضيف level_number)
        // ════════════════════════════════════════════════════════════
        private DataTable BuildEditTable(List<DataRow> rows)
        {
            DataTable dt = new DataTable();

            // أعمدة البيانات من DB
            foreach (DataColumn col in rows[0].Table.Columns)
                dt.Columns.Add(col.ColumnName, col.DataType);

            // عمود إضافي للمستوى
            if (!dt.Columns.Contains("level_number"))
                dt.Columns.Add("level_number", typeof(int));

            int level = 1;
            foreach (DataRow src in rows)
            {
                DataRow newRow = dt.NewRow();
                foreach (DataColumn col in src.Table.Columns)
                    newRow[col.ColumnName] = src[col.ColumnName];
                newRow["level_number"] = level++;
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        // ════════════════════════════════════════════════════════════
        // تعديل خلية → cascade فوري (مرئي) - لا حفظ في DB هنا
        // ════════════════════════════════════════════════════════════
        private void dgvProductUnits_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _isRecalculating) return;

            string colName = dgvProductUnits.Columns[e.ColumnIndex].Name;

            if (colName == colPackSize.Name)
            {
                // flush الـ edit إلى DataRow أولاً
                (dgvProductUnits.Rows[e.RowIndex].DataBoundItem as DataRowView)?.EndEdit();
                RecalculateCascade(e.RowIndex);
            }

            // جميع الأعمدة القابلة للتعديل تُشغّل dirty flag
            if (colName == colPackSize.Name   ||
                colName == colSellPrice.Name  ||
                colName == colCostPrice.Name)
            {
                (dgvProductUnits.Rows[e.RowIndex].DataBoundItem as DataRowView)?.EndEdit();
                SetDirty(true);
            }
        }

        // ════════════════════════════════════════════════════════════
        // Cascade recalculation - يبدأ من rowIndex وينزل لكل الأبناء
        // factor = ReadOnly: يُحسب فقط من parentFactor × packSize
        // ════════════════════════════════════════════════════════════
        private void RecalculateCascade(int dgvRowIndex)
        {
            if (_editTable == null || _isRecalculating) return;

            _isRecalculating = true;
            try
            {
                // الصف الذي تغير (0-indexed في _editTable)
                // dgvRowIndex يتطابق مع ترتيب الـ editTable لأننا لا نستخدم filter
                int startIndex = dgvRowIndex;
                int rowCount   = _editTable.Rows.Count;

                for (int i = startIndex; i < rowCount; i++)
                {
                    DataRow row = _editTable.Rows[i];

                    if (i == 0)
                    {
                        // الوحدة الأساسية: factor دائماً = 1
                        row["factor"] = 1m;
                        continue;
                    }

                    DataRow parent = _editTable.Rows[i - 1];
                    decimal parentFactor = parent["factor"] == DBNull.Value
                        ? 1m : Convert.ToDecimal(parent["factor"]);

                    decimal packSize = row["pack_size"] == DBNull.Value
                        ? 1m : Convert.ToDecimal(row["pack_size"]);
                    if (packSize <= 0m) packSize = 1m;

                    row["factor"] = Math.Round(parentFactor * packSize, 6);
                }

                // تحديث معاينة الجانب الأيمن
                UpdateConversionPreview();
                UpdateProductBaseUnitDisplay();
            }
            finally
            {
                _isRecalculating = false;
            }
        }

        // ════════════════════════════════════════════════════════════
        // حفظ كل الصفوف في DB (يُنفَّذ فقط عند الضغط على "حفظ")
        // ════════════════════════════════════════════════════════════
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!_isDirty) return;

            // flush أي تعديل معلّق
            dgvProductUnits.EndEdit();
            _editTable?.AcceptChanges();

            if (_editTable == null || _editTable.Rows.Count == 0)
            {
                SetDirty(false);
                return;
            }

            // تحذير عند وجود حركات تاريخية
            if (_productHasTx &&
                MessageBox.Show(
                    "هذا المنتج له حركات بيع/شراء سابقة.\n" +
                    "الفواتير القديمة محفوظة بـ Snapshot خاص بها ولن تتأثر.\n\n" +
                    "هل تريد المتابعة في الحفظ؟",
                    "تنبيه - حركات تاريخية",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {
                int order = 10;
                DataRow prevRow = null;

                foreach (DataRow row in _editTable.Rows)
                {
                    int     id       = Convert.ToInt32(row["id"]);
                    int     unitId   = Convert.ToInt32(row["unit_id"]);
                    decimal factor   = row["factor"]     == DBNull.Value ? 1m : Convert.ToDecimal(row["factor"]);
                    decimal sell     = row["sell_price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["sell_price"]);
                    decimal cost     = row["cost_price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["cost_price"]);
                    decimal packSize = row["pack_size"]  == DBNull.Value ? 1m : Convert.ToDecimal(row["pack_size"]);

                    int?    parentId = prevRow == null ? (int?)null
                                      : Convert.ToInt32(prevRow["id"]);
                    decimal? pack    = prevRow == null ? (decimal?)null : packSize;

                    _repo.UpdateProductUnit(id, unitId, factor, sell, cost, order, parentId, pack);
                    order += 10;
                    prevRow = row;
                }

                SetDirty(false);
                LoadUnitsGrid(); // إعادة تحميل من DB للتأكيد
                MessageBox.Show("تم الحفظ بنجاح.", "حفظ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في الحفظ: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // إضافة وحدة - دائماً كآخر مستوى في السلسلة
        // ════════════════════════════════════════════════════════════
        private void btnAddUnit_Click(object sender, EventArgs e)
        {
            if (!_selectedProductId.HasValue)
            {
                MessageBox.Show("اختر منتجاً أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // الوحدات الموجودة حالياً
                HashSet<int> existingIds = new HashSet<int>();
                if (_editTable != null)
                    foreach (DataRow r in _editTable.Rows)
                        existingIds.Add(Convert.ToInt32(r["unit_id"]));

                DataTable masterUnits = _repo.GetAllUnits();
                List<KeyValuePair<int, string>> choices = new List<KeyValuePair<int, string>>();
                foreach (DataRow r in masterUnits.Rows)
                {
                    int uid = Convert.ToInt32(r["id"]);
                    if (!existingIds.Contains(uid))
                        choices.Add(new KeyValuePair<int, string>(uid, r["name"]?.ToString() ?? ""));
                }

                if (choices.Count == 0)
                {
                    MessageBox.Show(
                        "كل الوحدات المتاحة في قاموس الوحدات مُستخدمة بالفعل.\n" +
                        "اضغط «قاموس الوحدات» لإضافة وحدة جديدة.",
                        "لا توجد وحدات إضافية",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // الوحدة الأخيرة في السلسلة تكون الأب للوحدة الجديدة
                DataRow lastRow = _editTable != null && _editTable.Rows.Count > 0
                    ? _editTable.Rows[_editTable.Rows.Count - 1] : null;

                decimal lastFactor   = lastRow == null ? 1m
                    : (lastRow["factor"] == DBNull.Value ? 1m : Convert.ToDecimal(lastRow["factor"]));
                int?    parentId     = lastRow == null ? (int?)null : Convert.ToInt32(lastRow["id"]);
                int     newOrder     = (_editTable?.Rows.Count ?? 0) * 10 + 10;
                int     newLevel     = (_editTable?.Rows.Count ?? 0) + 1;

                using (AddProductUnitPickDialog dlg = new AddProductUnitPickDialog(choices, lastFactor, newLevel))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;

                    decimal packSize  = dlg.PackSize;
                    if (packSize <= 0m) packSize = 1m;
                    decimal newFactor = Math.Round(lastFactor * packSize, 6);

                    int newId = _repo.AddProductUnit(
                        _selectedProductId.Value,
                        dlg.SelectedUnitId,
                        newFactor,
                        dlg.SellPrice,
                        dlg.CostPrice,
                        newOrder,
                        parentId,
                        parentId.HasValue ? packSize : (decimal?)null);

                    SetDirty(false); // تم الحفظ مباشرة في AddProductUnit
                    LoadProductInfo();
                    LoadUnitsGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في إضافة الوحدة: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // تغيير الوحدة للمستوى المحدد (تغيير المسمى فقط بدون تغيير المعامل)
        // ════════════════════════════════════════════════════════════
        private void btnChangeUnit_Click(object sender, EventArgs e)
        {
            if (dgvProductUnits.CurrentRow == null)
            {
                MessageBox.Show("اختر صف وحدة من الجدول أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataRowView drv = dgvProductUnits.CurrentRow.DataBoundItem as DataRowView;
                if (drv == null) return;

                int currentUnitId = Convert.ToInt32(drv["unit_id"]);

                // الوحدات الموجودة حالياً (باستثناء الوحدة الحالية)
                HashSet<int> existingIds = new HashSet<int>();
                if (_editTable != null)
                {
                    foreach (DataRow r in _editTable.Rows)
                    {
                        int uId = Convert.ToInt32(r["unit_id"]);
                        if (uId != currentUnitId) existingIds.Add(uId);
                    }
                }

                DataTable masterUnits = _repo.GetAllUnits();
                List<KeyValuePair<int, string>> choices = new List<KeyValuePair<int, string>>();
                foreach (DataRow r in masterUnits.Rows)
                {
                    int uid = Convert.ToInt32(r["id"]);
                    if (!existingIds.Contains(uid))
                        choices.Add(new KeyValuePair<int, string>(uid, r["name"]?.ToString() ?? ""));
                }

                if (choices.Count == 0)
                {
                    MessageBox.Show("لا توجد وحدات أخرى متاحة في القاموس.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int level = drv["level_number"] == DBNull.Value ? 0 : Convert.ToInt32(drv["level_number"]);
                string oldName = drv["unit_name"]?.ToString() ?? "";

                using (ChangeProductUnitPickDialog dlg = new ChangeProductUnitPickDialog(choices, level, oldName, currentUnitId))
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        // تحديث الصف الحالي
                        drv["unit_id"]   = dlg.SelectedUnitId;
                        drv["unit_name"] = dlg.SelectedUnitName;

                        drv.EndEdit();
                        SetDirty(true);
                        UpdateConversionPreview();
                        if (level == 1) UpdateProductBaseUnitDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تغيير الوحدة: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // حذف الوحدة الأخيرة فقط (لحماية السلسلة)
        // ════════════════════════════════════════════════════════════
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProductUnits.CurrentRow == null)
            {
                MessageBox.Show("اختر وحدة للحذف.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int rowIndex = dgvProductUnits.CurrentRow.Index;
            int rowCount = _editTable?.Rows.Count ?? 0;

            // نسمح بحذف الأخيرة فقط لحماية السلسلة
            if (rowCount > 1 && rowIndex != rowCount - 1)
            {
                MessageBox.Show(
                    "لا يمكن حذف وحدة وسطى في السلسلة.\n" +
                    "احذف الوحدات من الأسفل للأعلى (الأعلى مستوى أولاً).",
                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataRowView drv = dgvProductUnits.CurrentRow.DataBoundItem as DataRowView;
                if (drv == null) return;

                int    unitId   = Convert.ToInt32(drv["id"]);
                string unitName = drv["unit_name"]?.ToString() ?? "وحدة";

                if (MessageBox.Show("تأكيد حذف الوحدة '" + unitName + "'؟",
                    "تأكيد الحذف", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _repo.DeleteProductUnit(unitId);
                    SetDirty(false);
                    LoadProductInfo();
                    LoadUnitsGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في الحذف: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // تحديث (Reload)
        // ════════════════════════════════════════════════════════════
        private void btnReload_Click(object sender, EventArgs e)
        {
            if (_isDirty &&
                MessageBox.Show("يوجد تغييرات غير محفوظة. هل تريد تجاهلها؟",
                    "تأكيد", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                return;

            LoadProductInfo();
            LoadUnitsGrid();
        }

        // ════════════════════════════════════════════════════════════
        // إضافة باركود للوحدة المحددة
        // ════════════════════════════════════════════════════════════
        private void BtnAddBarcode_Click(object sender, EventArgs e)
        {
            if (!_selectedProductId.HasValue)
            {
                MessageBox.Show("اختر منتجاً أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvProductUnits.CurrentRow == null)
            {
                MessageBox.Show("اختر صف وحدة في الجدول أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string code = (txtBarcodeInput?.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("أدخل الباركود.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcodeInput?.Focus();
                return;
            }

            DataRowView drv = dgvProductUnits.CurrentRow.DataBoundItem as DataRowView;
            if (drv == null) return;

            try
            {
                int  productUnitId = Convert.ToInt32(drv["id"]);
                bool isDefault     = chkBarcodeDefault != null && chkBarcodeDefault.Checked;
                _repo.AddProductUnitBarcode(productUnitId, code, isDefault);
                txtBarcodeInput.Clear();
                if (chkBarcodeDefault != null) chkBarcodeDefault.Checked = false;
                LoadUnitsGrid();
                LoadProductInfo();
                MessageBox.Show("تمت إضافة الباركود.", "تم",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // تصدير CSV
        // ════════════════════════════════════════════════════════════
        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try { DataGridViewExportHelper.ExportToCsv(dgvProductUnits, "product_units.csv"); }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في التصدير: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════
        // البحث بـ RowFilter (لا CurrencyManager exception)
        // ════════════════════════════════════════════════════════════
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplySearchFilter();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
        }

        private void ApplySearchFilter()
        {
            if (_editView == null) return;
            try
            {
                string q = (txtSearch?.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(q))
                {
                    _editView.RowFilter = string.Empty;
                }
                else
                {
                    string esc = q.Replace("'", "''");
                    _editView.RowFilter =
                        string.Format("unit_name LIKE '%{0}%' OR default_barcode LIKE '%{0}%'", esc);
                }
            }
            catch { /* تجاهل أخطاء RowFilter */ }
        }

        // ════════════════════════════════════════════════════════════
        // تحديث معاينة التحويل عند تغيير الصف
        // ════════════════════════════════════════════════════════════
        private void dgvProductUnits_SelectionChanged(object sender, EventArgs e)
        {
            UpdateConversionPreview();
        }

        private void UpdateConversionPreview()
        {
            if (dgvProductUnits.CurrentRow == null)
            {
                lblCurrentUnit.Text      = "معاينة الوحدة المحددة";
                txtConversions.Text      = string.Empty;
                txtFinalConversion.Text  = string.Empty;
                txtSellPriceUnit.Text    = string.Empty;
                txtCostPriceUnit.Text    = string.Empty;
                return;
            }

            try
            {
                DataRowView drv = dgvProductUnits.CurrentRow.DataBoundItem as DataRowView;
                if (drv == null) return;

                string unitName   = drv["unit_name"]?.ToString() ?? "";
                int    level      = drv["level_number"] == DBNull.Value
                                    ? 0 : Convert.ToInt32(drv["level_number"]);
                decimal factor    = drv["factor"]     == DBNull.Value ? 0m : Convert.ToDecimal(drv["factor"]);
                decimal packSize  = drv["pack_size"]  == DBNull.Value ? 1m : Convert.ToDecimal(drv["pack_size"]);
                decimal sell      = drv["sell_price"] == DBNull.Value ? 0m : Convert.ToDecimal(drv["sell_price"]);
                decimal cost      = drv["cost_price"] == DBNull.Value ? 0m : Convert.ToDecimal(drv["cost_price"]);
                string  barcode   = drv["default_barcode"]?.ToString() ?? "—";

                lblCurrentUnit.Text = string.Format("المستوى {0}: {1}", level, unitName);

                if (level <= 1)
                {
                    txtConversions.Text = "الوحدة الأساسية\nfactor = 1 (دائماً)";
                }
                else
                {
                    string parentName = drv["parent_unit_name"]?.ToString() ?? "السابق";
                    txtConversions.Text = string.Format(
                        "1 {0} = {1} × ({2})\nالمعامل الإجمالي = {3}",
                        unitName, packSize, parentName, factor);
                }

                txtFinalConversion.Text = SalesNumberFormat.FormatFactor(factor);
                txtSellPriceUnit.Text   = SalesNumberFormat.FormatPrice(sell);
                txtCostPriceUnit.Text   = SalesNumberFormat.FormatPrice(cost);
            }
            catch { /* تجاهل */ }
        }

        // ════════════════════════════════════════════════════════════
        // تلوين صف الوحدة الأساسية (المستوى 1)
        // ════════════════════════════════════════════════════════════
        private void dgvProductUnits_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvProductUnits.Rows[e.RowIndex];
            DataRowView drv = row.DataBoundItem as DataRowView;
            if (drv == null) return;

            try
            {
                int level = drv["level_number"] == DBNull.Value
                    ? 0 : Convert.ToInt32(drv["level_number"]);

                if (level == 1 && !row.Selected)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 220); // أصفر فاتح للأساس
                else if (!row.Selected)
                    row.DefaultCellStyle.BackColor = e.RowIndex % 2 == 0
                        ? Color.White : Color.FromArgb(245, 248, 255);
            }
            catch { /* تجاهل */ }
        }

        // ════════════════════════════════════════════════════════════
        // تحديث عرض الوحدة الأساسية في البطاقة العلوية
        // ════════════════════════════════════════════════════════════
        private void UpdateProductBaseUnitDisplay()
        {
            if (_editTable == null || _editTable.Rows.Count == 0)
            {
                txtProductBaseUnit.Text  = string.Empty;
                txtProductBasePrice.Text = string.Empty;
                return;
            }

            DataRow baseRow = _editTable.Rows[0]; // الصف الأول = الأساس
            txtProductBaseUnit.Text  = baseRow["unit_name"]?.ToString()  ?? string.Empty;
            txtProductBasePrice.Text = baseRow["sell_price"] == DBNull.Value
                ? string.Empty : SalesNumberFormat.FormatPrice(Convert.ToDecimal(baseRow["sell_price"]));
        }

        // ════════════════════════════════════════════════════════════
        // إدارة حالة Dirty (تغييرات غير محفوظة)
        // ════════════════════════════════════════════════════════════
        private void SetDirty(bool dirty)
        {
            _isDirty = dirty;
            tsBtnSave.Enabled    = dirty;
            tsLblDirty.Visible   = dirty;
        }

        // ════════════════════════════════════════════════════════════
        // أدوات مساعدة
        // ════════════════════════════════════════════════════════════
        private static void DisposePictureBoxImage(PictureBox box)
        {
            if (box == null) return;
            Image old = box.Image;
            box.Image = null;
            old?.Dispose();
        }

        // منع الإغلاق مع تغييرات غير محفوظة
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (_isDirty &&
                MessageBox.Show(
                    "يوجد تغييرات غير محفوظة. هل تريد الإغلاق بدون حفظ؟",
                    "تأكيد الإغلاق",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        private void lblBarcodeFor_Click(object sender, EventArgs e)
        {

        }
    }

    // ══════════════════════════════════════════════════════════════════
    // حوار إضافة وحدة - مع تمييز خاص للوحدة الأساسية (المستوى 1)
    // ══════════════════════════════════════════════════════════════════
    internal sealed class AddProductUnitPickDialog : Form
    {
        private ComboBox      _cbUnits;
        private NumericUpDown _numPackSize;
        private NumericUpDown _numSell;
        private NumericUpDown _numCost;
        private TextBox       _txtFactorPreview;
        private Button        _btnOk;
        private Button        _btnCancel;

        private readonly decimal _parentFactor;
        private readonly bool    _isBaseUnit; // true = المستوى 1

        public int     SelectedUnitId { get; private set; }
        public decimal PackSize       => _isBaseUnit ? 1m : (_numPackSize?.Value ?? 1m);
        public decimal SellPrice      => _numSell?.Value  ?? 0m;
        public decimal CostPrice      => _numCost?.Value  ?? 0m;
        public decimal Factor         => _isBaseUnit ? 1m : Math.Round(_parentFactor * PackSize, 6);

        public AddProductUnitPickDialog(
            IList<KeyValuePair<int, string>> unitChoices,
            decimal parentFactor,
            int newLevel)
        {
            _parentFactor = parentFactor <= 0 ? 1m : parentFactor;
            _isBaseUnit   = newLevel <= 1;

            RightToLeft       = RightToLeft.Yes;
            RightToLeftLayout = true;
            FormBorderStyle   = FormBorderStyle.FixedDialog;
            StartPosition     = FormStartPosition.CenterParent;
            MaximizeBox       = false;
            MinimizeBox       = false;
            Font              = new Font("Segoe UI", 10F);
            BackColor         = Color.FromArgb(245, 247, 250);

            DataTable dt = new DataTable();
            dt.Columns.Add("id",   typeof(int));
            dt.Columns.Add("name", typeof(string));
            foreach (KeyValuePair<int, string> kv in unitChoices)
                dt.Rows.Add(kv.Key, kv.Value);

            _cbUnits = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource    = dt,
                DisplayMember = "name",
                ValueMember   = "id",
                TabIndex      = 0,
                Font          = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            _btnCancel = new Button
            {
                Text         = "إلغاء",
                DialogResult = DialogResult.Cancel,
                FlatStyle    = FlatStyle.Flat,
                TabIndex     = 9,
                BackColor    = Color.FromArgb(189, 195, 199)
            };
            _btnCancel.FlatAppearance.BorderSize = 0;

            if (_isBaseUnit) BuildBaseUnitLayout();
            else             BuildChildUnitLayout(newLevel);

            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
            UpdateFactorPreview();
            Shown += (s, ev) => _cbUnits.Focus();
        }

        // ── واجهة الوحدة الأساسية (المستوى 1) ────────────────────────
        private void BuildBaseUnitLayout()
        {
            Text       = "تحديد الوحدة الأساسية للمنتج";
            ClientSize = new Size(480, 282);

            // شريط ذهبي توضيحي
            Panel banner = new Panel { Location = new Point(0, 0), Size = new Size(480, 48), BackColor = Color.FromArgb(255, 243, 205) };
            Label lblBanner = new Label
            {
                Text = "⭐  الوحدة الأساسية — المعامل = 1 دائماً  ⭐",
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(133, 100, 4)
            };
            banner.Controls.Add(lblBanner);

            Label lblUnit = MakeLabel("اختر اسم الوحدة الأساسية للمنتج:", 12, 56);
            _cbUnits.Location = new Point(12, 76);
            _cbUnits.Size     = new Size(450, 30);

            Label lblNote = new Label
            {
                Text = "هي الوحدة الأصغر للمنتج (مثال: حبة، قطعة، كيلو).\n" +
                       "كل وحدات أكبر منها ستُحسب بالنسبة لها تلقائياً.",
                Location = new Point(12, 114), Size = new Size(450, 40),
                ForeColor = Color.FromArgb(80, 100, 140),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            Label lblSell = MakeLabel("سعر البيع:", 12, 162);
            _numSell = new NumericUpDown
            {
                DecimalPlaces = 2, Maximum = 100000000M,
                Location = new Point(12, 182), Size = new Size(150, 30), TabIndex = 1
            };

            Label lblCost = MakeLabel("سعر التكلفة:", 178, 162);
            _numCost = new NumericUpDown
            {
                DecimalPlaces = 2, Maximum = 100000000M,
                Location = new Point(178, 182), Size = new Size(150, 30), TabIndex = 2
            };

            Panel sep = new Panel { Location = new Point(12, 228), Size = new Size(450, 1), BackColor = Color.FromArgb(200, 210, 220) };

            _btnOk = new Button
            {
                Text = "تعيين كوحدة أساسية", DialogResult = DialogResult.OK,
                Location = new Point(240, 238), Size = new Size(220, 36),
                BackColor = Color.FromArgb(215, 155, 0), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, TabIndex = 3
            };
            _btnOk.FlatAppearance.BorderSize = 0;
            _btnCancel.Location = new Point(130, 238);
            _btnCancel.Size     = new Size(100, 36);

            // نهيئه مخفياً لتجنب NullReference
            _numPackSize = new NumericUpDown { Value = 1m, Visible = false };

            Controls.AddRange(new Control[] { banner, lblUnit, _cbUnits, lblNote, lblSell, _numSell, lblCost, _numCost, sep, _btnOk, _btnCancel });
        }

        // ── واجهة الوحدة الفرعية (المستوى 2+) ───────────────────────
        private void BuildChildUnitLayout(int newLevel)
        {
            Text       = string.Format("إضافة وحدة - المستوى {0}", newLevel);
            ClientSize = new Size(480, 316);

            // شريط أزرق توضيحي
            Panel banner = new Panel { Location = new Point(0, 0), Size = new Size(480, 44), BackColor = Color.FromArgb(219, 234, 254) };
            Label lblBanner = new Label
            {
                Text = string.Format("وحدة المستوى {0}  —  تُحسب بالنسبة للوحدة السابقة", newLevel),
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175)
            };
            banner.Controls.Add(lblBanner);

            Label lblUnit = MakeLabel("اختر الوحدة:", 12, 52);
            _cbUnits.Location = new Point(12, 72);
            _cbUnits.Size     = new Size(450, 30);

            Label lblPackDesc = new Label
            {
                Text = "العبوة = كم وحدة من المستوى السابق تحتوي هذه الوحدة؟",
                Location = new Point(12, 112), Size = new Size(450, 20),
                ForeColor = Color.FromArgb(80, 100, 140),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            Label lblPack = MakeLabel("العبوة:", 12, 136);
            _numPackSize = new NumericUpDown
            {
                DecimalPlaces = 3, Minimum = 0.001M, Maximum = 1000000M,
                Increment = 1M, Value = 1M,
                Location = new Point(12, 156), Size = new Size(130, 30), TabIndex = 1
            };
            _numPackSize.ValueChanged += (s, ev) => UpdateFactorPreview();

            Label lblFactorPrev = MakeLabel("المعامل الإجمالي (محسوب تلقائياً):", 158, 136);
            _txtFactorPreview = new TextBox
            {
                ReadOnly = true, BackColor = Color.LightYellow,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(158, 156), Size = new Size(150, 30),
                TabStop = false, TextAlign = HorizontalAlignment.Center
            };

            Label lblSell = MakeLabel("سعر البيع:", 12, 200);
            _numSell = new NumericUpDown
            {
                DecimalPlaces = 2, Maximum = 100000000M,
                Location = new Point(12, 220), Size = new Size(150, 30), TabIndex = 2
            };

            Label lblCost = MakeLabel("سعر التكلفة:", 178, 200);
            _numCost = new NumericUpDown
            {
                DecimalPlaces = 2, Maximum = 100000000M,
                Location = new Point(178, 220), Size = new Size(150, 30), TabIndex = 3
            };

            Panel sep = new Panel { Location = new Point(12, 262), Size = new Size(450, 1), BackColor = Color.FromArgb(200, 210, 220) };

            _btnOk = new Button
            {
                Text = "إضافة", DialogResult = DialogResult.OK,
                Location = new Point(268, 272), Size = new Size(110, 36),
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, TabIndex = 4
            };
            _btnOk.FlatAppearance.BorderSize = 0;
            _btnCancel.Location = new Point(148, 272);
            _btnCancel.Size     = new Size(110, 36);

            Controls.AddRange(new Control[]
            {
                banner, lblUnit, _cbUnits, lblPackDesc,
                lblPack, _numPackSize, lblFactorPrev, _txtFactorPreview,
                lblSell, _numSell, lblCost, _numCost,
                sep, _btnOk, _btnCancel
            });
        }

        private void UpdateFactorPreview()
        {
            if (_txtFactorPreview == null) return;
            decimal f = _isBaseUnit ? 1m : Math.Round(_parentFactor * (_numPackSize?.Value ?? 1m), 6);
            _txtFactorPreview.Text = SalesNumberFormat.FormatFactor(f);
        }

        private static Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text      = text,
                AutoSize  = true,
                Location  = new Point(x, y),
                ForeColor = Color.FromArgb(60, 70, 90),
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (DialogResult != DialogResult.OK) return;
            if (_cbUnits.SelectedValue == null)
            {
                e.Cancel = true;
                MessageBox.Show("اختر وحدة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SelectedUnitId = Convert.ToInt32(_cbUnits.SelectedValue);
        }
    }

    // ══════════════════════════════════════════════════════════════════
    // حوار تغيير الوحدة - مخصص لاختيار مسمى جديد فقط
    // ══════════════════════════════════════════════════════════════════
    internal sealed class ChangeProductUnitPickDialog : Form
    {
        private ComboBox _cbUnits;
        private Button   _btnOk;
        private Button   _btnCancel;

        public int    SelectedUnitId   { get; private set; }
        public string SelectedUnitName { get; private set; }

        public ChangeProductUnitPickDialog(
            IList<KeyValuePair<int, string>> unitChoices,
            int level,
            string oldName,
            int currentUnitId)
        {
            Text              = "تغيير مسمى الوحدة";
            RightToLeft       = RightToLeft.Yes;
            RightToLeftLayout = true;
            FormBorderStyle   = FormBorderStyle.FixedDialog;
            StartPosition     = FormStartPosition.CenterParent;
            MaximizeBox       = false;
            MinimizeBox       = false;
            Font              = new Font("Segoe UI", 10F);
            BackColor         = Color.FromArgb(245, 247, 250);
            ClientSize        = new Size(400, 200);

            Panel banner = new Panel { Location = new Point(0, 0), Size = new Size(400, 40), BackColor = Color.FromArgb(220, 230, 240) };
            Label lblBanner = new Label
            {
                Text = string.Format("تغيير وحدة المستوى {0}", level),
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 60, 100)
            };
            banner.Controls.Add(lblBanner);

            Label lblOld = new Label
            {
                Text = "الوحدة الحالية: " + oldName,
                AutoSize = true, Location = new Point(12, 50),
                ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            Label lblUnit = new Label
            {
                Text = "اختر الوحدة الجديدة:",
                AutoSize = true, Location = new Point(12, 80),
                ForeColor = Color.FromArgb(60, 70, 90), Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            DataTable dt = new DataTable();
            dt.Columns.Add("id",   typeof(int));
            dt.Columns.Add("name", typeof(string));
            foreach (KeyValuePair<int, string> kv in unitChoices)
                dt.Rows.Add(kv.Key, kv.Value);

            _cbUnits = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource    = dt,
                DisplayMember = "name",
                ValueMember   = "id",
                Location      = new Point(12, 105),
                Size          = new Size(370, 30),
                TabIndex      = 0,
                Font          = new Font("Segoe UI", 11F)
            };
            _cbUnits.SelectedValue = currentUnitId;

            Panel sep = new Panel { Location = new Point(12, 145), Size = new Size(370, 1), BackColor = Color.FromArgb(200, 210, 220) };

            _btnOk = new Button
            {
                Text = "تغيير", DialogResult = DialogResult.OK,
                Location = new Point(200, 155), Size = new Size(100, 36),
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, TabIndex = 1
            };
            _btnOk.FlatAppearance.BorderSize = 0;

            _btnCancel = new Button
            {
                Text = "إلغاء", DialogResult = DialogResult.Cancel,
                Location = new Point(90, 155), Size = new Size(100, 36),
                BackColor = Color.FromArgb(189, 195, 199),
                FlatStyle = FlatStyle.Flat, TabIndex = 2
            };
            _btnCancel.FlatAppearance.BorderSize = 0;

            Controls.AddRange(new Control[] { banner, lblOld, lblUnit, _cbUnits, sep, _btnOk, _btnCancel });

            AcceptButton = _btnOk;
            CancelButton = _btnCancel;

            Shown += (s, ev) => _cbUnits.Focus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (DialogResult != DialogResult.OK) return;
            if (_cbUnits.SelectedValue == null)
            {
                e.Cancel = true;
                MessageBox.Show("اختر وحدة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SelectedUnitId   = Convert.ToInt32(_cbUnits.SelectedValue);
            SelectedUnitName = _cbUnits.Text;
        }
    }
}
