// ============================================================
// الملف    : LoginSecurityService.cs
// الغرض    : إدارة محاولات تسجيل الدخول وحماية الحسابات من التخمين
// ============================================================

using System;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Services
{
    /// <summary>
    /// قفل الحساب بعد محاولات فاشلة + تسجيل LoginAuditLog.
    /// </summary>
    public class LoginSecurityService
    {
        private readonly UserRepository _users = new UserRepository();
        private readonly LoginAttemptsRepository _attempts = new LoginAttemptsRepository();
        private readonly LoginAuditLogRepository _audit = new LoginAuditLogRepository();

        public User TryLogin(string userName, string password, out string failureMessage)
        {
            failureMessage = null;
            string id = (userName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                failureMessage = "اسم المستخدم مطلوب";
                _audit.Insert(id, GetLocalIp(), false, failureMessage);
                return null;
            }

            if (_attempts.IsLocked(id, out DateTime? lockedUntil))
            {
                failureMessage = "الحساب مقفل حتى " + (lockedUntil.HasValue ? lockedUntil.Value.ToString("yyyy-MM-dd HH:mm") : "لاحقاً");
                _audit.Insert(id, GetLocalIp(), false, failureMessage);
                return null;
            }

            try
            {
                var user = _users.ValidateUser(id, password);
                if (user == null)
                {
                    int max = AppSettingsManager.GetInt(AppSettingsManager.Keys.MaxFailedLoginAttempts, 5);
                    if (max < 1) max = 5;
                    int fails = _attempts.RecordFailure(id, max);
                    failureMessage = fails >= max
                        ? "تم قفل الحساب بعد " + max + " محاولات فاشلة"
                        : "معلومات الدخول غير صحيحة";
                    _audit.Insert(id, GetLocalIp(), false, failureMessage);
                    return null;
                }

                _attempts.ClearFailures(id);
                _audit.Insert(id, GetLocalIp(), true, null);
                return user;
            }
            catch (Exception ex)
            {
                failureMessage = ex.Message;
                try { Logger.LogError("LoginSecurityService.TryLogin", ex); } catch { }
                _audit.Insert(id, GetLocalIp(), false, failureMessage);
                return null;
            }
        }

        public static void RecordLogout(string userName)
        {
            try
            {
                new LoginAuditLogRepository().SetLogoutForOpenSession((userName ?? string.Empty).Trim());
            }
            catch (Exception ex)
            {
                try { Logger.LogError("RecordLogout failed", ex); } catch { }
            }
        }

        private static string GetLocalIp()
        {
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
