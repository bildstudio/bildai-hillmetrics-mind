using HillMetrics.MIND.API.Contracts.Responses.Languages;
using HillMetrics.Normalized.Domain.Contracts.Languages;

namespace HillMetrics.MIND.API.Mappers
{
    public static class LanguageMapper
    {
        public static LanguageDto FromDomain(this Language entity)
        {
            return new LanguageDto(entity.Id, entity.Name, entity.TwoLetterCode, entity.IsActive);
        }

        public static List<LanguageDto> FromDomains(this List<Language> entities)
        {
            return entities.Select(FromDomain).ToList();
        }
    }
}