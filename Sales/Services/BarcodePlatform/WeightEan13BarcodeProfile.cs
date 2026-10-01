// ============================================================
// الملف    : WeightEan13BarcodeProfile.cs
// الغرض    : ملف تعريف باركود الموازين (EAN-13 الموزون) لحساب الوزن تلقائياً
// ============================================================

using System;
using System.Globalization;
using Sales.Models;

namespace Sales.Services.BarcodePlatform
{
    public class WeightEan13BarcodeProfile : IBarcodeProfile, IBarcodeParserProfile
    {
        public string Code { get; private set; }
        public int Priority { get; private set; }
        public bool IsEnabled { get; private set; }

        private readonly WeightEan13ProfileConfig _cfg;

        public WeightEan13BarcodeProfile(string code, int priority, bool isEnabled, WeightEan13ProfileConfig cfg)
        {
            Code = code;
            Priority = priority;
            IsEnabled = isEnabled;
            _cfg = cfg ?? WeightEan13ProfileConfig.Default();
        }

        public BarcodeResolveResult TryResolve(BarcodeResolveRequest request, IBarcodeDataStore store)
        {
            // Orchestration (lookup/fallback/qty) is handled by BarcodeResolverEngine.
            // Keep TryResolve returning null so the engine can use TryParse payload.
            return null;
        }

        public BarcodeParseResult TryParse(BarcodeParseRequest request)
        {
            if (!IsEnabled) return null;
            if (request == null || string.IsNullOrWhiteSpace(request.RawCode)) return null;

            string code = request.RawCode.Trim();
            if (code.Length != 13) return null;

            if (!IsAllDigits(code))
                return new BarcodeParseResult { Kind = BarcodeParseKind.Unknown };

            if (!IsValidEan13CheckDigit(code))
                return new BarcodeParseResult { Kind = BarcodeParseKind.Unknown };

            if (string.IsNullOrWhiteSpace(_cfg.Prefix) || !_cfg.Prefix.Equals(code.Substring(0, _cfg.Prefix.Length), StringComparison.Ordinal))
                return null;

            int prefixLen = _cfg.Prefix.Length;
            int pluDigits = _cfg.PluDigits <= 0 ? 5 : _cfg.PluDigits;
            int priceDigits = _cfg.PriceDigits <= 0 ? 5 : _cfg.PriceDigits;

            int expectedBodyDigits = prefixLen + pluDigits + priceDigits;
            if (expectedBodyDigits != 12)
                return new BarcodeParseResult { Kind = BarcodeParseKind.Unknown };

            string plu = code.Substring(prefixLen, pluDigits);
            string priceRaw = code.Substring(prefixLen + pluDigits, priceDigits);

            return new BarcodeParseResult
            {
                Kind = BarcodeParseKind.WeightEan13,
                ProfileCode = Code,
                CanonicalCode = code,
                WeightEan13 = new WeightEan13Payload
                {
                    Prefix = _cfg.Prefix,
                    Plu = plu,
                    PriceRaw = priceRaw,
                    PriceDecimals = _cfg.PriceDecimals
                }
            };
        }

        private static bool IsAllDigits(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (s[i] < '0' || s[i] > '9')
                    return false;
            return true;
        }

        private static bool IsValidEan13CheckDigit(string code)
        {
            // code length 13 numeric
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = code[i] - '0';
                sum += (i % 2 == 0) ? digit : digit * 3;
            }
            int mod = sum % 10;
            int check = (10 - mod) % 10;
            int actual = code[12] - '0';
            return check == actual;
        }

        private static decimal ParseFixedPoint(string digits, int decimals)
        {
            if (string.IsNullOrWhiteSpace(digits)) return 0m;
            if (!int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v)) return 0m;
            if (decimals <= 0) return v;

            decimal scale = 1m;
            for (int i = 0; i < decimals; i++) scale *= 10m;
            return v / scale;
        }
    }
}
