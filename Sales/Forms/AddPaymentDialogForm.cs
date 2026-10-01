// ============================================================
// الملف    : AddPaymentDialogForm.cs
// الغرض    : شاشة منبثقة لإضافة دفعات مالية جديدة للفواتير وتحديث حالة السداد
// ============================================================

using System;
using System.Windows.Forms;
using Sales.Utilities;
using Sales.Validators;

namespace Sales.Forms
{
    public partial class AddPaymentDialogForm : Form
    {
        public int OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public string UserName { get; private set; }

        public AddPaymentDialogForm()
        {
            InitializeComponent();
            btnOk.Click += OnOk;
        }

        private void OnOk(object sender, EventArgs e)
        {
            if (!int.TryParse((txtOrderId.Text ?? string.Empty).Trim(), out int oid))
                oid = 0;

            decimal amt = 0m;
            decimal.TryParse((txtAmount.Text ?? string.Empty).Trim().Replace(",", ""), out amt);

            if (!PaymentValidator.TryValidate(oid, amt, out string err))
            {
                MessageHelper.ShowWarning(err);
                return;
            }

            OrderId = oid;
            Amount = amt;
            UserName = string.IsNullOrWhiteSpace(txtUser.Text) ? null : txtUser.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
