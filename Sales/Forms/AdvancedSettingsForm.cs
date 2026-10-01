// ============================================================
// الملف    : AdvancedSettingsForm.cs
// الغرض    : شاشة تكوين إعدادات الاتصال، الأمان والنسخ الاحتياطي
// ============================================================

using System;
using System.IO;
using System.Windows.Forms;
using Sales.Database;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class AdvancedSettingsForm : Form
    {
        public AdvancedSettingsForm()
        {
            InitializeComponent();
            try { UiTheme.ApplyToForm(this); } catch { }
        }

        private void AdvancedSettingsForm_Load(object sender, EventArgs e)
        {
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "الإعدادات المتقدمة"))
                return;
            LoadSettings();
        }

        private void LoadSettings()
        {
            chkTaxEnabled.Checked = AppSettingsManager.GetBool(AppSettingsManager.Keys.TaxEnabled, false);
            txtTaxPercent.Text = AppSettingsManager.GetDecimal(AppSettingsManager.Keys.DefaultTax, 0m).ToString("0.##");
            numAutoLogout.Value = Math.Max(0, Math.Min(480, AppSettingsManager.GetInt(AppSettingsManager.Keys.AutoLogoutMinutes, 0)));
            numMaxFailed.Value = Math.Max(1, Math.Min(50, AppSettingsManager.GetInt(AppSettingsManager.Keys.MaxFailedLoginAttempts, 5)));
            chkDailyBackup.Checked = AppSettingsManager.GetBool(AppSettingsManager.Keys.EnableDailyAutoBackup, false);
            string path = AppSettingsManager.GetString(AppSettingsManager.Keys.DailyBackupPath, string.Empty);
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(Path.GetDirectoryName(DatabaseInitializer.DbPath) ?? "", "Backups");
            txtBackupPath.Text = path;
        }

        private void btnBrowseBackup_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (Directory.Exists(txtBackupPath.Text))
                    dlg.SelectedPath = txtBackupPath.Text;
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtBackupPath.Text = dlg.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                decimal tax = 0m;
                if (!decimal.TryParse(txtTaxPercent.Text.Trim(), out tax) || tax < 0m || tax > 100m)
                {
                    MessageHelper.ShowWarning("نسبة الضريبة يجب أن تكون بين 0 و 100");
                    return;
                }

                AppSettingsManager.Set(AppSettingsManager.Keys.TaxEnabled, chkTaxEnabled.Checked ? "1" : "0");
                AppSettingsManager.Set(AppSettingsManager.Keys.DefaultTax, tax.ToString(System.Globalization.CultureInfo.InvariantCulture));
                AppSettingsManager.Set(AppSettingsManager.Keys.AutoLogoutMinutes, ((int)numAutoLogout.Value).ToString());
                AppSettingsManager.Set(AppSettingsManager.Keys.MaxFailedLoginAttempts, ((int)numMaxFailed.Value).ToString());
                AppSettingsManager.Set(AppSettingsManager.Keys.EnableDailyAutoBackup, chkDailyBackup.Checked ? "1" : "0");
                AppSettingsManager.Set(AppSettingsManager.Keys.DailyBackupPath, txtBackupPath.Text.Trim());
                AppSettingsManager.Set(AppSettingsManager.Keys.SettingsLastUpdatedAt, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                MessageHelper.ShowSuccess("تم حفظ الإعدادات");
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("فشل الحفظ", ex.Message);
            }
        }

        private void btnBackupNow_Click(object sender, EventArgs e)
        {
            try
            {
                string dest = DailyBackupService.RunBackup();
                if (string.IsNullOrWhiteSpace(dest))
                    MessageHelper.ShowWarning("لم يتم العثور على قاعدة البيانات");
                else
                    MessageHelper.ShowSuccess("تم النسخ الاحتياطي: " + dest);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("نسخ احتياطي", ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
