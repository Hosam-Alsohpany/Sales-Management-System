using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;

namespace Sales.Forms
{
    public partial class PosHotkeysForm : Form
    {
        private readonly PosHotkeysRepository _repo = new PosHotkeysRepository();
        private readonly ProductRepository _productRepo = new ProductRepository();

        private int? _selectedKeyCode;
        private int? _selectedProductId;
        private ProductUnitLookup _selectedProductUnit;

        public PosHotkeysForm()
        {
            InitializeComponent();
        }

        private void ApplyGridArabicHeadersAndStyle()
        {
            try
            {
                if (dgv == null || dgv.Columns == null) return;

                try
                {
                    dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(dgv.Font.FontFamily, 10F, System.Drawing.FontStyle.Bold);
                    dgv.DefaultCellStyle.Font = new System.Drawing.Font(dgv.Font.FontFamily, 10F, System.Drawing.FontStyle.Regular);
                }
                catch
                {
                }

                if (dgv.Columns.Contains("key_code")) dgv.Columns["key_code"].HeaderText = "المفتاح";
                if (dgv.Columns.Contains("product_unit_id")) dgv.Columns["product_unit_id"].HeaderText = "رقم وحدة المنتج";
                if (dgv.Columns.Contains("product_name")) dgv.Columns["product_name"].HeaderText = "اسم المنتج";
                if (dgv.Columns.Contains("unit_name")) dgv.Columns["unit_name"].HeaderText = "الوحدة";
                if (dgv.Columns.Contains("factor")) dgv.Columns["factor"].HeaderText = "المعامل";
                if (dgv.Columns.Contains("qty_delta")) dgv.Columns["qty_delta"].HeaderText = "الكمية";
                if (dgv.Columns.Contains("note")) dgv.Columns["note"].HeaderText = "ملاحظة";
                if (dgv.Columns.Contains("updated_at")) dgv.Columns["updated_at"].HeaderText = "آخر تحديث";

                try
                {
                    foreach (DataGridViewColumn c in dgv.Columns)
                    {
                        if (c != null)
                            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
                catch
                {
                }
            }
            catch
            {
            }
        }

        private void PosHotkeysForm_Load(object sender, EventArgs e)
        {
            ResetEntry();
            LoadGrid();
        }

        private void LoadGrid()
        {
            DataTable dt = _repo.GetAllHotkeys();
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = dt;
            ApplyGridArabicHeadersAndStyle();
        }

        private void ResetEntry()
        {
            _selectedKeyCode = null;
            _selectedProductId = null;
            _selectedProductUnit = null;

            txtKey.Text = string.Empty;
            txtProduct.Text = string.Empty;
            cmbUnits.DataSource = null;
            txtQtyDelta.Text = "1";
            txtNote.Text = string.Empty;

            btnSave.Enabled = true;
            btnDelete.Enabled = false;
        }

        private void btnPickProduct_Click(object sender, EventArgs e)
        {
            using (var picker = new ProductPickerForm())
            {
                if (picker.ShowDialog(this) != DialogResult.OK) return;
                if (!picker.SelectedProductId.HasValue) return;

                LoadProductUnits(picker.SelectedProductId.Value);
            }
        }

        private void LoadProductUnits(int productId)
        {
            var p = _productRepo.GetProductById(productId);
            if (p == null)
            {
                MessageBox.Show("المنتج غير موجود");
                return;
            }

            _selectedProductId = p.Id;
            txtProduct.Text = p.Label ?? string.Empty;

            var units = _productRepo.GetProductUnits(p.Id) ?? new System.Collections.Generic.List<ProductUnitLookup>();
            cmbUnits.DisplayMember = "DisplayName";
            cmbUnits.ValueMember = "ProductUnitId";
            cmbUnits.DataSource = units;

            if (units.Count > 0)
            {
                cmbUnits.SelectedIndex = 0;
                _selectedProductUnit = units[0];
            }
        }

        private void cmbUnits_SelectedValueChanged(object sender, EventArgs e)
        {
            var pu = cmbUnits.SelectedItem as ProductUnitLookup;
            _selectedProductUnit = pu;
        }

        private bool TryParseKeyCode(out int keyCode)
        {
            keyCode = 0;
            string t = (txtKey.Text ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(t)) return false;

            if (t.StartsWith("F"))
            {
                int n;
                if (int.TryParse(t.Substring(1), out n) && n >= 1 && n <= 12)
                {
                    keyCode = (int)(Keys.F1 + (n - 1));
                    return true;
                }
            }

            return false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int keyCode;
            if (!TryParseKeyCode(out keyCode))
            {
                MessageBox.Show("أدخل مفتاح من F1 إلى F12");
                return;
            }

            if (_selectedProductUnit == null || _selectedProductUnit.ProductUnitId <= 0)
            {
                MessageBox.Show("اختر منتج ثم وحدة");
                return;
            }

            decimal qtyDelta;
            if (!decimal.TryParse(txtQtyDelta.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out qtyDelta) || qtyDelta <= 0)
            {
                MessageBox.Show("QtyDelta غير صحيحة");
                return;
            }

            _repo.UpsertHotkey(keyCode, _selectedProductUnit.ProductUnitId, qtyDelta, txtNote.Text);
            LoadGrid();
            ResetEntry();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!_selectedKeyCode.HasValue) return;
            _repo.DeleteHotkey(_selectedKeyCode.Value);
            LoadGrid();
            ResetEntry();
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                btnDelete.Enabled = false;
                return;
            }

            object keyObj = dgv.CurrentRow.Cells["key_code"].Value;
            int keyCode;
            if (keyObj == null || keyObj == DBNull.Value || !int.TryParse(keyObj.ToString(), out keyCode))
            {
                btnDelete.Enabled = false;
                return;
            }

            _selectedKeyCode = keyCode;
            btnDelete.Enabled = true;

            // عرض النص F1..F12
            string keyText = Enum.GetName(typeof(Keys), keyCode) ?? string.Empty;
            txtKey.Text = keyText;

            // المنتج/الوحدة من الجدول
            try { txtProduct.Text = dgv.CurrentRow.Cells["product_name"].Value == DBNull.Value ? string.Empty : dgv.CurrentRow.Cells["product_name"].Value.ToString(); } catch { }
            try { txtNote.Text = dgv.CurrentRow.Cells["note"].Value == DBNull.Value ? string.Empty : dgv.CurrentRow.Cells["note"].Value.ToString(); } catch { }
            try { txtQtyDelta.Text = Convert.ToString(dgv.CurrentRow.Cells["qty_delta"].Value, CultureInfo.InvariantCulture); } catch { }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ResetEntry();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

      
    }
}
