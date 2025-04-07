using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class ElementValueRequest
    {
        public int Id { get; set; }
        public string ExtractedValue { get; set; } = null!;
        public int PropertyDataTypeId { get; set; }
        public int FileDataMappingId { get; set; }
        public int FinancialDataPointElementId { get; set; }
    }
}