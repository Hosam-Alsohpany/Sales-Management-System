// ============================================================
// الملف    : Payment.cs
// الغرض    : نموذج عملية الدفع لتمثيل حركات تسديد فواتير المبيعات
// ============================================================

using System;

namespace Sales.Models
{
    public class Payment// المدفوعات
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string UserName { get; set; }
    }
}
