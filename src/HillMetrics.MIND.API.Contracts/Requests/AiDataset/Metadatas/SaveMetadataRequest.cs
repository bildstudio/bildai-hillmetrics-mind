using HillMetrics.Core.Financial;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset.Metadatas
{
    public class SaveMetadataRequest
    {
        public int FinancialDataPointElementId { get; set; }

        public int DocumentTypeId { get; set; }

        public int LanguageId { get; set; }

        public Dictionary<FinancialDataPointElementMetadataKey, string?> Values { get; set; } = new Dictionary<FinancialDataPointElementMetadataKey, string?>();
    }
}
