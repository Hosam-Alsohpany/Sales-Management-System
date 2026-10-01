// ============================================================
// الملف    : BarcodeContracts.cs
// الغرض    : تعريف العقود والواجهات لنظام معالجة الباركود وفكه
// ============================================================

using System;

namespace Sales.Services.BarcodePlatform
{
    public enum BarcodeResolveStatus
    {
        Unknown = 0,
        Resolved = 1,
        NotFound = 2,
        Blocked = 3
    }

    public class BarcodeResolveRequest
    {
        public string RawCode { get; set; }

        // Context
        public string UserName { get; set; }
        public string Terminal { get; set; }
    }

    public class BarcodeResolveResult
    {
        public BarcodeResolveStatus Status { get; set; }
        public string Message { get; set; }

        public BarcodeResolvedItem Item { get; set; }

        public static BarcodeResolveResult Resolved(BarcodeResolvedItem item, string message = null)
        {
            return new BarcodeResolveResult
            {
                Status = BarcodeResolveStatus.Resolved,
                Item = item,
                Message = message
            };
        }

        public static BarcodeResolveResult NotFound(string message = null)
        {
            return new BarcodeResolveResult
            {
                Status = BarcodeResolveStatus.NotFound,
                Message = message
            };
        }

        public static BarcodeResolveResult Blocked(string message)
        {
            return new BarcodeResolveResult
            {
                Status = BarcodeResolveStatus.Blocked,
                Message = message
            };
        }
    }

    public class BarcodeResolvedItem
    {
        public int ProductUnitId { get; set; }

        public decimal Qty { get; set; }

        // For debug/trace
        public string ProfileCode { get; set; }
        public string CanonicalCode { get; set; }
    }
}
