using Sales.Services.BarcodePlatform.Pipeline;

namespace Sales.Services.BarcodePlatform.Pipeline.Stages
{
    public class WeightedQuantityStage : IBarcodePipelineStage
    {
        private readonly IQuantityResolverStrategy _qty;

        public WeightedQuantityStage(IQuantityResolverStrategy qty)
        {
            _qty = qty;
        }

        public void Execute(BarcodePipelineContext ctx, IBarcodeDataStore store)
        {
            if (ctx == null) return;
            if (ctx.Result != null) return;
            if (ctx.Parsed == null) return;
            if (ctx.Parsed.Kind != BarcodeParseKind.WeightEan13 || ctx.Parsed.WeightEan13 == null) return;

            if (ctx.Lookup == null || ctx.Lookup.ProductUnit == null)
            {
                ctx.Result = BarcodeResolveResult.NotFound("لم يتم العثور على صنف الميزان");
                ctx.AddTrace("Qty", PipelineTraceStatus.Fail, "Lookup missing product unit");
                return;
            }

            if (_qty == null)
            {
                ctx.Result = BarcodeResolveResult.Blocked("Barcode quantity strategy missing");
                ctx.AddTrace("Qty", PipelineTraceStatus.Fail, "Quantity strategy missing");
                return;
            }

            var qtyResult = _qty.TryResolve(ctx.Parsed, ctx.Lookup.ProductUnit);
            if (qtyResult == null || qtyResult.Qty <= 0m)
            {
                ctx.Result = BarcodeResolveResult.Blocked(qtyResult != null ? qtyResult.Message : "قيمة الكمية غير صالحة");
                ctx.AddTrace("Qty", PipelineTraceStatus.Fail, qtyResult != null ? qtyResult.Message : "Invalid qty");
                return;
            }

            ctx.Qty = qtyResult.Qty;
            if (!string.IsNullOrWhiteSpace(qtyResult.Warning))
                ctx.Warning = qtyResult.Warning;

            ctx.AddTrace("Qty", PipelineTraceStatus.Ok, string.Format("Qty={0}", qtyResult.Qty));
        }
    }
}
