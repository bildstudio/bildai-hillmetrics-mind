using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset.Metadatas;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Mappers
{
    public static class FinancialDataPointElementMetadataMapper
    {
        public static FinancialDataPointElementMetadataDto FromDomain(this FinancialDataPointElementMetadata entity)
        {
            return new FinancialDataPointElementMetadataDto
            {
                LanguageId = entity.LanguageId,
                DocumentTypeId = entity.DocumentTypeId,
                ElementId = entity.ElementId,
                Values = entity.Values
            };
        }

        public static List<FinancialDataPointElementMetadataDto> FromDomainsList(this List<FinancialDataPointElementMetadata> entities)
        {
            return entities.Select(FromDomain).ToList();
        }
    }
}
