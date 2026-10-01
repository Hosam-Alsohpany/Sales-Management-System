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
    public partial class PurchasesForm : Form
    {
        private readonly SupplierRepository _supplierRepo = new SupplierRepository();
        private readonly ProductRepository _productRepo = new ProductRepository();
        private readonly PurchaseRepository _purchaseRepo = new PurchaseRepository();

        private DataTable _dt = new DataTable();
        private List<ProductUnitLookup> _currentProductUnits = new List<ProductUnitLookup>();
        private ProductUnitLookup _selectedUnit;

        public PurchasesForm()
        {
            InitializeComponent();

            UiTheme.ApplyToForm(this);
        }

        private void PurchasesForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadSuppliers();
                SetupGrid();
                ResetEntry();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("فشل تحميل شاشة المشتريات", ex.Message);
            }
        }

        private void LoadSuppliers()
        {
            var list = _supplierRepo.GetAllSuppliers() ?? new List<Supplier>();
            cmbSuppliers.DataSource = list;
            cmbSuppliers.DisplayMember = nameof(Supplier.Name);
            cmbSuppliers.ValueMember = nameof(Supplier.Id);

            if (list.Count > 0)
                cmbSuppliers.SelectedIndex = 0;
        }

        private void SetupGrid()
        {
            if (_dt.Columns.Count == 0)
            {
                _dt.Columns.Add("ProductId", typeof(int));
                _dt.Columns.Add("ProductUnitId", typeof(int));
                _dt.Columns.Add("ProductName", typeof(string));
                _dt.Columns.Add("UnitName", typeof(string));
                _dt.Columns.Add("QtyUnit", typeof(decimal));
                _dt.Columns.Add("Factor", typeof(decimal));
                _dt.Columns.Add("BaseQty", typeof(decimal));
                _dt.Columns.Add("CostPrice", typeof(decimal));
                _dt.Columns.Add("ExpiryDate", typeof(string));
            }

            dgvLines.AutoGenerateColumns = false;
            dgvLines.DataSource = _dt;
        }

        private void ResetEntry()
        {
            txtProductId.Text = string.Empty;
            txtProductName.Text = string.Empty;
            _currentProductUnits = new List<ProductUnitLookup>();
            _selectedUnit = null;
            cmbUnits.DataSource = null;
            cmbUnits.Items.Clear();
            txtQtyUnit.Text = "1";
            txtCostPrice.Text = "0";
            chkHasExpiry.Checked = false;
            dtExpiry.Value = DateTime.Today;
            btnPickProduct.Focus();
        }

        private void btnPickProduct_Click(object sender, EventArgs e)
        {
            using (var frm = new ProductPickerForm())
            {
                if (frm.ShowDialog(this) == DialogResult.OK && frm.SelectedProductId.HasValue)
                {
                    int pid = frm.SelectedProductId.Value;
                    txtProductId.Text = pid.ToString();

                    var p = _productRepo.GetProductById(pid);
                    txtProductName.Text = p == null ? string.Empty : p.Label;

                    LoadUnits(pid);
                    txtQtyUnit.Focus();
                    txtQtyUnit.SelectAll();
                }
            }
        }

        private void LoadUnits(int productId)
        {
            _currentProductUnits = _productRepo.GetProductUnits(productId) ?? new List<ProductUnitLookup>();
            if (_currentProductUnits.Count == 0)
            {
                cmbUnits.DataSource = null;
                cmbUnits.Items.Clear();
                _selectedUnit = null;
                MessageHelper.ShowWarning("لا توجد وحدات لهذا المنتج. افتح (المنتجات → وحدات المنتج) وأضف وحدة للمنتج.");
                return;
            }
            cmbUnits.DataSource = _currentProductUnits;
            cmbUnits.DisplayMember = nameof(ProductUnitLookup.UnitName);
            cmbUnits.ValueMember = nameof(ProductUnitLookup.ProductUnitId);
            if (_currentProductUnits.Count > 0)
                cmbUnits.SelectedIndex = 0;
            _selectedUnit = cmbUnits.SelectedItem as ProductUnitLookup;

            if (_selectedUnit != null)
                txtCostPrice.Text = SalesNumberFormat.FormatPrice(_selectedUnit.CostPrice);
        }

        private void cmbUnits_SelectedValueChanged(object sender, EventArgs e)
        {
            _selectedUnit = cmbUnits.SelectedItem as ProductUnitLookup;
            if (_selectedUnit == null) return;

            // Do NOT use SellPrice in purchases
            try
            {
                txtCostPrice.Text = SalesNumberFormat.FormatPrice(_selectedUnit.CostPrice);
            }
            catch { }
        }

        private void chkHasExpiry_CheckedChanged(object sender, EventArgs e)
        {
            dtExpiry.Enabled = chkHasExpiry.Checked;
        }

        private void btnAddLine_Click(object sender, EventArgs e)
        {
            int pid;
            if (!int.TryParse((txtProductId.Text ?? string.Empty).Trim(), out pid) || pid <= 0)
            {
                MessageHelper.ShowWarning("اختر المنتج أولاً");
                return;
            }

            if (_selectedUnit == null)
            {
                MessageHelper.ShowWarning("اختر الوحدة");
                return;
            }

            if (!decimal.TryParse((txtQtyUnit.Text ?? "0").Trim().Replace(",", ""), out decimal qtyUnit) || qtyUnit <= 0)
            {
                MessageHelper.ShowWarning("الكمية غير صحيحة");
                return;
            }

            if (!decimal.TryParse((txtCostPrice.Text ?? "0").Trim().Replace(",", ""), out decimal costPrice) || costPrice < 0)
            {
                MessageHelper.ShowWarning("سعر التكلفة غير صحيح");
                return;
            }

            decimal factor = _selectedUnit.Factor <= 0 ? 1m : _selectedUnit.Factor;
            decimal baseQty = qtyUnit * factor;
            string expiry = chkHasExpiry.Checked ? dtExpiry.Value.ToString("yyyy-MM-dd") : string.Empty;

            var row = _dt.NewRow();
            row["ProductId"] = pid;
            row["ProductUnitId"] = _selectedUnit.ProductUnitId;
            row["ProductName"] = txtProductName.Text ?? string.Empty;
            row["UnitName"] = _selectedUnit.UnitName ?? string.Empty;
            row["QtyUnit"] = qtyUnit;
            row["Factor"] = factor;
            row["BaseQty"] = baseQty;
            row["CostPrice"] = costPrice;
            row["ExpiryDate"] = expiry;
            _dt.Rows.Add(row);

            ResetEntry();
        }

        private void btnRemoveLine_Click(object sender, EventArgs e)
        {
            if (dgvLines.CurrentRow == null) return;
            if (dgvLines.CurrentRow.IsNewRow) return;
            dgvLines.Rows.RemoveAt(dgvLines.CurrentRow.Index);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_dt.Rows.Count == 0)
            {
                MessageHelper.ShowWarning("لا توجد سطور");
                return;
            }

            int? supplierId = null;
            try
            {
                if (cmbSuppliers.SelectedValue != null)
                    supplierId = Convert.ToInt32(cmbSuppliers.SelectedValue);
            }
            catch
            {
                supplierId = null;
            }

            var lines = new List<PurchaseDetailInput>();
            foreach (DataRow r in _dt.Rows)
            {
                if (r == null || r.RowState == DataRowState.Deleted) continue;

                int productId = Convert.ToInt32(r["ProductId"]);
                int productUnitId = Convert.ToInt32(r["ProductUnitId"]);
                string unitName = r["UnitName"].ToString();
                decimal qtyUnit = Convert.ToDecimal(r["QtyUnit"]);
                decimal factor = Convert.ToDecimal(r["Factor"]);
                decimal baseQty = Convert.ToDecimal(r["BaseQty"]);
                decimal costPrice = Convert.ToDecimal(r["CostPrice"]);
                string expiry = r["ExpiryDate"] == DBNull.Value ? string.Empty : r["ExpiryDate"].ToString();

                lines.Add(new PurchaseDetailInput
                {
                    ProductId = productId,
                    ProductUnitId = productUnitId,
                    UnitNameSnapshot = unitName,
                    FactorSnapshot = factor,
                    QtyUnit = qtyUnit,
                    BaseQty = baseQty,
                    CostPrice = costPrice,
                    ExpiryDate = expiry
                });
            }

            try
            {
                string createdBy = SessionManager.CurrentUsername;
                _purchaseRepo.SavePurchase(supplierId, dtPurchaseDate.Value, note: null, createdBy: createdBy, details: lines);
                MessageHelper.ShowSuccess("تم حفظ فاتورة الشراء");
                _dt.Rows.Clear();
                ResetEntry();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("فشل الحفظ", ex.Message);
            }
        }
    }
}
