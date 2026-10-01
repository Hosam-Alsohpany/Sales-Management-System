// ============================================================
// الملف    : Manager_Payments.cs
// الغرض    : شاشة إدارة، مراجعة وتصفح جميع مدفوعات العملاء
// ============================================================

using System;
using System.Drawing;
using System.Windows.Forms;
using Sales.Repositories;
using Sales.Models;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class Manager_Payments : Form
    {
        private readonly string _userName;
        private PaymentRepository _repo = new PaymentRepository();

        public Manager_Payments() : this(null)
        {
        }

        public Manager_Payments(string userName)
        {
            _userName = userName;
            InitializeComponent();

            UiTheme.ApplyToForm(this);

            try
            {
                KeyPreview = true;
                KeyDown -= Manager_Payments_KeyDown;
                KeyDown += Manager_Payments_KeyDown;
            }
            catch
            {
            }

            DataGridViewDateTimeFormatter.Apply(dgvPayments);
            ConfigureGrid();
            LoadData();

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                dgvPayments.ReadOnly = true;
                dgvPayments.AllowUserToAddRows = false;
            }
        }

        private void Manager_Payments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;

            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    try
                    {
                        dgvPayments.Focus();
                    }
                    catch
                    {
                    }

                    return;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    Close();
                    return;
                }
            }
            catch
            {
            }
        }

        private void Manager_Payments_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadData();
        }

        private void ConfigureGrid()
        {
            if (dgvPayments == null) return;

            dgvPayments.AutoGenerateColumns = false;
        }

        private void LoadData()
        {
            dgvPayments.DataSource = _repo.GetAllPayments();
        }

        private void btnAddPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (SessionManager.IsReadOnlyForCurrentUser)
                {
                    MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                    return;
                }

                using (var frm = new AddPaymentDialogForm())
                {
                    if (frm.ShowDialog(this) != DialogResult.OK)
                        return;

                    _repo.AddPayment(new Payment
                    {
                        OrderId = frm.OrderId,
                        Amount = frm.Amount,
                        UserName = frm.UserName
                    });
                }

                LoadData();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ", ex.Message);
            }
        }

        private void btnDeletePayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (SessionManager.IsReadOnlyForCurrentUser)
                {
                    MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                    return;
                }

                if (!SessionManager.CanCurrentUserDelete)
                {
                    MessageHelper.ShowUnauthorized("حذف الدفعات");
                    return;
                }

                if (dgvPayments.CurrentRow == null)
                {
                    MessageHelper.ShowWarning("حدد عملية أولاً");
                    return;
                }

                var payment = dgvPayments.CurrentRow.DataBoundItem as Payment;
                if (payment == null)
                {
                    MessageHelper.ShowWarning("لا يمكن تحديد العملية");
                    return;
                }

                if (!MessageHelper.AskForConfirmation("هل أنت متأكد من حذف الدفعة المحددة؟") )
                    return;

                _repo.DeletePayment(payment.Id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ", ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
