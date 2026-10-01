using Sales.Services.BarcodePlatform.Pipeline;

namespace Sales.Services.BarcodePlatform.Pipeline.Stages
{
    public class FinalMappingStage : IBarcodePipelineStage
    {
        public void Execute(BarcodePipelineContext ctx, IBarcodeDataStore store)
        {
            if (ctx == null) return;
            if (ctx.Result != null) return;

            if (ctx.Parsed != null && ctx.Parsed.Kind == BarcodeParseKind.WeightEan13)
            {
                if (ctx.ProductUnit == null)
                {
                    ctx.Result = BarcodeResolveResult.NotFound("لم يتم العثور على صنف الميزان");
                    ctx.AddTrace("Map", PipelineTraceStatus.Fail, "Missing product unit");
                    return;
                }

                if (!ctx.Qty.HasValue || ctx.Qty.Value <= 0m)
                {
                    ctx.Result = BarcodeResolveResult.Blocked("قيمة الكمية غير صالحة");
                    ctx.AddTrace("Map", PipelineTraceStatus.Fail, "Missing qty");
                    return;
                }

                string msg = null;
                if (!string.IsNullOrWhiteSpace(ctx.Warning)) msg = ctx.Warning;
                else if (ctx.Lookup != null && !string.IsNullOrWhiteSpace(ctx.Lookup.Warning)) msg = ctx.Lookup.Warning;

                ctx.Result = BarcodeResolveResult.Resolved(new BarcodeResolvedItem
                {
                    ProductUnitId = ctx.ProductUnit.ProductUnitId,
                    Qty = ctx.Qty.Value,
                    ProfileCode = ctx.Parsed.ProfileCode,
                    CanonicalCode = ctx.Parsed.CanonicalCode
                }, msg);

                ctx.AddTrace("Map", PipelineTraceStatus.Ok, "Resolved");

                return;
            }
        }
    }
}
