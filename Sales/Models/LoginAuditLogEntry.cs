// ============================================================
// الملف    : LoginAuditLogEntry.cs
// الغرض    : نموذج سجل عمليات الدخول والخروج لمراقبة المستخدمين
// ============================================================

using System;

namespace Sales.Models
{
    public class LoginAuditLogEntry
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; }
        public bool Success { get; set; }
        public string FailureReason { get; set; }
    }
}
