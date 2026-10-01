using Sales.Services.BarcodePlatform.Pipeline;

namespace Sales.Services.BarcodePlatform.Pipeline.Stages
{
    public class WeightedLookupStage : IBarcodePipelineStage
    {
        public void Execute(BarcodePipelineContext ctx, IBarcodeDataStore store)
        {
            if (ctx == null || store == null) return;
            if (ctx.Parsed == null) return;
            if (ctx.Parsed.Kind != BarcodeParseKind.WeightEan13 || ctx.Parsed.WeightEan13 == null) return;
            if (ctx.Lookup != null) return;

            var lookupStore = store as IBarcodeLookupStore;
            if (lookupStore == null)
            {
                ctx.Result = BarcodeResolveResult.Blocked("باركود ميزان: طبقة lookup غير مدعومة");
                ctx.AddTrace("Lookup", PipelineTraceStatus.Fail, "IBarcodeLookupStore not available");
                return;
            }

            var lookup = lookupStore.Find(new Sales.Services.BarcodePlatform.PluBarcodeLookupRequest
            {
                Plu = ctx.Parsed.WeightEan13.Plu,
                AllowLegacyFallback = true,
                LegacyPrefix = ctx.Parsed.WeightEan13.Prefix,
                RequiredDomainType = "WEIGHTED"
            });

            ctx.Lookup = lookup;

            if (lookup == null || !lookup.IsFound)
            {
                ctx.AddTrace("Lookup", PipelineTraceStatus.Fail, "PLU not found");
                return;
            }

            if (!string.IsNullOrWhiteSpace(lookup.Warning))
                ctx.AddTrace("Lookup", PipelineTraceStatus.Ok, lookup.Warning);
            else
                ctx.AddTrace("Lookup", PipelineTraceStatus.Ok, "PLU found");
        }
    }
}
