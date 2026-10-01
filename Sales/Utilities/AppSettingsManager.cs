// ============================================================
// الملف    : AppSettingsManager.cs
// الغرض    : إدارة وتكوين إعدادات التطبيق من ملفات التكوين الخارجية
// ============================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using Sales.Repositories;

namespace Sales.Utilities
{
    public static class AppSettingsManager
    {
        private static readonly object _lock = new object();
        private static readonly Dictionary<string, string> _cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly AppSettingsRepository _repo = new AppSettingsRepository();

        public static string GetString(string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key)) return defaultValue;

            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var cached))
                    return cached;

                var value = _repo.GetValue(key);
                if (value == null)
                    value = defaultValue;

                _cache[key] = value;
                return value;
            }
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            var s = GetString(key, defaultValue ? "1" : "0");
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;

            s = s.Trim();
            if (string.Equals(s, "1") || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(s, "0") || string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "no", StringComparison.OrdinalIgnoreCase))
                return false;

            return defaultValue;
        }

        public static decimal GetDecimal(string key, decimal defaultValue)
        {
            var s = GetString(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                return v;
            if (decimal.TryParse(s, out v))
                return v;
            return defaultValue;
        }

        public static int GetInt(string key, int defaultValue)
        {
            var s = GetString(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                return v;
            if (int.TryParse(s, out v))
                return v;
            return defaultValue;
        }

        public static void Set(string key, string value)
        {
            lock (_lock)
            {
                _repo.SetValue(key, value);
                _cache[key] = value ?? string.Empty;
            }
        }

        public static void ClearCache()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        public static class Keys
        {
            public const string RequireCustomerInInvoice = "RequireCustomerInInvoice";
            public const string AllowPartialPayment = "AllowPartialPayment";
            public const string LowStockThreshold = "LowStockThreshold";
            public const string EnforceAdminForDeletes = "EnforceAdminForDeletes";
            public const string ReadOnlyForNormalUser = "ReadOnlyForNormalUser";
            public const string AllowNormalUserCreateOrders = "AllowNormalUserCreateOrders";
            public const string AllowNormalUserEditOrders = "AllowNormalUserEditOrders";
            public const string AllowNormalUserDeleteOrders = "AllowNormalUserDeleteOrders";

            public const string PreventSellingWhenOutOfStock = "PreventSellingWhenOutOfStock";
            public const string EnableLowStockAlerts = "EnableLowStockAlerts";
            public const string AllowNormalUserEditProductPrices = "AllowNormalUserEditProductPrices";
            public const string AllowNormalUserDeleteProducts = "AllowNormalUserDeleteProducts";

            public const string AllowNormalUserEditCustomers = "AllowNormalUserEditCustomers";
            public const string AllowNormalUserDeleteCustomers = "AllowNormalUserDeleteCustomers";

            public const string AutoLogoutMinutes = "AutoLogoutMinutes";
            public const string MaxFailedLoginAttempts = "MaxFailedLoginAttempts";
            public const string EnableDailyAutoBackup = "EnableDailyAutoBackup";
            public const string DailyBackupPath = "DailyBackupPath";
            public const string TaxEnabled = "TaxEnabled";

            public const string SettingsLastUpdatedAt = "SettingsLastUpdatedAt";

            public const string StoreName = "StoreName";
            public const string Currency = "Currency";
            public const string DefaultTax = "DefaultTax";
            public const string DefaultUnit = "DefaultUnit";
            public const string DefaultWarehouseId = "DefaultWarehouseId";
            public const string IsInitialized = "IsInitialized";
            public const string RememberLastUsername = "RememberLastUsername";
            public const string LastLoginUsername = "LastLoginUsername";
        }
    }
}
