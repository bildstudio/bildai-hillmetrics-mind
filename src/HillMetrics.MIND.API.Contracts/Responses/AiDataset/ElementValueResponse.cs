using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset
{
    public class ElementValueResponse
    {
        public int Id { get; set; }
        public string? ExtractedValue { get; set; }
        public PropertyMapping PropertyDataType { get; set; }
        public int FileDataMappingId { get; set; }
        public FileDataMapping FileDataMapping { get; set; } = null!;
        public int FinancialDataPointElementId { get; set; }
        public FinancialDataPointElement FinancialDataPointElement { get; set; } = null!;
    }

    public class ElementValueListResponse
    {
        /// <summary>
        /// List of element values
        /// </summary>
        public List<ElementValueResponse> Elements { get; set; }
    }
}