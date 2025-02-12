using HillMetrics.Core.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxIdentifyDataPointResponse
    {
        public List<FluxIdentifyDataPointItemResponse> DetectionResult { get; set; } = new List<FluxIdentifyDataPointItemResponse>();
    }

    public class FluxIdentifyDataPointItemResponse
    {
        public bool IsSuccess { get; set; }
        public string? DetectorName { get; set; }
        public FinancialDataPoint? DataPoint { get; set; }
        public string? MetadataMapping { get; set; }
        public int? Score { get; set; }
    }
}
