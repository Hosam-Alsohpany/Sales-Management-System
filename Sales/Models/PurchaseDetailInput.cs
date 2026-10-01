// ============================================================
// الملف    : PurchaseDetailInput.cs
// الغرض    : نموذج إدخال تفاصيل فاتورة المشتريات أثناء عملية التوريد
// ============================================================

using System;

namespace Sales.Models
{
    public class PurchaseDetailInput
    {
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }

        public string UnitNameSnapshot { get; set; }
        public decimal FactorSnapshot { get; set; }

        public decimal QtyUnit { get; set; }
        public decimal BaseQty { get; set; }

        public decimal CostPrice { get; set; }

        public string ExpiryDate { get; set; }
    }
}
