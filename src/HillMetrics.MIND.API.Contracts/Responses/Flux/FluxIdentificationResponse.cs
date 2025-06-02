using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxIdentificationResponse
    {
        public int FluxIdentificationHistoryId { get; set; }

        public List<IdentificationItemResponse> DetectionResult { get; set; } = new List<IdentificationItemResponse>();
    }

    public class IdentificationItemResponse
    {
        public bool IsSuccess { get; set; }
        public string? DetectorName { get; set; }
        public FinancialDataPoint? DataPoint { get; set; }
        public string? MetadataMapping { get; set; }
        public int? Score { get; set; }
        public int? FinancialDataPointId { get; set; }
    }
}
