// ============================================================
// الملف    : ProductPickerForm.cs
// الغرض    : شاشة اختيار والبحث السريع عن المنتجات لإدراجها بالفاتورة
// ============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class ProductPickerForm : Form
    {
        private readonly ProductRepository _productRepo = new ProductRepository();
        private List<Product> _allProducts = new List<Product>();

        public int? SelectedProductId { get; private set; }

        public ProductPickerForm()
        {
            InitializeComponent();
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgv, "اختيار منتج", "products_picker.pdf", txtSearch?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgv, "اختيار منتج", txtSearch?.Text);
        }

        private void ProductPickerForm_Load(object sender, EventArgs e)
        {
            LoadProducts();
            txtSearch.Focus();
        }

        private void LoadProducts()
        {
            _allProducts = _productRepo.GetAllProducts() ?? new List<Product>();
            BindProducts(_allProducts);
        }

        private void BindProducts(List<Product> list)
        {
            var src = new BindingList<Product>(list ?? new List<Product>());
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = src;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string q = (txtSearch.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(q))
            {
                BindProducts(_allProducts);
                return;
            }

            var filtered = _allProducts
                .Where(p => p != null &&
                            (
                                (p.Label ?? string.Empty).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                (p.Id.ToString()).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                (p.DefaultBarcode ?? string.Empty).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                            ))
                .ToList();

            BindProducts(filtered);
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSearch != null)
                {
                    txtSearch.Text = string.Empty;
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
                DataGridViewExportHelper.ExportToCsv(dgv, "products_picker.csv");
            }
            catch
            {
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            TrySelectCurrentAndClose();
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            TrySelectCurrentAndClose();
        }

        private void TrySelectCurrentAndClose()
        {
            if (dgv.CurrentRow == null) return;

            var item = dgv.CurrentRow.DataBoundItem as Product;
            if (item == null) return;

            SelectedProductId = item.Id;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
