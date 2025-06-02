using HillMetrics.Core.Financial;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset.Metadatas
{
    public class DeleteMetadataRequest
    {
        public int FinancialDataPointElementId { get; set; }

        public int DocumentTypeId { get; set; }

        public int LanguageId { get; set; }

        public List<FinancialDataPointElementMetadataKey> KeysToDelete { get; set; } = [];
    }
}
