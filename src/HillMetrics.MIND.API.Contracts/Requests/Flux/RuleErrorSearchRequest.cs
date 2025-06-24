using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Core.Rules.Abstract;
using HillMetrics.Core.Search;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    public class RuleErrorSearchRequest
    {
        public SearchCriteria<int>? Id { get; set; }
        public SearchCriteria<int>? FluxId { get; set; }
        public SearchCriteria<int>? FinancialId { get; set; }
        public SearchCriteria<int>? FluxProcessingContentId { get; set; }
        public SearchCriteria<int>? WorkflowStepId { get; set; }
        public FinancialTechnicalDataPoint? DataPoint { get; set; }
        public string? ErrorValue { get; set; }
        public string? ErrorMessage { get; set; }
        public RuleType? RuleType { get; set; }
        public string? RuleName { get; set; }
        public RuleSeverity? RuleSeverity { get; set; }
        public bool? IsProcessed { get; set; }
        public SearchCriteria<DateTime>? CreationDate { get; set; }
        public SearchCriteria<DateTime>? ProcessedDate { get; set; }

        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(CreationDate), Core.Search.SortDirection.Desc);
    }
}