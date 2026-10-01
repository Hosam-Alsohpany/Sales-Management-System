// ============================================================
// الملف    : GeneralBarcodeProfile.cs
// الغرض    : ملف تعريف الباركود العادي لقراءة المنتجات ذات الكود الثابت
// ============================================================

using Sales.Models;

namespace Sales.Services.BarcodePlatform
{
    public class GeneralBarcodeProfile : IBarcodeProfile
    {
        public string Code { get; private set; }
        public int Priority { get; private set; }
        public bool IsEnabled { get; private set; }

        public GeneralBarcodeProfile(string code, int priority, bool isEnabled)
        {
            Code = code;
            Priority = priority;
            IsEnabled = isEnabled;
        }

        public BarcodeResolveResult TryResolve(BarcodeResolveRequest request, IBarcodeDataStore store)
        {
            if (!IsEnabled) return null;
            if (request == null || string.IsNullOrWhiteSpace(request.RawCode))
                return null;

            string code = request.RawCode.Trim();
            if (code.Length == 0) return null;

            ProductUnitLookup pu = store.GetProductUnitByBarcode(code);
            if (pu == null) return null;

            return BarcodeResolveResult.Resolved(new BarcodeResolvedItem
            {
                ProductUnitId = pu.ProductUnitId,
                Qty = 1m,
                ProfileCode = Code,
                CanonicalCode = code
            });
        }
    }
}
