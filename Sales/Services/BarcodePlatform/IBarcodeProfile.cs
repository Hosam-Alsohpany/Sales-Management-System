// ============================================================
// الملف    : IBarcodeProfile.cs
// الغرض    : واجهة ملف تعريف قراءة الباركود وقواعد تفسير الأرقام
// ============================================================

namespace Sales.Services.BarcodePlatform
{
    public interface IBarcodeProfile
    {
        string Code { get; }
        int Priority { get; }
        bool IsEnabled { get; }

        BarcodeResolveResult TryResolve(BarcodeResolveRequest request, IBarcodeDataStore store);
    }
}
