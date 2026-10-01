// ============================================================
// الملف    : IBarcodeDataStore.cs
// الغرض    : واجهة مخزن بيانات الباركود للبحث والتعرف على المواد
// ============================================================

using Sales.Models;

namespace Sales.Services.BarcodePlatform
{
    public interface IBarcodeDataStore
    {
        ProductUnitLookup GetProductUnitByBarcode(string barcode);

        // For weight profile: map PLU -> barcode -> ProductUnit
        ProductUnitLookup GetProductUnitByWeightPlu(string pluBarcode);
    }

    // Optional contract-based lookup API to avoid repository explosion.
    // Resolver/orchestrator can use it when available.
    public interface IBarcodeLookupStore
    {
        BarcodeLookupResult Find(BarcodeLookupRequest request);
    }

    public interface IBarcodeDataStoreWithDomains : IBarcodeDataStore
    {
        string GetProductDomainTypeOrNull(int productId);
        string GetProductDomainConfigJsonOrNull(int productId);
    }
}
