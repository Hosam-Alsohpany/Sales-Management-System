// ============================================================
// الملف    : ErrorLogEntry.cs
// الغرض    : نموذج سجل الأخطاء لتمثيل وتوثيق الاستثناءات التقنية
// ============================================================

using System;

namespace Sales.Models
{
    public class ErrorLogEntry
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
