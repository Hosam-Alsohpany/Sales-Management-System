using System;
using Sales.Utilities;

namespace Sales.Models
{
    /// <summary>
    /// نموذج يمثل منتجاً في النظام.
    /// المخزون الفعلي: qty_scaled + qty_scale_pow10.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Sku { get; set; }

        /// <summary>كمية العرض (مشتقة من qty_scaled للتوافق مع الواجهات).</summary>
        public int Qty
        {
            get => ProductQuantityHelper.LegacyQtyFromScaled(QtyScaled, QtyScalePow10);
            set => QtyScaled = ProductQuantityHelper.BaseQtyToScaled(value, QtyScalePow10);
        }

        public long QtyScaled { get; set; }
        public int QtyScalePow10 { get; set; }
        public string ProductType { get; set; }

        public decimal DisplayQty => ProductQuantityHelper.ScaledToDisplay(QtyScaled, QtyScalePow10);

        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public decimal CostPrice { get; set; }
        public decimal MinQty { get; set; }
        public string ExpiryDate { get; set; }
        public string UpdatedAt { get; set; }
        public string DefaultBarcode { get; set; }
        public string ImagePath { get; set; }
    }
}
