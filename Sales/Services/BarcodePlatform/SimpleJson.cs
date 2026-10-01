// ============================================================
// الملف    : SimpleJson.cs
// الغرض    : فئة مساعدة لتحليل وتكوين نصوص JSON الخفيفة لملفات الباركود
// ============================================================

using System;
using System.Globalization;

namespace Sales.Services.BarcodePlatform
{
    public static class SimpleJson
    {
        public static string TryGetString(string json, string key)
        {
            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key)) return null;

            // Very small JSON helper for flat objects with string values.
            // Ex: {"prefix":"21","type":"weight_ean13"}
            string token = "\"" + key.Trim() + "\"";
            int idx = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;

            idx = json.IndexOf(':', idx);
            if (idx < 0) return null;
            idx++;

            while (idx < json.Length && char.IsWhiteSpace(json[idx])) idx++;
            if (idx >= json.Length) return null;

            if (json[idx] != '\"') return null;
            idx++;

            int end = json.IndexOf('\"', idx);
            if (end < 0) return null;

            return json.Substring(idx, end - idx);
        }

        public static int TryGetInt(string json, string key, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key)) return defaultValue;

            string token = "\"" + key.Trim() + "\"";
            int idx = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return defaultValue;

            idx = json.IndexOf(':', idx);
            if (idx < 0) return defaultValue;
            idx++;

            while (idx < json.Length && (char.IsWhiteSpace(json[idx]) || json[idx] == '\"')) idx++;
            if (idx >= json.Length) return defaultValue;

            int end = idx;
            while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '-' || json[end] == '+')) end++;

            string raw = json.Substring(idx, end - idx);
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v))
                return v;

            return defaultValue;
        }

        public static bool TryGetBool(string json, string key, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key)) return defaultValue;

            string token = "\"" + key.Trim() + "\"";
            int idx = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return defaultValue;

            idx = json.IndexOf(':', idx);
            if (idx < 0) return defaultValue;
            idx++;

            while (idx < json.Length && (char.IsWhiteSpace(json[idx]) || json[idx] == '\"')) idx++;
            if (idx >= json.Length) return defaultValue;

            if (json.IndexOf("true", idx, StringComparison.OrdinalIgnoreCase) == idx) return true;
            if (json.IndexOf("false", idx, StringComparison.OrdinalIgnoreCase) == idx) return false;

            return defaultValue;
        }
    }
}
