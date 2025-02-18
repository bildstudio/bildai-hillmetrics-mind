using AutoMapper;
using HillMetrics.MIND.API.Contracts.Responses.Llm;
using HillMetrics.Normalized.Domain.Contracts.AI;

namespace HillMetrics.MIND.API.Mappers
{
    public class LlmMappingProfile : Profile
    {
        public LlmMappingProfile()
        {
            CreateMap<AiLlmEntity, AiLlmEntityDto>(MemberList.Destination);
            CreateMap<AiLlmHistoryEntity, AiLlmHistoryEntityDto>(MemberList.Destination);
        }
    }
}
