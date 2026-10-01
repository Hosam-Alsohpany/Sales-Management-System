// ============================================================
// الملف    : DailyBackupService.cs
// الغرض    : خدمة النسخ الاحتياطي لقاعدة البيانات بشكل يومي وتلقائي
// ============================================================

using System;
using System.IO;
using Sales.Database;
using Sales.Utilities;

namespace Sales.Services
{
    /// <summary>
    /// نسخ احتياطي يومي لقاعدة البيانات عند الإغلاق أو حسب الجدولة.
    /// </summary>
    public static class DailyBackupService
    {
        private const string LastBackupDateKey = "DailyBackup_LastRunDate";

        public static void TryRunScheduledOrOnExit(bool onExit)
        {
            try
            {
                if (!AppSettingsManager.GetBool(AppSettingsManager.Keys.EnableDailyAutoBackup, false))
                    return;

                string today = DateTime.Now.ToString("yyyy-MM-dd");
                string last = AppSettingsManager.GetString(LastBackupDateKey, string.Empty);
                if (!onExit && string.Equals(last, today, StringComparison.Ordinal))
                    return;

                RunBackup();
                AppSettingsManager.Set(LastBackupDateKey, today);
            }
            catch (Exception ex)
            {
                try { Logger.LogError("DailyBackupService failed", ex); } catch { }
            }
        }

        public static string RunBackup()
        {
            string dbPath = DatabaseInitializer.DbPath;
            if (!File.Exists(dbPath))
                return null;

            string folder = AppSettingsManager.GetString(
                AppSettingsManager.Keys.DailyBackupPath,
                Path.Combine(Path.GetDirectoryName(dbPath) ?? dbPath, "Backups"));

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string dest = Path.Combine(folder, "SalesAuto_" + stamp + ".db");
            File.Copy(dbPath, dest, overwrite: false);
            return dest;
        }
    }
}
