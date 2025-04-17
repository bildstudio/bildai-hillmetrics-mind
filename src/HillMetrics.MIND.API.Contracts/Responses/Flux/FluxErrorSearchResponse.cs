using HillMetrics.Core.Workflow;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxErrorSearchResponse
    {
        public required int Id { get; set; }
        public required int FluxId { get; set; }
        public required string FluxErrorType { get; set; }
        public required string ExternalId { get; set; }
        public required FluxActionType ActionType { get; set; }
        public required string Message { get; set; }
        public string? Metadata { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
