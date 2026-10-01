// ============================================================
// الملف    : ProductTypeHelper.cs
// الغرض    : مساعد التعامل وتحديد أنواع أصناف المنتجات
// ============================================================

using System;
using System.Data.SQLite;
using Sales.Services.ProductDomains;

namespace Sales.Services
{
    /// <summary>
    /// قيم product_type في جدول Products وربطها بـ ProductDomains.
    /// </summary>
    public static class ProductTypeHelper
    {
        public const string Physical = "physical";
        public const string Weighted = "weighted";
        public const string Service = "service";

        public static int DefaultScalePow10ForType(string productType)
        {
            return string.Equals(productType, Weighted, StringComparison.OrdinalIgnoreCase) ? 3 : 0;
        }

        public static bool IsService(string productType)
        {
            return string.Equals(productType, Service, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsWeighted(string productType)
        {
            return string.Equals(productType, Weighted, StringComparison.OrdinalIgnoreCase);
        }

        public static string NormalizeProductType(string productType)
        {
            if (string.IsNullOrWhiteSpace(productType)) return Physical;
            var t = productType.Trim().ToLowerInvariant();
            if (t == Weighted || t == Service) return t;
            return Physical;
        }

        public static ProductDomainType ToDomainType(string productType)
        {
            if (IsWeighted(productType)) return ProductDomainType.WEIGHTED;
            return ProductDomainType.GENERAL;
        }

        public static bool IsServiceProduct(SQLiteConnection con, SQLiteTransaction tran, int productId)
        {
            if (productId <= 0) return false;
            using (var cmd = new SQLiteCommand("SELECT product_type FROM Products WHERE id=@id LIMIT 1", con, tran))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                object v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value) return false;
                return IsService(v.ToString());
            }
        }
    }
}
