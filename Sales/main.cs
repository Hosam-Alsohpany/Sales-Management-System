// ============================================================
// الملف    : main.cs
// الغرض    : النافذة والشاشة الرئيسية لإدارة ميزات التطبيق الكاملة
// ============================================================

using System;

using System.Collections.Generic;

using System.ComponentModel;

using System.Data;

using System.Drawing;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Windows.Forms;

using Sales.Forms;

using Sales.Database;

using Sales.Services;

using Sales.Utilities;



namespace Sales

{

    public partial class main : Form

    {



        // دالة عامة لفتح الفورم مرة واحدة فقط

        private void OpenForm(Form frm)

        {

            // المرور على جميع الفورمات المفتوحة داخل الـ MDI Parent

            foreach (Form f in MdiChildren)

            {

                // مقارنة نوع الفورم المفتوح مع نوع الفورم المطلوب فتحه

                if (f.GetType() == frm.GetType())

                {

                    // إذا كان الفورم مفتوح مسبقًا → نفعّله (نجيبه للأمام)

                    f.Activate();

                    // نخرج من الدالة بدون فتح فورم جديد

                    return;

                }

            }

            // لو وصلنا هنا → الفورم غير مفتوح

            // نحدد أن الفورم الجديد تابع للفورم الرئيسي (MDI)

            frm.MdiParent = this;

            try
            {
                if (!string.IsNullOrWhiteSpace(_storeName) && frm != null)
                {
                    string prefix = _storeName + " - ";
                    if (string.IsNullOrWhiteSpace(frm.Text))
                        frm.Text = _storeName;
                    else if (!frm.Text.StartsWith(prefix, StringComparison.Ordinal))
                        frm.Text = prefix + frm.Text;
                }
            }
            catch
            {
                try { Logger.LogWarning(nameof(main), "OpenForm", "Failed to set child title", null); } catch { }
            }

            try
            {
                UiTheme.ApplyToForm(frm);
                SalesUiBootstrap.WireForm(frm);
            }
            catch { }

            // عرض الفورم

            frm.Show();
            try { HookActivity(frm); } catch { }

        }



        private string _userName; // اسم المستخدم الحالي

        private string _FullName;

        private string _role;

        private string _storeName;



        // Constructor يستقبل اسم المستخدم

        public main(string userName, string FullName, string role)

        {

            InitializeComponent();

            _userName = userName;

            _FullName = FullName;

            _role = role;

            try { SalesUiBootstrap.WireForm(this); } catch { }

        }



        /* public main()

         {

             InitializeComponent();

         }*/



        private void main_Load(object sender, EventArgs e)

        {

            //lblComputer.Text  &=  Environment.MachineName; vb.net

            //lblDate.Text &= now.toshortdatestring;

            lblComputer.Text += Environment.MachineName;

            lblTime.Text += DateTime.Now.ToShortDateString();





            // مثال: عرض اسم المستخدم

            lblUsers.Text += _userName + " / " + _FullName + " (" + (_role == "admin" ? "مدير" : "مستخدم") + ")";

            try
            {
                _storeName = AppSettingsManager.GetString(AppSettingsManager.Keys.StoreName, string.Empty);
                if (!string.IsNullOrWhiteSpace(_storeName))
                    this.Text = _storeName + " - " + this.Text;
            }
            catch
            {
                try { Logger.LogWarning(nameof(main), "Load", "Failed to read StoreName"); } catch { }
            }
            ApplyPermissions();
            SetupAutoLogoutAndActivity();

        }

        private void SetupAutoLogoutAndActivity()
        {
            try
            {
                UserActivityTracker.Reset();
                HookActivity(this);
                foreach (Form child in MdiChildren)
                    HookActivity(child);
                this.MdiChildActivate += (s, e) =>
                {
                    if (ActiveMdiChild != null)
                        HookActivity(ActiveMdiChild);
                };
                timerAutoLogout.Start();
            }
            catch (Exception ex)
            {
                try { Logger.LogWarning(nameof(main), "SetupAutoLogout", ex.Message); } catch { }
            }
        }

        private void HookActivity(Control root)
        {
            if (root == null) return;
            root.MouseMove -= OnUserActivity;
            root.MouseMove += OnUserActivity;
            root.KeyDown -= OnUserActivityKey;
            root.KeyDown += OnUserActivityKey;
            foreach (Control c in root.Controls)
                HookActivity(c);
        }

        private void OnUserActivity(object sender, EventArgs e) => UserActivityTracker.NotifyActivity();

        private void OnUserActivityKey(object sender, KeyEventArgs e) => UserActivityTracker.NotifyActivity();

