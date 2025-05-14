using HillMetrics.Core.Extensions;
using HillMetrics.MIND.API.Contracts.Responses.Clients;
using HillMetrics.Normalized.Domain.Contracts.Clients;

namespace HillMetrics.MIND.API.Mappers
{
    public static class ClientEntityMappings
    {
        public static ClientDto FromDomain(this ClientEntity entity)
        {
            return new ClientDto(entity.Id, entity.Name, entity.Email);
        }

        public static List<ClientDto> FromDomainList(this List<ClientEntity> entities)
        {
            if (entities.IsNullOrEmpty())
                return [];

            return entities.Select(FromDomain).ToList();
        }
    }
}
