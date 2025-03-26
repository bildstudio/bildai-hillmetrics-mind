using System.ComponentModel.DataAnnotations;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset;

public class CreateElementValuesRequest
{
    [Required]
    public ICollection<ElementValueData> ElementValues { get; set; } = new List<ElementValueData>();

    public class ElementValueData
    {
        [Required]
        public int FileDataMappingId { get; set; }

        [Required]
        public int FinancialDataPointElementId { get; set; }

        public string? ExtractedValue { get; set; }

        [Required]
        public PropertyDataType PropertyDataType { get; set; }
    }
}