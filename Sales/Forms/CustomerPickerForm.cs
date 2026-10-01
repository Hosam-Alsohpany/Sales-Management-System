// ============================================================
// الملف    : CustomerPickerForm.cs
// الغرض    : شاشة اختيار العميل المناسب لإرفاقه بالفاتورة الحالية
// ============================================================

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class CustomerPickerForm : Form
    {
        private readonly CustomerRepository _repo = new CustomerRepository();
        private BindingList<Customer> _all = new BindingList<Customer>();

        public int? SelectedCustomerId { get; private set; }
        public string SelectedCustomerName { get; private set; }

        public CustomerPickerForm()
        {
            InitializeComponent();
            try { UiTheme.ApplyToForm(this); } catch { }
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = _all;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DoubleClick += (s, e) => AcceptSelection();
        }

        private void CustomerPickerForm_Load(object sender, EventArgs e)
        {
            var list = _repo.GetAllCustomers() ?? new System.Collections.Generic.List<Customer>();
            _all = new BindingList<Customer>(list);
            dgv.DataSource = _all;
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string q = (txtSearch.Text ?? string.Empty).Trim();
            var src = _repo.GetAllCustomers() ?? new System.Collections.Generic.List<Customer>();
            if (!string.IsNullOrEmpty(q))
            {
                src = src.Where(c => c != null &&
                    ((c.Name ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                     (c.Tel ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            }
            dgv.DataSource = new BindingList<Customer>(src);
        }

        private void btnOk_Click(object sender, EventArgs e) => AcceptSelection();

        private void btnClear_Click(object sender, EventArgs e)
        {
            SelectedCustomerId = null;
            SelectedCustomerName = null;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AcceptSelection()
        {
            if (dgv.CurrentRow?.DataBoundItem is Customer c)
            {
                SelectedCustomerId = c.Id;
                SelectedCustomerName = c.Name;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
