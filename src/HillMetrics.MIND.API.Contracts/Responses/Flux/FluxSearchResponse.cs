using HillMetrics.Core;
using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxSearchResponse
    {
        public required int FluxId { get; set; }
        public required string Name { get; set; }
        public FinancialType? FinancialType { get; set; }
        public required FluxType FluxType { get; set; }
        public required FluxState FluxState { get; set; }
        public DateTime? LastFetching { get; set; }
        public StatusProcess? LastFetchingStatus { get; set; }
        public int LastFetchingErrorCount { get; set; }
        public DateTime? LastProcessing { get; set; }
        public StatusProcess? LastProcessingStatus { get; set; }
        public int LastProcessingErrorCount { get; set; }
        public int MappingsCount { get; set; }
    }
}
