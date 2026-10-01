// ============================================================
// الملف    : ParseStage.cs
// الغرض    : مرحلة تفكيك وتحليل سلسلة الباركود للميزان واستخراج كود المادة
// ============================================================

using System.Collections.Generic;
using Sales.Services.BarcodePlatform.Pipeline;

namespace Sales.Services.BarcodePlatform.Pipeline.Stages
{
    public class ParseStage : IBarcodePipelineStage
    {
        private readonly IEnumerable<IBarcodeProfile> _profiles;

        public ParseStage(IEnumerable<IBarcodeProfile> profiles)
        {
            _profiles = profiles;
        }

        public void Execute(BarcodePipelineContext ctx, IBarcodeDataStore store)
        {
            if (ctx == null || ctx.ResolveRequest == null) return;
            if (ctx.Parsed != null) return;

            string raw = ctx.ResolveRequest.RawCode;
            if (string.IsNullOrWhiteSpace(raw))
            {
                ctx.Result = BarcodeResolveResult.NotFound();
                ctx.AddTrace("Parse", PipelineTraceStatus.Fail, "Empty raw code");
                return;
            }

            if (_profiles == null) return;

            foreach (var p in _profiles)
            {
                var parser = p as IBarcodeParserProfile;
                if (parser == null) continue;
                if (!parser.IsEnabled) continue;

                BarcodeParseResult parsed = null;
                try
                {
                    parsed = parser.TryParse(new BarcodeParseRequest { RawCode = raw });
                }
                catch
                {
                    parsed = null;
                }

                if (parsed == null) continue;
                if (parsed.Kind == BarcodeParseKind.Unknown) continue;

                ctx.Parsed = parsed;
                ctx.AddTrace("Parse", PipelineTraceStatus.Ok, string.Format("Matched {0} via {1}", parsed.Kind, parsed.ProfileCode));
                return;
            }

            ctx.AddTrace("Parse", PipelineTraceStatus.Info, "No parser matched");
        }
    }
}
