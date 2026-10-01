// ============================================================
// الملف    : BarcodeLookupContracts.cs
// الغرض    : تعريف عقود البحث عن المواد باستخدام قيم الباركود المدخلة
// ============================================================

using System;
using Sales.Models;

namespace Sales.Services.BarcodePlatform
{
    public enum BarcodeIdentityType
    {
        Normal = 0,
        Plu = 1,
        Internal = 2
    }

    public class BarcodeLookupRequest
    {
        public BarcodeIdentityType IdentityType { get; set; }
        public string Value { get; set; }

        // Migration fallback support (orchestrator-owned policy)
        public bool AllowLegacyFallback { get; set; }
        public string LegacyPrefix { get; set; }

        // Optional domain constraint (e.g. "WEIGHTED")
        public string RequiredDomainType { get; set; }
    }

    public class NormalBarcodeLookupRequest : BarcodeLookupRequest
    {
        public NormalBarcodeLookupRequest()
        {
            IdentityType = BarcodeIdentityType.Normal;
        }

        public string Barcode
        {
            get { return Value; }
            set { Value = value; }
        }
    }

    public class PluBarcodeLookupRequest : BarcodeLookupRequest
    {
        public PluBarcodeLookupRequest()
        {
            IdentityType = BarcodeIdentityType.Plu;
        }

        public string Plu
        {
            get { return Value; }
            set { Value = value; }
        }
    }

    public class BarcodeLookupResult
    {
        public ProductUnitLookup ProductUnit { get; set; }

        // Orchestrator can surface warnings (e.g. legacy fallback used)
        public string Warning { get; set; }

        public bool IsFound
        {
            get { return ProductUnit != null && ProductUnit.ProductUnitId > 0; }
        }
    }

    internal static class BarcodeIdentityTypeSql
    {
        public static string ToSql(BarcodeIdentityType t)
        {
            if (t == BarcodeIdentityType.Plu) return "PLU";
            if (t == BarcodeIdentityType.Internal) return "INTERNAL";
            return "NORMAL";
        }

        public static BarcodeIdentityType FromSql(string s)
        {
            s = (s ?? string.Empty).Trim();
            if (s.Equals("PLU", StringComparison.OrdinalIgnoreCase)) return BarcodeIdentityType.Plu;
            if (s.Equals("INTERNAL", StringComparison.OrdinalIgnoreCase)) return BarcodeIdentityType.Internal;
            return BarcodeIdentityType.Normal;
        }
    }
}
