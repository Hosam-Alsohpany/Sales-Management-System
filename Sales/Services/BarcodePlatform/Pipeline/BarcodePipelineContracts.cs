// ============================================================
// الملف    : BarcodePipelineContracts.cs
// الغرض    : تعريف واجهات مراحل خط معالجة وتفسير الباركود (Pipeline)
// ============================================================

using System;
using System.Collections.Generic;
using Sales.Models;
using Sales.Services.BarcodePlatform;

namespace Sales.Services.BarcodePlatform.Pipeline
{
    public enum PipelineTraceStatus
    {
        Info = 0,
        Ok = 1,
        Fail = 2
    }

    public class PipelineTraceEntry
    {
        public string Stage { get; set; }
        public PipelineTraceStatus Status { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [{1}] {2}", Stage, Status, Message);
        }
    }

    public class BarcodePipelineContext
    {
        public BarcodeResolveRequest ResolveRequest { get; set; }

        public BarcodeParseResult Parsed { get; set; }
        public BarcodeLookupResult Lookup { get; set; }

        public decimal? Qty { get; set; }
        public string Warning { get; set; }

        public BarcodeResolveResult Result { get; set; }

        public List<PipelineTraceEntry> Trace { get; private set; }

        public bool UsedPipeline { get; set; }
        public bool UsedLegacy { get; set; }

        public BarcodePipelineContext()
        {
            Trace = new List<PipelineTraceEntry>();
        }

        public void AddTrace(string stage, PipelineTraceStatus status, string message)
        {
            if (Trace == null) Trace = new List<PipelineTraceEntry>();
            Trace.Add(new PipelineTraceEntry
            {
                Stage = (stage ?? string.Empty).Trim(),
                Status = status,
                Message = message
            });
        }

        public bool HasResult
        {
            get { return Result != null; }
        }

        public ProductUnitLookup ProductUnit
        {
            get { return Lookup != null ? Lookup.ProductUnit : null; }
        }
    }

    public interface IBarcodePipelineStage
    {
        void Execute(BarcodePipelineContext ctx, IBarcodeDataStore store);
    }
}
