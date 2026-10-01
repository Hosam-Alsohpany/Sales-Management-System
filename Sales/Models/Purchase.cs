// ============================================================
// الملف    : Purchase.cs
// الغرض    : نموذج فاتورة المشتريات لتمثيل الفاتورة الموردة للمخازن
// ============================================================

using System;
using System.Collections.Generic;

namespace Sales.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public List<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
    }
}
