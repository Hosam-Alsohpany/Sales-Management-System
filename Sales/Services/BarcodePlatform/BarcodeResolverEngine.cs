using System;
using System.Collections.Generic;
using System.Linq;
using Sales.Services.BarcodePlatform.Pipeline;
using Sales.Services.BarcodePlatform.Pipeline.Stages;
using Sales.Services.BarcodePlatform.Strategies;
using Sales.Utilities;

namespace Sales.Services.BarcodePlatform
{
    public class BarcodeResolverEngine
    {
        private readonly IBarcodeDataStore _store;
        private readonly List<IBarcodeProfile> _profiles;

        public BarcodeResolverEngine(IBarcodeDataStore store, IEnumerable<IBarcodeProfile> profiles)
        {
            _store = store;
            _profiles = (profiles ?? Enumerable.Empty<IBarcodeProfile>()).OrderBy(x => x.Priority).ToList();
        }

        public BarcodeResolveResult Resolve(BarcodeResolveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RawCode))
                return BarcodeResolveResult.NotFound();

            var pipeline = new DefaultBarcodeResolvePipeline(new IBarcodePipelineStage[]
            {
                new ParseStage(_profiles),
                new WeightedLookupStage(),
                new WeightedQuantityStage(new PriceEmbeddedQuantityStrategy()),
                new FinalMappingStage()
            });

            var piped = pipeline.Execute(request, _store);
            if (piped != null && piped.Status != BarcodeResolveStatus.NotFound)
            {
                if (piped.Status != BarcodeResolveStatus.Resolved)
                    Logger.LogInfo(string.Format("[BARCODE][PIPELINE] Raw={0} Status={1} Message={2}", request.RawCode, piped.Status, piped.Message));
                return piped;
            }

            foreach (var p in _profiles)
            {
                if (p == null) continue;

                BarcodeResolveResult r = null;
                try
                {
                    r = p.TryResolve(request, _store);
                }
                catch
                {
                    // Ignore profile failure: treat as no-match.
                    r = null;
                }

                if (r == null) continue;

                if (r.Status == BarcodeResolveStatus.Resolved)
                {
                    if (r.Item == null || r.Item.ProductUnitId <= 0 || r.Item.Qty <= 0m)
                        return BarcodeResolveResult.Blocked("باركود غير صالح");
                }

                return r;
            }

            Logger.LogInfo(string.Format("[BARCODE][LEGACY_TRYRESOLVE] Raw={0} Status=NotFound", request.RawCode));
            return BarcodeResolveResult.NotFound();
        }

        public static BarcodeResolverEngine CreateDefault(IBarcodeDataStore store)
        {
            var repo = new BarcodeProfilesRepository();
            List<BarcodeProfileRow> rows = repo.GetEnabledProfiles();

            var profiles = new List<IBarcodeProfile>();

            if (rows == null || rows.Count == 0)
            {
                profiles.Add(new WeightEan13BarcodeProfile("WEIGHT_EAN13", 10, true, WeightEan13ProfileConfig.Default()));
                profiles.Add(new GeneralBarcodeProfile("GENERAL", 100, true));
                return new BarcodeResolverEngine(store, profiles);
            }

            foreach (var row in rows)
            {
                if (row == null) continue;

                string code = (row.Code ?? string.Empty).Trim();
                if (code.Length == 0) continue;

                string cfg = row.ConfigJson ?? string.Empty;
                string type = SimpleJson.TryGetString(cfg, "type") ?? string.Empty;

                if (type.Equals("weight_ean13", StringComparison.OrdinalIgnoreCase) || code.Equals("WEIGHT_EAN13", StringComparison.OrdinalIgnoreCase))
                {
                    var c = new WeightEan13ProfileConfig
                    {
                        Prefix = SimpleJson.TryGetString(cfg, "prefix") ?? "21",
                        PluDigits = SimpleJson.TryGetInt(cfg, "pluDigits", 5),
                        PriceDigits = SimpleJson.TryGetInt(cfg, "priceDigits", 5),
                        PriceDecimals = SimpleJson.TryGetInt(cfg, "priceDecimals", 2)
                    };

                    profiles.Add(new WeightEan13BarcodeProfile(code, row.Priority, row.IsEnabled, c));
                    continue;
                }

                if (type.Equals("general", StringComparison.OrdinalIgnoreCase) || code.Equals("GENERAL", StringComparison.OrdinalIgnoreCase))
                {
                    profiles.Add(new GeneralBarcodeProfile(code, row.Priority, row.IsEnabled));
                    continue;
                }

                // Unknown profile type => ignore for MVP.
            }

            // Safety: if misconfigured to empty.
            if (profiles.Count == 0)
                profiles.Add(new GeneralBarcodeProfile("GENERAL", 100, true));

            return new BarcodeResolverEngine(store, profiles);
        }
    }
}
