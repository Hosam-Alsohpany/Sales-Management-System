// ============================================================
// الملف    : ProductUnitHierarchyContracts.cs
// الغرض    : عقود واجهات التحقق من هرمية وحدات المنتجات ونسب التحويل
// ============================================================

using System;
using System.Collections.Generic;

namespace Sales.Services.ProductUnits
{
    public class ProductUnitHierarchyNode
    {
        public int ProductUnitId { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int? ParentProductUnitId { get; set; }
        public decimal? PackSize { get; set; }
        public decimal Factor { get; set; }
    }

    public enum ProductUnitHierarchyIssueSeverity
    {
        Error = 1,
        Warning = 2
    }

    public class ProductUnitHierarchyIssue
    {
        public ProductUnitHierarchyIssueSeverity Severity { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public int? ProductUnitId { get; set; }
        public int? UnitId { get; set; }
    }

    public class ProductUnitHierarchyValidationResult
    {
        public List<ProductUnitHierarchyIssue> Errors { get; private set; }
        public List<ProductUnitHierarchyIssue> Warnings { get; private set; }
        public HashSet<int> AffectedProductUnitIds { get; private set; }
        public Dictionary<int, decimal> ComputedFactorsByProductUnitId { get; private set; }

        public ProductUnitHierarchyValidationResult()
        {
            Errors = new List<ProductUnitHierarchyIssue>();
            Warnings = new List<ProductUnitHierarchyIssue>();
            AffectedProductUnitIds = new HashSet<int>();
            ComputedFactorsByProductUnitId = new Dictionary<int, decimal>();
        }

        public bool IsValid
        {
            get { return Errors == null || Errors.Count == 0; }
        }

        public void AddError(string code, string message, int? productUnitId, int? unitId)
        {
            Errors.Add(new ProductUnitHierarchyIssue
            {
                Severity = ProductUnitHierarchyIssueSeverity.Error,
                Code = code,
                Message = message,
                ProductUnitId = productUnitId,
                UnitId = unitId
            });

            if (productUnitId.HasValue && productUnitId.Value > 0)
                AffectedProductUnitIds.Add(productUnitId.Value);
        }

        public void AddWarning(string code, string message, int? productUnitId, int? unitId)
        {
            Warnings.Add(new ProductUnitHierarchyIssue
            {
                Severity = ProductUnitHierarchyIssueSeverity.Warning,
                Code = code,
                Message = message,
                ProductUnitId = productUnitId,
                UnitId = unitId
            });

            if (productUnitId.HasValue && productUnitId.Value > 0)
                AffectedProductUnitIds.Add(productUnitId.Value);
        }
    }

    public class ProductUnitHierarchyServiceOptions
    {
        public int MaxDepth { get; set; }
        public decimal MaxFactor { get; set; }

        public static ProductUnitHierarchyServiceOptions Default()
        {
            return new ProductUnitHierarchyServiceOptions
            {
                MaxDepth = 20,
                MaxFactor = 1000000000m
            };
        }
    }
}
