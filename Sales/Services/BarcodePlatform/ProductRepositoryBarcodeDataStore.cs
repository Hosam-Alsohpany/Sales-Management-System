// ============================================================
// الملف    : ProductRepositoryBarcodeDataStore.cs
// الغرض    : مخزن بيانات الباركود المعتمد على مستودع المنتجات الحالي
// ============================================================

using Sales.Models;
using Sales.Repositories;
using Sales.Services.ProductDomains;

namespace Sales.Services.BarcodePlatform
{
    public class ProductRepositoryBarcodeDataStore : IBarcodeDataStoreWithDomains, IBarcodeLookupStore
    {
        private readonly ProductRepository _repo;
        private readonly ProductDomainsRepository _domainsRepo;

        public ProductRepositoryBarcodeDataStore(ProductRepository repo)
        {
            _repo = repo;
            _domainsRepo = new ProductDomainsRepository();
        }

        public ProductUnitLookup GetProductUnitByBarcode(string barcode)
        {
            var r = Find(new NormalBarcodeLookupRequest
            {
                Barcode = barcode
            });

            return r != null ? r.ProductUnit : null;
        }

        public ProductUnitLookup GetProductUnitByWeightPlu(string pluBarcode)
        {
            var r = Find(new PluBarcodeLookupRequest
            {
                Plu = pluBarcode
            });

            return r != null ? r.ProductUnit : null;
        }

        public BarcodeLookupResult Find(BarcodeLookupRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Value))
                return new BarcodeLookupResult();

            string value = request.Value.Trim();
            string barcodeType = BarcodeIdentityTypeSql.ToSql(request.IdentityType);

            ProductUnitLookup pu = _repo.GetProductUnitByBarcodeType(barcodeType, value);
            if (pu == null && request.AllowLegacyFallback && request.IdentityType == BarcodeIdentityType.Plu)
            {
                string prefix = (request.LegacyPrefix ?? string.Empty).Trim();
                string legacy = prefix + value;
                if (!string.IsNullOrWhiteSpace(legacy))
                {
                    pu = _repo.GetProductUnitByBarcode(legacy);
                    if (pu != null)
                    {
                        return new BarcodeLookupResult
                        {
                            ProductUnit = pu,
                            Warning = "Legacy PLU fallback used"
                        };
                    }
                }
            }

            if (pu == null)
                return new BarcodeLookupResult();

            if (!string.IsNullOrWhiteSpace(request.RequiredDomainType))
            {
                try
                {
                    string dt = GetProductDomainTypeOrNull(pu.ProductId);
                    if (!string.IsNullOrWhiteSpace(dt) && !dt.Equals(request.RequiredDomainType, System.StringComparison.OrdinalIgnoreCase))
                        return new BarcodeLookupResult();
                }
                catch
                {
                    // ignore
                }
            }

            return new BarcodeLookupResult { ProductUnit = pu };
        }

        public string GetProductDomainTypeOrNull(int productId)
        {
            try
            {
                var s = _domainsRepo.GetDomainSnapshotOrNull(productId);
                if (s == null) return null;
                return s.DomainType.ToString();
            }
            catch
            {
                return null;
            }
        }

        public string GetProductDomainConfigJsonOrNull(int productId)
        {
            try
            {
                var s = _domainsRepo.GetDomainSnapshotOrNull(productId);
                return s != null ? s.ConfigJson : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
