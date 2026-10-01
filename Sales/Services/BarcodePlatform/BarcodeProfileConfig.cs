// ============================================================
// الملف    : BarcodeProfileConfig.cs
// الغرض    : تعريف إعدادات تكوين ملفات قراءة الباركود المتنوعة
// ============================================================

using System;

namespace Sales.Services.BarcodePlatform
{
    public class BarcodeProfileRow
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public int Priority { get; set; }
        public string ConfigJson { get; set; }
    }

    public class WeightEan13ProfileConfig
    {
        public string Prefix { get; set; }
        public int PluDigits { get; set; }
        public int PriceDigits { get; set; }
        public int PriceDecimals { get; set; }

        public static WeightEan13ProfileConfig Default()
        {
            return new WeightEan13ProfileConfig
            {
                Prefix = "21",
                PluDigits = 5,
                PriceDigits = 5,
                PriceDecimals = 2
            };
        }
    }
}
