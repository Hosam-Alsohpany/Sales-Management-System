// ============================================================
// الملف    : OrderAuditLogEntry.cs
// الغرض    : نموذج سجل التغييرات على الفواتير لتحديد المسؤول عن الحذف أو التعديل
// ============================================================

using System;

namespace Sales.Models
{
    public class OrderAuditLogEntry
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Details { get; set; }
    }
}
