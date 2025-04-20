using HillMetrics.Core.Financial;
using HillMetrics.Core;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxFetchingSearchResponse
    {
        public int Id { get; set; }
        public int FluxId { get; set; }
        public required DateTime FetchingDate { get; set; }
        public required int NbContent { get; set; }
        public required int NbProcessing { get; set; }
        public string? ExternalDataId { get; set; }
        public string? Metadata { get; set; }
        public StatusProcess? ContentStatus { get; set; }
        public string? ContentName { get; set; }
        public string? RawId { get; set; }
        public Guid WorkflowId { get; set; }
    }
}
