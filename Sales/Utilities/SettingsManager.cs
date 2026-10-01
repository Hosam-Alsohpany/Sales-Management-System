// ============================================================
// الملف    : SettingsManager.cs
// الغرض    : حفظ واسترجاع إعدادات تهيئة التطبيق الأساسية وقاعدة البيانات
// ============================================================

using System;

namespace Sales.Utilities
{
    internal static class LegacySettingsBridge
    {
        internal static string GetString(string key, string defaultValue)
        {
            return AppSettingsManager.GetString(key, defaultValue);
        }

        internal static bool GetBool(string key, bool defaultValue)
        {
            return AppSettingsManager.GetBool(key, defaultValue);
        }

        internal static decimal GetDecimal(string key, decimal defaultValue)
        {
            return AppSettingsManager.GetDecimal(key, defaultValue);
        }

        internal static void Set(string key, string value)
        {
            AppSettingsManager.Set(key, value);
        }

        internal static void ClearCache()
        {
            AppSettingsManager.ClearCache();
        }
    }
}
