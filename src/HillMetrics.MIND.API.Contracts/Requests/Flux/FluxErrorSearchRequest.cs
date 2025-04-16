using HillMetrics.Core.Search;
using HillMetrics.Core.Workflow;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    public class FluxErrorSearchRequest
    {
        public SearchCriteria<int>? FluxId { get; set; }
        public string? FluxErrorType { get; set; }
        public string? ExternalId { get; set; }
        public FluxActionType? ActionType { get; set; }
        public string? Message { get; set; }
        public string? Metadata { get; set; }

        public SearchCriteria<DateTime>? CreatedAt { get; set; }

        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(FluxId), Core.Search.SortDirection.Ascending);
    }
}
