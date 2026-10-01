// ============================================================
// الملف    : FirstTimeSetup.cs
// الغرض    : معالج الإعداد الأول للنظام وتهيئة مدير النظام
// ============================================================

using System;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Sales.Database;
using Sales.Services;
using Sales.Utilities;

namespace Sales
{
    public partial class FirstTimeSetup : Form
    {
        private int _currentStepIndex;

        public FirstTimeSetup()
        {
            InitializeComponent();

            try
            {
                UiTheme.ApplyToForm(this);
                SalesUiBootstrap.WireForm(this);
            }
            catch { }
        }

        private void UpdateNavigationState()
        {
            try
            {
                bool canNext = true;
                bool canFinish = true;

                try
                {
                    if (_currentStepIndex == 0) { canNext = ValidateStep1Silent(); canFinish = false; }
                    else if (_currentStepIndex == 1) { canNext = ValidateStep2Silent(); canFinish = false; }
                    else if (_currentStepIndex == 2) { canNext = ValidateStep3Silent(); canFinish = false; }
                    else if (_currentStepIndex == 3) { canNext = false; canFinish = ValidateStep1Silent() && ValidateStep2Silent() && ValidateStep3Silent(); }
                }
                catch
                {
                    canNext = true;
                    canFinish = true;
                }

                try { if (btnNext != null) btnNext.Enabled = (_currentStepIndex < 3) && canNext; } catch { }
                try { if (btnFinish != null) btnFinish.Enabled = (_currentStepIndex == 3) && canFinish; } catch { }
            }
            catch
            {
            }
        }

