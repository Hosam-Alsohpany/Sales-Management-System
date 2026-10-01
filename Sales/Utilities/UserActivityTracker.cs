using System;

namespace Sales.Utilities
{
    /// <summary>
    /// تتبع آخر نشاط للمستخدم (للخروج التلقائي).
    /// </summary>
    public static class UserActivityTracker
    {
        private static DateTime _lastActivityUtc = DateTime.UtcNow;

        public static DateTime LastActivityUtc
        {
            get { return _lastActivityUtc; }
        }

        public static void NotifyActivity()
        {
            _lastActivityUtc = DateTime.UtcNow;
        }

        public static void Reset()
        {
            _lastActivityUtc = DateTime.UtcNow;
        }
    }
}
