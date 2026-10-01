using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sales.Services.BarcodePlatform;

namespace Sales.Services.ProductDomains
{
    public enum ProductDomainType
    {
        GENERAL,
        WEIGHTED,
        FASHION,
        INTERNAL
    }

    public interface IProductDomainConfig
    {
        int Version { get; set; }
    }

    public class GeneralDomainConfig : IProductDomainConfig
    {
        public int Version { get; set; } = 1;

        public static GeneralDomainConfig Template() => new GeneralDomainConfig { Version = 1 };
    }

    public class WeightedDomainConfig : IProductDomainConfig
    {
        public int Version { get; set; } = 1;
        public string Profile { get; set; } = "weight_ean13";
        public List<string> Prefixes { get; set; } = new List<string>();
        public int PluLength { get; set; }
        public int QtyDigits { get; set; }
        public int QtyPow10 { get; set; }

        public static WeightedDomainConfig Template()
        {
            return new WeightedDomainConfig
            {
                Version = 1,
                Profile = "weight_ean13",
                Prefixes = new List<string> { "21" },
                PluLength = 5,
                QtyDigits = 5,
                QtyPow10 = 3
            };
        }
    }

    public static class ProductDomainConfigSerializer
    {
        public static string CreateTemplateJson(ProductDomainType domain)
        {
            switch (domain)
            {
                case ProductDomainType.WEIGHTED:
                    return SerializeWeighted(WeightedDomainConfig.Template());
                case ProductDomainType.GENERAL:
                default:
                    return SerializeGeneral(GeneralDomainConfig.Template());
            }
        }