        private void timerAutoLogout_Tick(object sender, EventArgs e)
        {
            int minutes = AppSettingsManager.GetInt(AppSettingsManager.Keys.AutoLogoutMinutes, 0);
            if (minutes <= 0) return;

            var idle = DateTime.UtcNow - UserActivityTracker.LastActivityUtc;
            if (idle.TotalMinutes < minutes) return;

            try
            {
                timerAutoLogout.Stop();
                MessageBox.Show("انتهت مدة الجلسة بسبب عدم النشاط.", "خروج تلقائي",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                PerformLogoutAndClose();
            }
            catch { }
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { DailyBackupService.TryRunScheduledOrOnExit(onExit: true); } catch { }
            try { LoginSecurityService.RecordLogout(_userName); } catch { }
        }

        private void PerformLogoutAndClose()
        {
            try { LoginSecurityService.RecordLogout(_userName); } catch { }
            SessionManager.Clear();
            Close();
        }







        private void timer1_Tick(object sender, EventArgs e)

        {

            lblDate.Text = DateTime.Now.ToLongTimeString();



        }



        private void menuExit_Click(object sender, EventArgs e)

        {

            PerformLogoutAndClose();

        }



        private void muneAddProducts_Click(object sender, EventArgs e)

        {

            // var  frm = new Add_Products();

            //frm.MdiParent=Me;

            OpenForm(new Add_Products(_userName));



        }



        private void muneManageProducts_Click(object sender, EventArgs e)

        {

            OpenForm(new Manager_Products(_userName));



        }



        private void tsProducs_Click(object sender, EventArgs e)

        {

            /* var frm = new Manager_Products(_userName);

             frm.MdiParent = this;

             frm.Show();*/

            OpenForm(new Manager_Products(_userName));





        }



        private void tsCustomers_Click(object sender, EventArgs e)

        {

            OpenForm(new Manager_Customers(_userName));

        }



        private void muneManageCustomers_Click(object sender, EventArgs e)

        {

            OpenForm(new Manager_Customers(_userName));

        }



        private void tsOrders_Click(object sender, EventArgs e)

        {

            OpenForm(new POSForm(_FullName));

        }



       



        private void muneSearchInOrders_Click(object sender, EventArgs e)

        {

            OpenForm(new Search_Manager_Orders(_userName, _FullName));

        }



        private void munePayments_Click(object sender, EventArgs e)

        {

            OpenForm(new Manager_Payments(_userName));

        }



        private void muneStockHistory_Click(object sender, EventArgs e)

        {

            OpenForm(new Stock_History());

        }



        private void muneManageUsers_Click(object sender, EventArgs e)

        {

            OpenForm(new Manager_Users());

        }



        private void muneManageSuppliers_Click(object sender, EventArgs e)

        {

            OpenForm(new Manager_Suppliers(_userName));

        }



        private void ApplyPermissions()

        {

            if (_role != "admin")

            {

                muneManageProducts.Enabled = false;

                tsProducs.Enabled = false;

                muneAddProducts.Enabled = false;



                if (muneManageUsers != null) muneManageUsers.Enabled = false;

                if (المستخدمينToolStripMenuItem != null) المستخدمينToolStripMenuItem.Visible = false;



                muneStockHistory.Enabled = false;

                munePayments.Enabled = false;

                if (munePurchases != null) munePurchases.Enabled = false;

                muneManageCategories.Enabled = false;
                if (muneUnits != null) muneUnits.Enabled = false;
                if (muneProductUnits != null) muneProductUnits.Enabled = false;
                if (muneBarcodeProfiles != null) muneBarcodeProfiles.Enabled = false;
            }
            else
            {
                if (إدارةToolStripMenuItem != null) إدارةToolStripMenuItem.Visible = true;
            }

            if (_role != "admin" && إدارةToolStripMenuItem != null)
                إدارةToolStripMenuItem.Visible = false;

            if (_role != "admin" && muneReports != null)
                muneReports.Visible = false;

        }



        private void muneCustomersList_Click(object sender, EventArgs e)

        {

            OpenForm(new Customers_List());

        }



        private void muneManageCategories_Click(object sender, EventArgs e)

        {

            OpenForm(new Manager_Categories());

        }

        private void muneUnits_Click(object sender, EventArgs e)
        {
            OpenForm(new UnitsForm());
        }

        private void munePurchases_Click(object sender, EventArgs e)
        {
            OpenForm(new PurchasesForm());
        }

        private void muneProductUnits_Click(object sender, EventArgs e)
        {
            OpenForm(new ProductUnitsForm());
        }

        private void muneBarcodeProfiles_Click(object sender, EventArgs e)
        {
            OpenForm(new BarcodeSetupForm());
        }



        /// <summary>

        private void مننحنToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
        }

        private void muneUs_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
        }

        private void ShowAboutDialog()
        {
            try
            {
                using (var frm = new AboutBox())
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح نافذة حول البرنامج: {ex.Message}",
                    "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Sales.Utilities.Logger.LogError("Error opening AboutBox", ex);
            }
        }

        /// <summary>

        private void muneAbout_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new AboutBox())
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح نافذة المعلومات: {ex.Message}", 
                               "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Sales.Utilities.Logger.LogError("Error opening AboutBox", ex);
            }
        }

        private void muneManageOrdersbarcode_Click(object sender, EventArgs e)
        {
            OpenForm(new POSForm(_FullName));
        }

        private void muneManageOrders_Click(object sender, EventArgs e)
        {
            OpenForm(new Manager_Orders(_FullName));
        }



        private void munePosHotkeys_Click(object sender, EventArgs e)

        {

            OpenForm(new PosHotkeysForm());

        }

        private void muneAdvancedSettings_Click(object sender, EventArgs e)
        {
            OpenForm(new AdvancedSettingsForm());
        }

        private void muneOrderAuditLog_Click(object sender, EventArgs e)
        {
            OpenForm(new OrderAuditLogForm());
        }

        private void muneLoginAuditLog_Click(object sender, EventArgs e)
        {
            OpenForm(new LoginAuditLogForm());
        }

        private void muneErrorLog_Click(object sender, EventArgs e)
        {
            OpenForm(new ErrorLogForm());
        }

        private void muneDashboard_Click(object sender, EventArgs e)
        {
            OpenForm(new DashboardForm());
        }

        private void muneReports_Click(object sender, EventArgs e)
        {
            OpenForm(new ReportsForm());
        }
    }
}

