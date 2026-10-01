using System;

namespace Sales.Models
{
    public class ProductUnit
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public decimal Factor { get; set; }
        public decimal SellPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentProductUnitId { get; set; }
        public decimal? PackSize { get; set; }
        /// <summary>اسم الوحدة (من JOIN، للعرض).</summary>
        public string UnitName { get; set; }
        public string DefaultBarcode { get; set; }
    }
}
