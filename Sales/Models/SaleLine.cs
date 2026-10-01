// ============================================================
// الملف    : SaleLine.cs
// الغرض    : نموذج عناصر المبيعات لتمثيل كمية وأسعار المنتجات في الفاتورة
// ============================================================

using System;

namespace Sales.Models
{
    public class SaleLine
    {
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal Factor { get; set; }
        public decimal Qty { get; set; }
        public decimal BaseQty { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }

        // Used by current POS UI.
        public string ScanCode { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }

        public void Recalc()
        {
            if (Factor <= 0) Factor = 1;
            if (Qty < 0) Qty = 0;

            BaseQty = Qty * Factor;

            decimal total = Qty * Price;
            if (Discount < 0) Discount = 0;
            if (Discount > total) Discount = total;
            LineTotal = total - Discount;
        }
    }
}
