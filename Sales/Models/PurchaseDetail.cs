// ============================================================
// الملف    : PurchaseDetail.cs
// الغرض    : نموذج تفاصيل المشتريات لتسجيل أصناف وكميات الفاتورة الموردة
// ============================================================

namespace Sales.Models
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int? ProductUnitId { get; set; }
        public string UnitNameSnapshot { get; set; }
        public decimal? FactorSnapshot { get; set; }
        public decimal QtyUnit { get; set; }
        public decimal BaseQty { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public string ExpiryDate { get; set; }
        public string Note { get; set; }
        public long? QtyScaledSnapshot { get; set; }
        public int? QtyScalePow10Snapshot { get; set; }
    }
}
