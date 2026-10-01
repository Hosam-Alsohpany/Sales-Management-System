// ============================================================
// الملف    : ProductUnitLookup.cs
// الغرض    : نموذج الربط بين المنتجات ووحداتها بالباركود وسعر البيع
// ============================================================

using System;

namespace Sales.Models
{
    public class ProductUnitLookup
    {
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal Factor { get; set; }
        public decimal SellPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int DisplayOrder { get; set; }
        public string Barcode { get; set; }
        public bool IsDefaultBarcode { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ProductName, UnitName);
        }
    }
}
