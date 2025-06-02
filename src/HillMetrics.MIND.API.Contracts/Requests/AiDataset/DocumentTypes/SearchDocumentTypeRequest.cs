using HillMetrics.Core.Financial;
using HillMetrics.Core.Search;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset.DocumentTypes
{
    public class SearchDocumentTypeRequest
    {
        public string? Name { get; set; } = null!;
        public FinancialType? FinancialType { get; set; }
        public Pagination Pagination { get; set; } = Pagination.Default;
    }
}
