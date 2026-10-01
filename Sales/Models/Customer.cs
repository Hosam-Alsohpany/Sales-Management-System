// ============================================================
// الملف    : Customer.cs
// الغرض    : نموذج بيانات العميل لتمثيل جدول العملاء وحساب أرصدتهم
// ============================================================

using System;

namespace Sales.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Details { get; set; }
        public string Address { get; set; }
        public decimal Balance { get; set; }
    }
}
