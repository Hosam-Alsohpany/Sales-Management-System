// ============================================================
// الملف    : DefaultBarcodeResolvePipeline.cs
// الغرض    : خط معالجة الباركود الافتراضي لتمرير الباركود عبر عدة مراحل
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Sales.Utilities;

namespace Sales.Services.BarcodePlatform.Pipeline
{
    public class DefaultBarcodeResolvePipeline
    {
        private readonly List<IBarcodePipelineStage> _stages;

        public DefaultBarcodeResolvePipeline(IEnumerable<IBarcodePipelineStage> stages)
        {
            _stages = (stages ?? Enumerable.Empty<IBarcodePipelineStage>()).ToList();
        }

        public BarcodeResolveResult Execute(BarcodeResolveRequest request, IBarcodeDataStore store)
        {
            var ctx = new BarcodePipelineContext { ResolveRequest = request };
            ctx.UsedPipeline = true;

            foreach (var s in _stages)
            {
                if (s == null) continue;
                if (ctx.HasResult) break;

                try
                {
                    s.Execute(ctx, store);
                }
                catch
                {
                    // stage failure -> treat as no-match
                }
            }

            var r = ctx.Result ?? BarcodeResolveResult.NotFound();
            if (r.Status != BarcodeResolveStatus.Resolved)
            {
                if (ctx.Trace != null && ctx.Trace.Count > 0)
                {
                    Logger.LogInfo(string.Format("[BARCODE][PIPELINE_TRACE] Raw={0} Status={1}", request != null ? request.RawCode : string.Empty, r.Status));
                    for (int i = 0; i < ctx.Trace.Count; i++)
                        Logger.LogInfo("  " + ctx.Trace[i]);
                }
            }

            return r;
        }
    }
}