        private bool ValidateStep1Silent()
        {
            try
            {
                string storeName = string.Empty;
                try { storeName = (txtStoreName?.Text ?? string.Empty).Trim(); } catch { storeName = string.Empty; }
                if (string.IsNullOrWhiteSpace(storeName)) return false;

                string currency = string.Empty;
                try { currency = (cmbCurrency?.SelectedItem ?? string.Empty).ToString(); } catch { currency = string.Empty; }
                if (string.IsNullOrWhiteSpace(currency)) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateStep2Silent()
        {
            try
            {
                string username = string.Empty;
                try { username = (txtUsername?.Text ?? string.Empty).Trim(); } catch { username = string.Empty; }
                string fullName = string.Empty;
                try { fullName = (txtFullName?.Text ?? string.Empty).Trim(); } catch { fullName = string.Empty; }
                string pwd = string.Empty;
                try { pwd = txtPassword?.Text ?? string.Empty; } catch { pwd = string.Empty; }
                string confirm = string.Empty;
                try { confirm = txtConfirm?.Text ?? string.Empty; } catch { confirm = string.Empty; }

                if (string.IsNullOrWhiteSpace(username)) return false;
                if (string.IsNullOrWhiteSpace(fullName)) return false;
                if (pwd.Length < 6) return false;
                if (!string.Equals(pwd, confirm, StringComparison.Ordinal)) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateStep3Silent()
        {
            try
            {
                string unit = string.Empty;
                try { unit = (txtDefaultUnit?.Text ?? string.Empty).Trim(); } catch { unit = string.Empty; }
                string wh = string.Empty;
                try { wh = (txtDefaultWarehouse?.Text ?? string.Empty).Trim(); } catch { wh = string.Empty; }
                if (string.IsNullOrWhiteSpace(unit)) return false;
                if (string.IsNullOrWhiteSpace(wh)) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool HasAnyInput()
        {
            try
            {
                string store = string.Empty;
                try { store = (txtStoreName != null ? txtStoreName.Text : string.Empty) ?? string.Empty; } catch { store = string.Empty; }
                if (!string.IsNullOrWhiteSpace(store)) return true;

                string u = string.Empty;
                try { u = (txtUsername != null ? txtUsername.Text : string.Empty) ?? string.Empty; } catch { u = string.Empty; }
                if (!string.IsNullOrWhiteSpace(u)) return true;

                string fn = string.Empty;
                try { fn = (txtFullName != null ? txtFullName.Text : string.Empty) ?? string.Empty; } catch { fn = string.Empty; }
                if (!string.IsNullOrWhiteSpace(fn)) return true;

                string p = string.Empty;
                try { p = (txtPassword != null ? txtPassword.Text : string.Empty) ?? string.Empty; } catch { p = string.Empty; }
                if (!string.IsNullOrWhiteSpace(p)) return true;

                string c = string.Empty;
                try { c = (txtConfirm != null ? txtConfirm.Text : string.Empty) ?? string.Empty; } catch { c = string.Empty; }
                if (!string.IsNullOrWhiteSpace(c)) return true;

                string unit = string.Empty;
                try { unit = (txtDefaultUnit != null ? txtDefaultUnit.Text : string.Empty) ?? string.Empty; } catch { unit = string.Empty; }
                if (!string.IsNullOrWhiteSpace(unit) && unit.Trim() != "قطعة") return true;

                string wh = string.Empty;
                try { wh = (txtDefaultWarehouse != null ? txtDefaultWarehouse.Text : string.Empty) ?? string.Empty; } catch { wh = string.Empty; }
                if (!string.IsNullOrWhiteSpace(wh) && wh.Trim() != "الرئيسي") return true;

                bool seed = false;
                try { seed = chkSeedDemoProduct != null && chkSeedDemoProduct.Checked; } catch { seed = false; }
                if (seed) return true;

                return false;
            }
            catch
            {
                return true;
            }
        }

        private void FirstTimeSetup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                try
                {
                    if (HasAnyInput())
                    {
                        if (!MessageHelper.AskForConfirmation("هل تريد إغلاق إعداد النظام؟ سيتم فقدان البيانات المدخلة."))
                            return;
                    }
                }
                catch
                {
                }

                try { DialogResult = DialogResult.Cancel; } catch { }
                try { Close(); } catch { }
                return;
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                try
                {
                    int step = _currentStepIndex;

                    if (step == 0)
                    {
                        if (ActiveControl == txtStoreName) { try { cmbCurrency.Focus(); } catch { } return; }
                        if (ActiveControl == cmbCurrency) { try { btnNext.PerformClick(); } catch { } return; }
                    }

                    if (step == 1)
                    {
                        if (ActiveControl == txtUsername) { try { txtFullName.Focus(); } catch { } return; }
                        if (ActiveControl == txtFullName) { try { txtPassword.Focus(); } catch { } return; }
                        if (ActiveControl == txtPassword) { try { txtConfirm.Focus(); } catch { } return; }
                        if (ActiveControl == txtConfirm) { try { btnNext.PerformClick(); } catch { } return; }
                    }

                    if (step == 2)
                    {
                        if (ActiveControl == txtDefaultUnit) { try { txtDefaultWarehouse.Focus(); } catch { } return; }
                        if (ActiveControl == txtDefaultWarehouse) { try { chkSeedDemoProduct.Focus(); } catch { } return; }
                        if (ActiveControl == chkSeedDemoProduct) { try { btnNext.PerformClick(); } catch { } return; }
                    }

                    if (step == 3)
                    {
                        try { btnFinish.PerformClick(); } catch { }
                        return;
                    }

                    try { SelectNextControl(ActiveControl, forward: true, tabStopOnly: true, nested: true, wrap: true); } catch { }
                }
                catch
                {
                }

                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                try
                {
                    int step = _currentStepIndex;

                    if (step == 0)
                    {
                        if (ActiveControl == cmbCurrency) { try { txtStoreName.Focus(); } catch { } return; }
                        if (ActiveControl == txtStoreName) { try { btnBack.PerformClick(); } catch { } return; }
                    }

                    if (step == 1)
                    {
                        if (ActiveControl == txtConfirm) { try { txtPassword.Focus(); } catch { } return; }
                        if (ActiveControl == txtPassword) { try { txtFullName.Focus(); } catch { } return; }
                        if (ActiveControl == txtFullName) { try { txtUsername.Focus(); } catch { } return; }
                        if (ActiveControl == txtUsername) { try { btnBack.PerformClick(); } catch { } return; }
                    }

                    if (step == 2)
                    {
                        if (ActiveControl == chkSeedDemoProduct) { try { txtDefaultWarehouse.Focus(); } catch { } return; }
                        if (ActiveControl == txtDefaultWarehouse) { try { txtDefaultUnit.Focus(); } catch { } return; }
                        if (ActiveControl == txtDefaultUnit) { try { btnBack.PerformClick(); } catch { } return; }
                    }

                    if (step == 3)
                    {
                        try { btnBack.PerformClick(); } catch { }
                        return;
                    }

                    try { SelectNextControl(ActiveControl, forward: false, tabStopOnly: true, nested: true, wrap: true); } catch { }
                }
                catch
                {
                }

                return;
            }
        }

        private void SetStatus(string text, bool isError)
        {
            try
            {
                if (lblStatus == null) return;
                lblStatus.Text = text ?? string.Empty;
                lblStatus.ForeColor = isError ? Color.Firebrick : Color.DarkGreen;
            }
            catch
            {
            }
        }

        private void UpdateWizardUi()
        {
            try
            {
                if (tabWizard == null) return;

                int total = tabWizard.TabCount;
                if (_currentStepIndex < 0) _currentStepIndex = 0;
                if (_currentStepIndex >= total) _currentStepIndex = total - 1;

                try { tabWizard.SelectedIndex = _currentStepIndex; } catch { }

                try
                {
                    if (lblStep != null)
                        lblStep.Text = string.Format("الخطوة {0}/{1}", _currentStepIndex + 1, total);
                }
                catch
                {
                }

                try { btnBack.Enabled = _currentStepIndex > 0; } catch { }
                try { btnNext.Enabled = _currentStepIndex < total - 1; } catch { }
                try { btnFinish.Enabled = _currentStepIndex == total - 1; } catch { }

                if (_currentStepIndex == 0)
                {
                    try { txtStoreName.Focus(); } catch { }
                }
                else if (_currentStepIndex == 1)
                {
                    try { txtUsername.Focus(); } catch { }
                }
                else if (_currentStepIndex == 2)
                {
                    try { txtDefaultUnit.Focus(); } catch { }
                }
                else if (_currentStepIndex == 3)
                {
                    try { btnFinish.Focus(); } catch { }
                }

                UpdateNavigationState();
            }
            catch
            {
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool show = chkShowPassword != null && chkShowPassword.Checked;
                if (txtPassword != null) txtPassword.UseSystemPasswordChar = !show;
                if (txtConfirm != null) txtConfirm.UseSystemPasswordChar = !show;
            }
            catch
            {
            }
        }

        private bool ValidateStep1()
        {
            string storeName = string.Empty;
            try { storeName = (txtStoreName?.Text ?? string.Empty).Trim(); } catch { storeName = string.Empty; }

            if (string.IsNullOrWhiteSpace(storeName))
            {
                SetStatus("يرجى إدخال اسم المتجر", isError: true);
                try { txtStoreName.Focus(); } catch { }
                return false;
            }

            string currency = string.Empty;
            try { currency = (cmbCurrency?.SelectedItem ?? string.Empty).ToString(); } catch { currency = string.Empty; }
            if (string.IsNullOrWhiteSpace(currency))
            {
                SetStatus("يرجى اختيار العملة", isError: true);
                try { cmbCurrency.Focus(); } catch { }
                return false;
            }

            SetStatus(string.Empty, isError: false);
            return true;
        }

        private bool ValidateStep2()
        {
            string username = string.Empty;
            try { username = (txtUsername?.Text ?? string.Empty).Trim(); } catch { username = string.Empty; }
            string fullName = string.Empty;
            try { fullName = (txtFullName?.Text ?? string.Empty).Trim(); } catch { fullName = string.Empty; }
            string pwd = string.Empty;
            try { pwd = txtPassword?.Text ?? string.Empty; } catch { pwd = string.Empty; }
            string confirm = string.Empty;
            try { confirm = txtConfirm?.Text ?? string.Empty; } catch { confirm = string.Empty; }

            if (string.IsNullOrWhiteSpace(username))
            {
                SetStatus("يرجى إدخال اسم الحساب (Username)", isError: true);
                try { txtUsername.Focus(); } catch { }
                return false;
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                SetStatus("يرجى إدخال اسم المستخدم الظاهر", isError: true);
                try { txtFullName.Focus(); } catch { }
                return false;
            }

            if (pwd.Length < 6)
            {
                SetStatus("كلمة المرور يجب أن تكون 6 أحرف على الأقل", isError: true);
                try { txtPassword.Focus(); } catch { }
                return false;
            }

            if (!string.Equals(pwd, confirm, StringComparison.Ordinal))
            {
                SetStatus("كلمتا المرور غير متطابقتين", isError: true);
                try { txtConfirm.Focus(); txtConfirm.SelectAll(); } catch { }
                return false;
            }

            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE id=@id", con))
                    {
                        checkCmd.Parameters.AddWithValue("@id", username);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            SetStatus("اسم الحساب (Username) موجود مسبقاً", isError: true);
                            try { txtUsername.Focus(); txtUsername.SelectAll(); } catch { }
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(nameof(FirstTimeSetup), "ValidateStep2", ex);
                SetStatus("تعذر التحقق من اسم الحساب", isError: true);
                return false;
            }

            SetStatus(string.Empty, isError: false);
            return true;
        }

        private bool ValidateStep3()
        {
            string unit = string.Empty;
            try { unit = (txtDefaultUnit?.Text ?? string.Empty).Trim(); } catch { unit = string.Empty; }
            string wh = string.Empty;
            try { wh = (txtDefaultWarehouse?.Text ?? string.Empty).Trim(); } catch { wh = string.Empty; }

            if (string.IsNullOrWhiteSpace(unit))
            {
                SetStatus("يرجى إدخال اسم الوحدة الافتراضية", isError: true);
                try { txtDefaultUnit.Focus(); } catch { }
                return false;
            }

            if (string.IsNullOrWhiteSpace(wh))
            {
                SetStatus("يرجى إدخال اسم المخزن الافتراضي", isError: true);
                try { txtDefaultWarehouse.Focus(); } catch { }
                return false;
            }

            SetStatus(string.Empty, isError: false);
            return true;
        }

        private void RefreshSummary()
        {
            try
            {
                if (lblSummary == null) return;

                string storeName = string.Empty;
                try { storeName = (txtStoreName?.Text ?? string.Empty).Trim(); } catch { storeName = string.Empty; }
                string currency = string.Empty;
                try { currency = (cmbCurrency?.SelectedItem ?? string.Empty).ToString(); } catch { currency = string.Empty; }

                string username = string.Empty;
                try { username = (txtUsername?.Text ?? string.Empty).Trim(); } catch { username = string.Empty; }
                string fullName = string.Empty;
                try { fullName = (txtFullName?.Text ?? string.Empty).Trim(); } catch { fullName = string.Empty; }

                string unit = string.Empty;
                try { unit = (txtDefaultUnit?.Text ?? string.Empty).Trim(); } catch { unit = string.Empty; }
                string wh = string.Empty;
                try { wh = (txtDefaultWarehouse?.Text ?? string.Empty).Trim(); } catch { wh = string.Empty; }

                lblSummary.Text =
                    "ملخص الإعداد:\r\n\r\n" +
                    "- اسم المتجر: " + storeName + "\r\n" +
                    "- العملة: " + currency + "\r\n" +
                    "\r\n" +
                    "- اسم الحساب (مدير): " + username + "\r\n" +
                    "- الاسم الظاهر: " + fullName + "\r\n\r\n" +
                    "- الوحدة الافتراضية: " + unit + "\r\n" +
                    "- المخزن الافتراضي: " + wh + "\r\n";
            }
            catch
            {
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // Disabled: setup must be performed ONLY via SetupService.RunInitialSetup (btnFinish).
            // This prevents partial initialization states (admin created without settings/units/warehouse).
            try
            {
                SetStatus("استخدم زر (إنهاء) لإتمام الإعداد.", isError: true);
                _currentStepIndex = 3;
                UpdateWizardUi();
            }
            catch { }
        }

        private void FirstTimeSetup_Load(object sender, EventArgs e)
        {
            try
            {
                if (cmbCurrency != null && cmbCurrency.Items.Count == 0)
                {
                    cmbCurrency.Items.AddRange(new object[] { "YER", "EGP", "SAR", "AED", "USD", "EUR" });
                    cmbCurrency.SelectedIndex = 0;
                }
            }
            catch
            {
            }

            // Tax field is not used in the project currently → hide it to avoid meaningless input.
            try
            {
                if (lblTax != null) lblTax.Visible = false;
                if (txtDefaultTax != null)
                {
                    txtDefaultTax.Visible = false;
                    txtDefaultTax.Text = string.Empty;
                }
            }
            catch
            {
            }

            try
            {
                if (txtPassword != null) txtPassword.TextChanged += PasswordFields_TextChanged;
                if (txtConfirm != null) txtConfirm.TextChanged += PasswordFields_TextChanged;
            }
            catch
            {
            }

            _currentStepIndex = 0;

            try
            {
                if (txtStoreName != null) txtStoreName.TextChanged += (s, e2) => UpdateNavigationState();
                if (cmbCurrency != null) cmbCurrency.SelectedIndexChanged += (s, e2) => UpdateNavigationState();

                if (txtUsername != null) txtUsername.TextChanged += (s, e2) => UpdateNavigationState();
                if (txtFullName != null) txtFullName.TextChanged += (s, e2) => UpdateNavigationState();
                if (txtPassword != null) txtPassword.TextChanged += (s, e2) => UpdateNavigationState();
                if (txtConfirm != null) txtConfirm.TextChanged += (s, e2) => UpdateNavigationState();

                if (txtDefaultUnit != null) txtDefaultUnit.TextChanged += (s, e2) => UpdateNavigationState();
                if (txtDefaultWarehouse != null) txtDefaultWarehouse.TextChanged += (s, e2) => UpdateNavigationState();
                if (chkSeedDemoProduct != null) chkSeedDemoProduct.CheckedChanged += (s, e2) => UpdateNavigationState();
            }
            catch
            {
            }

            try
            {
                KeyPreview = true;
                KeyDown -= FirstTimeSetup_KeyDown;
                KeyDown += FirstTimeSetup_KeyDown;
            }
            catch
            {
            }
            UpdateWizardUi();
        }

        private void PasswordFields_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_currentStepIndex != 1) return;

                string pwd = txtPassword != null ? (txtPassword.Text ?? string.Empty) : string.Empty;
                string confirm = txtConfirm != null ? (txtConfirm.Text ?? string.Empty) : string.Empty;

                if (string.IsNullOrEmpty(pwd) && string.IsNullOrEmpty(confirm))
                {
                    SetStatus(string.Empty, isError: false);
                    return;
                }

                if (pwd.Length > 0 && pwd.Length < 6)
                {
                    SetStatus("كلمة المرور يجب أن تكون 6 أحرف على الأقل", isError: true);
                    return;
                }

                if (confirm.Length > 0 && !string.Equals(pwd, confirm, StringComparison.Ordinal))
                {
                    SetStatus("كلمتا المرور غير متطابقتين", isError: true);
                    return;
                }

                SetStatus(string.Empty, isError: false);
            }
            catch
            {
            }
        }

        private void tabWizard_Selecting(object sender, TabControlCancelEventArgs e)
        {
            try
            {
                if (e == null) return;
                if (tabWizard == null) return;

                if (e.TabPageIndex != _currentStepIndex)
                    e.Cancel = true;
            }
            catch
            {
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SetStatus(string.Empty, isError: false);
            _currentStepIndex--;
            if (_currentStepIndex < 0) _currentStepIndex = 0;
            UpdateWizardUi();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SetStatus(string.Empty, isError: false);

            if (_currentStepIndex == 0)
            {
                if (!ValidateStep1())
                    return;
            }
            else if (_currentStepIndex == 1)
            {
                if (!ValidateStep2())
                    return;
            }
            else if (_currentStepIndex == 2)
            {
                if (!ValidateStep3())
                    return;
            }

            _currentStepIndex++;
            if (tabWizard != null && _currentStepIndex >= tabWizard.TabCount)
                _currentStepIndex = tabWizard.TabCount - 1;

            if (_currentStepIndex == 3)
                RefreshSummary();

            UpdateWizardUi();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            SetStatus(string.Empty, isError: false);

            if (!ValidateStep1()) return;
            if (!ValidateStep2()) return;
            if (!ValidateStep3()) return;

            string storeName = string.Empty;
            try { storeName = (txtStoreName?.Text ?? string.Empty).Trim(); } catch { storeName = string.Empty; }
            string currency = string.Empty;
            try { currency = (cmbCurrency?.SelectedItem ?? string.Empty).ToString(); } catch { currency = string.Empty; }

            string username = string.Empty;
            try { username = (txtUsername?.Text ?? string.Empty).Trim(); } catch { username = string.Empty; }
            string fullName = string.Empty;
            try { fullName = (txtFullName?.Text ?? string.Empty).Trim(); } catch { fullName = string.Empty; }
            string pwd = string.Empty;
            try { pwd = (txtPassword?.Text ?? string.Empty); } catch { pwd = string.Empty; }

            string defaultUnit = string.Empty;
            try { defaultUnit = (txtDefaultUnit?.Text ?? string.Empty).Trim(); } catch { defaultUnit = string.Empty; }
            string defaultWarehouse = string.Empty;
            try { defaultWarehouse = (txtDefaultWarehouse?.Text ?? string.Empty).Trim(); } catch { defaultWarehouse = string.Empty; }

            decimal? tax = null;

            bool seedDemo = false;
            try { seedDemo = chkSeedDemoProduct != null && chkSeedDemoProduct.Checked; } catch { seedDemo = false; }

            SetStatus("جارٍ تنفيذ الإعداد...", isError: false);

            try
            {
                var req = new SetupRequest
                {
                    StoreName = storeName,
                    Currency = currency,
                    DefaultTax = tax,
                    AdminUsername = username,
                    AdminPassword = pwd,
                    AdminFullName = fullName,
                    DefaultUnitName = defaultUnit,
                    DefaultWarehouseName = defaultWarehouse,
                    SeedDemoProduct = seedDemo
                };

                var res = SetupService.RunInitialSetup(req);
                if (!res.Success)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(res.DiagnosticMessage))
                            Logger.LogError(nameof(FirstTimeSetup), "Finish.Diagnostic", new Exception(res.DiagnosticMessage));
                    }
                    catch
                    {
                    }

                    SetStatus(res.UserMessage ?? "فشل تنفيذ الإعداد.", isError: true);
                    return;
                }

                SetStatus("تم إعداد النظام بنجاح", isError: false);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SQLiteException ex) when ((ex.Message ?? string.Empty).IndexOf("UNIQUE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Logger.LogError(nameof(FirstTimeSetup), "Finish", ex);
                SetStatus("اسم المستخدم موجود مسبقاً", isError: true);
                _currentStepIndex = 1;
                UpdateWizardUi();
                try { txtUsername.Focus(); txtUsername.SelectAll(); } catch { }
            }
            catch (Exception ex)
            {
                Logger.LogError(nameof(FirstTimeSetup), "Finish", ex);
                SetStatus("فشل تنفيذ الإعداد. يرجى مراجعة السجل.", isError: true);
            }
        }
    }
}
