using System;

using System.ComponentModel;

using System.Drawing;

using System.Windows.Forms;

using Sales.Models;

using Sales.Repositories;

using Sales.Utilities;



namespace Sales.Forms

{

    public partial class Manager_Users : Form

    {

        private UserRepository _repo = new UserRepository();

        private BindingList<User> _users;



        public Manager_Users()

        {

            InitializeComponent();

            try { UiTheme.ApplyToForm(this); } catch { }
            DataGridViewDateTimeFormatter.Apply(dgvUsers);
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgvUsers, "المستخدمين", "users.pdf");
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgvUsers, "المستخدمين");
        }



        private void Manager_Users_Load(object sender, EventArgs e)

        {

            LoadData();

        }



        private void LoadData()

        {

            try

            {

                var list = _repo.GetAllUsers();

                _users = new BindingList<User>(list);

                dgvUsers.DataSource = _users;



                // تنسيق الأعمدة

                if (dgvUsers.Columns["Id"] != null) dgvUsers.Columns["Id"].HeaderText = "اسم المستخدم";

                if (dgvUsers.Columns["Pwd"] != null)
                {
                    dgvUsers.Columns["Pwd"].HeaderText = "كلمة المرور"; // يفضل إخفاؤها في التطبيقات الحقيقية لكن للتبسيط هنا
                    dgvUsers.Columns["Pwd"].Visible = false;
                }

                if (dgvUsers.Columns["FullName"] != null) dgvUsers.Columns["FullName"].HeaderText = "الاسم الكامل";

                if (dgvUsers.Columns["Role"] != null) dgvUsers.Columns["Role"].HeaderText = "الصلاحية";

            }

            catch (Exception ex)

            {

                MessageHelper.ShowError("خطأ أثناء تحميل البيانات", ex.Message);

            }

        }



        private void ClearInputs()

        {

            txtId.Enabled = true; // السماح بالكتابة عند الجديد

            txtId.Clear();

            txtPwd.Clear();

            txtName.Clear();

            cmbRole.SelectedIndex = 0;

        }



        // ===================== أحداث الـ Toolbox =====================



        private void btnNew_Click(object sender, EventArgs e)

        {

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return;
            }

            ClearInputs();

            txtId.Focus();

        }



        private void btnAdd_Click(object sender, EventArgs e)

        {

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtPwd.Text))

            {

                MessageHelper.ShowWarning("يرجى إدخال اسم المستخدم وكلمة المرور");

                return;

            }



            try

            {

                if (_repo.IsUserExists(txtId.Text))

                {

                    MessageHelper.ShowWarning("اسم المستخدم موجود مسبقاً");

                    return;

                }



                var user = new User

                {

                    Id = txtId.Text,

                    Pwd = txtPwd.Text,

                    FullName = txtName.Text,

                    Role = cmbRole.SelectedItem != null ? cmbRole.SelectedItem.ToString() : "user"

                };



                _repo.AddUser(user);

                MessageHelper.ShowSuccess("تمت الإضافة بنجاح");

                LoadData();

                ClearInputs();

            }

            catch (Exception ex)

            {

                MessageHelper.ShowError("خطأ أثناء الإضافة", ex.Message);

            }

        }



        private void btnEdit_Click(object sender, EventArgs e)

        {

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text)) return;



            // في التعديل لا نسمح بتغيير المعرف الأساسي (ID)

            try

            {

                var user = new User

                {

                    Id = txtId.Text,

                    Pwd = txtPwd.Text,

                    FullName = txtName.Text,

                    Role = cmbRole.SelectedItem != null ? cmbRole.SelectedItem.ToString() : "user"

                };



                _repo.UpdateUser(user);

                MessageHelper.ShowSuccess("تم التعديل بنجاح");

                LoadData();

                ClearInputs();

            }

            catch (Exception ex)

            {

                MessageHelper.ShowError("خطأ أثناء التعديل", ex.Message);

            }

        }



        private void btnDelete_Click(object sender, EventArgs e)

        {

            if (dgvUsers.CurrentRow == null) return;

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return;
            }

            if (!SessionManager.CanCurrentUserDelete)
            {
                MessageHelper.ShowUnauthorized("حذف المستخدمين");
                return;
            }

            

            string id = dgvUsers.CurrentRow.Cells["Id"].Value.ToString();

            string roleValue = dgvUsers.CurrentRow.Cells["Role"].Value != null
                ? dgvUsers.CurrentRow.Cells["Role"].Value.ToString()
                : "";

            if (string.Equals(roleValue, "admin", StringComparison.OrdinalIgnoreCase))
            {
                int adminCount = 0;
                foreach (DataGridViewRow row in dgvUsers.Rows)
                {
                    if (row == null || row.IsNewRow) continue;
                    var r = row.Cells["Role"].Value != null ? row.Cells["Role"].Value.ToString() : "";
                    if (string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)) adminCount++;
                }

                if (adminCount <= 1)
                {
                    MessageHelper.ShowWarning("لا يمكن حذف آخر مدير. يجب أن يكون هناك مدير واحد على الأقل.");
                    return;
                }
            }

            

            // منع حذف المستخدم الحالي (نفسه) أو الأدمن الأساسي إذا أردنا

            if (id.ToLower() == "admin" || id.ToLower() == "hosam") // مثال للحماية البسيطة

            {

                if (!MessageHelper.AskForConfirmation("هل أنت متأكد من حذف هذا المستخدم الرئيسي؟"))

                    return;

            }

            else

            {

                if (!MessageHelper.AskForConfirmation("هل أنت متأكد من الحذف؟"))

                    return;

            }



            try

            {

                _repo.DeleteUser(id);

                MessageHelper.ShowSuccess("تم الحذف بنجاح");

                LoadData();

                ClearInputs();

            }

            catch (Exception ex)

            {

                MessageHelper.ShowError("خطأ أثناء الحذف", ex.Message);

            }

        }



        private void btnClose_Click(object sender, EventArgs e)

        {

            Close();

        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewExportHelper.ExportToCsv(dgvUsers, "users.csv");
            }
            catch
            {
            }
        }



        private void dgvUsers_Click(object sender, EventArgs e)

        {

            if (dgvUsers.CurrentRow != null && !dgvUsers.CurrentRow.IsNewRow)

            {

                object idObj = dgvUsers.CurrentRow.Cells["Id"] != null ? dgvUsers.CurrentRow.Cells["Id"].Value : null;
                txtId.Text = idObj != null ? idObj.ToString() : string.Empty;

                txtId.Enabled = false; // لا يمكن تغيير المعرف أثناء التعديل



                // كلمة المرور لا تُجلب من قاعدة البيانات (للأمان) لذلك لا تحاول قراءتها من الجريد
                // اتركها فارغة، وإذا أدخل المستخدم قيمة جديدة سيتم تحديثها
                txtPwd.Text = string.Empty;

                object nameObj = dgvUsers.CurrentRow.Cells["FullName"] != null ? dgvUsers.CurrentRow.Cells["FullName"].Value : null;
                txtName.Text = nameObj != null ? nameObj.ToString() : string.Empty;

                

                object roleObj = dgvUsers.CurrentRow.Cells["Role"] != null ? dgvUsers.CurrentRow.Cells["Role"].Value : null;
                string role = roleObj != null ? roleObj.ToString() : string.Empty;

                if (cmbRole.Items.Contains(role))

                    cmbRole.SelectedItem = role;
                else if (cmbRole.Items.Count > 0)
                    cmbRole.SelectedIndex = 0;

            }

        }

    }

}