        public static bool TryValidateAndNormalize(ProductDomainType domain, string json, out string normalizedJson, out string error)
        {
            normalizedJson = null;
            error = null;

            if (string.IsNullOrWhiteSpace(json))
            {
                error = "config_json فارغ";
                return false;
            }

            string trimmed = json.Trim();
            if (trimmed == "{}")
            {
                error = "config_json لا يمكن أن يكون {}";
                return false;
            }

            try
            {
                switch (domain)
                {
                    case ProductDomainType.WEIGHTED:
                    {
                        var cfg = ParseWeighted(trimmed);
                        if (cfg == null)
                        {
                            error = "WEIGHTED config غير صالح";
                            return false;
                        }

                        NormalizeWeighted(cfg);
                        error = ValidateWeighted(cfg);
                        if (!string.IsNullOrWhiteSpace(error))
                            return false;

                        normalizedJson = SerializeWeighted(cfg);
                        return true;
                    }

                    case ProductDomainType.GENERAL:
                    default:
                    {
                        var cfg = ParseGeneral(trimmed);
                        if (cfg == null)
                        {
                            error = "GENERAL config غير صالح";
                            return false;
                        }

                        if (cfg.Version <= 0) cfg.Version = 1;
                        normalizedJson = SerializeGeneral(cfg);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                error = "JSON غير صالح: " + ex.Message;
                return false;
            }
        }

        private static GeneralDomainConfig ParseGeneral(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return new GeneralDomainConfig
            {
                Version = SimpleJson.TryGetInt(json, "version", 1)
            };
        }

        private static WeightedDomainConfig ParseWeighted(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;

            var cfg = new WeightedDomainConfig
            {
                Version = SimpleJson.TryGetInt(json, "version", 1),
                Profile = SimpleJson.TryGetString(json, "profile") ?? "weight_ean13",
                PluLength = SimpleJson.TryGetInt(json, "plu_length", 0),
                QtyDigits = SimpleJson.TryGetInt(json, "qty_digits", 0),
                QtyPow10 = SimpleJson.TryGetInt(json, "qty_pow10", 0),
                Prefixes = TryGetStringArray(json, "prefixes") ?? new List<string>()
            };

            return cfg;
        }

        private static void NormalizeWeighted(WeightedDomainConfig cfg)
        {
            if (cfg == null) return;
            if (cfg.Version <= 0) cfg.Version = 1;
            if (string.IsNullOrWhiteSpace(cfg.Profile)) cfg.Profile = "weight_ean13";
            if (cfg.Prefixes == null) cfg.Prefixes = new List<string>();

            cfg.Prefixes = cfg.Prefixes
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.Ordinal)
                .OrderBy(p => p, StringComparer.Ordinal)
                .ToList();
        }

        private static string ValidateWeighted(WeightedDomainConfig cfg)
        {
            if (cfg == null) return "WEIGHTED config غير صالح";

            if (cfg.PluLength <= 0 || cfg.PluLength > 10)
                return "plu_length غير صحيح";

            if (cfg.QtyDigits <= 0 || cfg.QtyDigits > 10)
                return "qty_digits غير صحيح";

            if (cfg.QtyPow10 < 0 || cfg.QtyPow10 > 6)
                return "qty_pow10 غير صحيح";

            return null;
        }

        private static string SerializeGeneral(GeneralDomainConfig cfg)
        {
            if (cfg == null) cfg = GeneralDomainConfig.Template();
            if (cfg.Version <= 0) cfg.Version = 1;

            return "{\n" +
                   "  \"version\": " + cfg.Version.ToString(CultureInfo.InvariantCulture) + "\n" +
                   "}";
        }

        private static string SerializeWeighted(WeightedDomainConfig cfg)
        {
            if (cfg == null) cfg = WeightedDomainConfig.Template();
            NormalizeWeighted(cfg);

            var sb = new StringBuilder();
            sb.Append("{\n");
            sb.Append("  \"version\": ").Append(cfg.Version.ToString(CultureInfo.InvariantCulture)).Append(",\n");
            sb.Append("  \"profile\": \"").Append(Escape(cfg.Profile)).Append("\",\n");
            sb.Append("  \"prefixes\": [");
            for (int i = 0; i < cfg.Prefixes.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append("\"").Append(Escape(cfg.Prefixes[i])).Append("\"");
            }
            sb.Append("],\n");
            sb.Append("  \"plu_length\": ").Append(cfg.PluLength.ToString(CultureInfo.InvariantCulture)).Append(",\n");
            sb.Append("  \"qty_digits\": ").Append(cfg.QtyDigits.ToString(CultureInfo.InvariantCulture)).Append(",\n");
            sb.Append("  \"qty_pow10\": ").Append(cfg.QtyPow10.ToString(CultureInfo.InvariantCulture)).Append("\n");
            sb.Append("}");
            return sb.ToString();
        }

        private static string Escape(string s)
        {
            if (s == null) return string.Empty;
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static List<string> TryGetStringArray(string json, string key)
        {
            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key)) return null;

            string token = "\"" + key.Trim() + "\"";
            int idx = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;

            idx = json.IndexOf(':', idx);
            if (idx < 0) return null;
            idx++;

            while (idx < json.Length && char.IsWhiteSpace(json[idx])) idx++;
            if (idx >= json.Length || json[idx] != '[') return null;
            idx++;

            var list = new List<string>();
            while (idx < json.Length)
            {
                while (idx < json.Length && char.IsWhiteSpace(json[idx])) idx++;
                if (idx >= json.Length) break;
                if (json[idx] == ']') break;

                if (json[idx] == ',') { idx++; continue; }
                if (json[idx] != '"')
                {
                    // only support string arrays in MVP
                    break;
                }

                idx++; // skip quote
                int end = json.IndexOf('"', idx);
                if (end < 0) break;

                string v = json.Substring(idx, end - idx);
                list.Add(v);
                idx = end + 1;
            }

            return list;
        }
    }

    public class ProductDomainSnapshot
    {
        public ProductDomainType DomainType { get; set; }
        public string ConfigJson { get; set; }
    }

    public interface IProductDomainsReadStore
    {
        ProductDomainSnapshot GetDomainOrNull(int productId);
    }
}
