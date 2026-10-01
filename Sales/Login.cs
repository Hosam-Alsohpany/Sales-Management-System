// ============================================================
// الملف    : Login.cs
// الغرض    : نافذة تسجيل الدخول للمستخدمين
// ============================================================

using System;                          // أساسيات C#
using System.Data.SQLite;              // مزود SQLite
using System.Windows.Forms;            // عناصر الواجهة
using Sales.Database;                  // DatabaseInitializer
using Sales.Forms;
using Sales.Utilities;
using Sales.Services;

namespace Sales
{
    public partial class Login : Form
    {

        /*  هذا السطر يعني:
  متغير عام للقراءة
  لا يمكن تغييره إلا من داخل Login
  آمن واحترافي*/

        public string LoggedUserName { get; private set; }
        //get : يسمح بقرائه القيمه 
        // private set : يعني ممنوع أي كلاس خارجي يغيّر القيمة

        public string FullName { get; private set; }
        public string Role { get; private set; }

        //public static string Userr;هناك طريقه امن واحترافيه اكثر 

        public Login()
        {
            InitializeComponent();
            try { SalesUiBootstrap.WireForm(this); } catch { }
        }


        private void Login_Load(object sender, EventArgs e)
        {
            // STEP 2 (Remember Username): تحميل آخر اسم مستخدم إذا كان خيار التذكر مفعّل
            try
            {
                bool remember = AppSettingsManager.GetBool(AppSettingsManager.Keys.RememberLastUsername, defaultValue: false);
                if (chkRememberUsername != null)
                    chkRememberUsername.Checked = remember;

                if (remember)
                {
                    string last = AppSettingsManager.GetString(AppSettingsManager.Keys.LastLoginUsername, defaultValue: string.Empty);
                    if (!string.IsNullOrWhiteSpace(last))
                        txtUsername.Text = last;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Login_Load remember username failed", ex);
            }

            // يكون المؤشر عليه في البدايه
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtPassword.Focus();
            }
            else
            {
                txtUsername.Focus();
            }

            // STEP 1 (Show/Hide Password): تأكيد أن كلمة المرور مخفية عند فتح الشاشة
            if (txtPassword != null)
            {
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '*';
            }
            if (chkShowPassword != null)
                chkShowPassword.Checked = false;

            //  this.AcceptButton = null; // لا تفعل زر الدخول عند فتح الفورم
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (txtPassword == null)
                return;

            bool show = chkShowPassword != null && chkShowPassword.Checked;
            if (show)
            {
                // إظهار كلمة المرور
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                // إخفاء كلمة المرور
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '*';
            }

            // الحفاظ على المؤشر داخل مربع النص بعد التبديل
            txtPassword.Focus();
            txtPassword.SelectionStart = txtPassword.TextLength;
        }

        private void btncreat_Click(object sender, EventArgs e)
        {
            /* var signupForm = new Signup();
             signupForm.Owner = this; // Login هو الأب
             signupForm.Show();
             this.Hide(); // نخفي Login مؤقتًا
             */
            /* using (Signup signupForm = new Signup())
             {
                 if (signupForm.ShowDialog() == DialogResult.OK)
                 {
                     // إذا أردت شيء بعد نجاح Signup
                 }
             }*/
            MessageHelper.ShowUnauthorized("إنشاء المستخدمين");

        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            try
            {
                var security = new LoginSecurityService();
                var user = security.TryLogin(txtUsername.Text, txtPassword.Text, out string failureMessage);

                if (user != null)
                {
                    // STEP 2 (Remember Username): حفظ/مسح آخر اسم مستخدم حسب الخيار
                    try
                    {
                        using (TransactionGuard.Enter("Login.RememberUsername"))
                        {
                            bool remember = chkRememberUsername != null && chkRememberUsername.Checked;
                            AppSettingsManager.Set(AppSettingsManager.Keys.RememberLastUsername, remember ? "1" : "0");
                            AppSettingsManager.Set(AppSettingsManager.Keys.LastLoginUsername, remember ? (txtUsername.Text ?? string.Empty).Trim() : string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Login remember username save failed", ex);
                    }

                    // حفظ اسم المستخدم
                    LoggedUserName = user.Id;
                    FullName = user.FullName;
                    Role = user.Role;

                    SessionManager.SetCurrentUser(user.Id, user.Role);

                    // إبلاغ البرنامج بنجاح الدخول
                    this.DialogResult = DialogResult.OK;

                    // إغلاق فورم الدخول
                    this.Close();
                }
                else
                {
                    MessageHelper.ShowError(string.IsNullOrWhiteSpace(failureMessage)
                        ? "معلومات الدخول غير صحيحة"
                        : failureMessage);
                }
            }
            catch (Exception ex)
            {
                // عرض أي خطأ يحدث
                MessageHelper.ShowError("خطأ أثناء تسجيل الدخول", ex.Message);
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                txtPassword.Focus();   // ينتقل لحقل الباسورد
                e.SuppressKeyPress = true;  // يمنع صوت التنبيه
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.SuppressKeyPress = true;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {

            /*  if (e.KeyCode == Keys.Enter)
              {
                  btnlogin.PerformClick();   // ينفّذ زر الدخول
                 // this.AcceptButton = btnlogin;  // من الآن Enter سينفذ زر الدخول

                  e.SuppressKeyPress = true;
              }
              if (e.KeyCode == Keys.Up)
              {
                  txtUsername.Focus();   // ينتقل لحقل الباسورد
                  e.SuppressKeyPress = true;  // يمنع صوت التنبيه
              }*/
            if (e.KeyCode == Keys.Enter)
            {
                var pwd = (txtPassword != null ? txtPassword.Text : string.Empty) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(pwd))
                {
                    MessageHelper.ShowWarning("اكتب كلمة المرور أولاً");
                    try
                    {
                        txtPassword.Focus();
                        txtPassword.SelectAll();
                    }
                    catch { }
                }
                else
                {
                    btnlogin.PerformClick();   // تنفيذ تسجيل الدخول
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                txtUsername.Focus();       // الرجوع لاسم المستخدم
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.SuppressKeyPress = true;
            }
        }


    }
}
