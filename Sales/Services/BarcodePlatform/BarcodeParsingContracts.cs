// ============================================================
// الملف    : BarcodeParsingContracts.cs
// الغرض    : تعريف عقود تفكيك الباركودات الموزونة لاستخلاص السعر والوزن
// ============================================================

namespace Sales.Services.BarcodePlatform
{
    public class BarcodeParseRequest
    {
        public string RawCode { get; set; }
    }

    public enum BarcodeParseKind
    {
        Unknown = 0,
        Normal = 1,
        WeightEan13 = 2
    }

    public class BarcodeParseResult
    {
        public BarcodeParseKind Kind { get; set; }
        public string ProfileCode { get; set; }
        public string CanonicalCode { get; set; }

        public WeightEan13Payload WeightEan13 { get; set; }
    }

    public class WeightEan13Payload
    {
        public string Prefix { get; set; }
        public string Plu { get; set; }
        public string PriceRaw { get; set; }
        public int PriceDecimals { get; set; }
    }

    public interface IBarcodeParserProfile
    {
        string Code { get; }
        int Priority { get; }
        bool IsEnabled { get; }

        BarcodeParseResult TryParse(BarcodeParseRequest request);
    }
}
