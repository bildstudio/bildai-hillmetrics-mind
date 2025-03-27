using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class ElementValueRequest
    {
        public int Id { get; set; }
        public string? ExtractedValue { get; set; }
        public PropertyDataType PropertyDataType { get; set; }
        public int FileDataMappingId { get; set; }
        public FileDataMapping FileDataMapping { get; set; } = null!;
        public int FinancialDataPointElementId { get; set; }
        public FinancialDataPointElement FinancialDataPointElement { get; set; } = null!;
    }

    public class ElementValuesRequest
    {
        /// <summary>
        /// A collection of element values to create
        /// </summary>
        public List<ElementValueRequest> Elements { get; set; }
    }
}