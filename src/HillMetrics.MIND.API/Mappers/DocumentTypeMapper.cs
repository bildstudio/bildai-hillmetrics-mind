using HillMetrics.MIND.API.Contracts.Responses.AiDataset.DocumentTypes;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Mappers
{
    public static class DocumentTypeMapper
    {
        public static DocumentTypeDto FromDomain(this DocumentType entity)
        {
            return new DocumentTypeDto(entity.Id, entity.Name, entity.FinancialType);
        }

        public static List<DocumentTypeDto> FromDomains(this List<DocumentType> entities)
        {
            return entities.Select(FromDomain).ToList();
        }
    }
}