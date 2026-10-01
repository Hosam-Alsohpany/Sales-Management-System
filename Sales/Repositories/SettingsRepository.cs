// ============================================================
// الملف    : SettingsRepository.cs
// الغرض    : مستودع إدارة وحفظ قيم إعدادات التهيئة العامة
// ============================================================

using System;

namespace Sales.Repositories
{
    internal sealed class LegacySettingsStore
    {
        internal string Get(string key)
        {
            return null;
        }

        internal void Set(string key, string value)
        {
            throw new NotSupportedException();
        }
    }
}
