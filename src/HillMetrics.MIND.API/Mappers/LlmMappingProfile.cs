using AutoMapper;
using HillMetrics.Core.Search;
using HillMetrics.MIND.API.Contracts.Requests.Common;
using HillMetrics.MIND.API.Contracts.Requests.Llm;
using HillMetrics.MIND.API.Contracts.Responses.Llm;
using HillMetrics.Normalized.Domain.Contracts.AI;
using HillMetrics.Normalized.Domain.Contracts.AI.Commands;

namespace HillMetrics.MIND.API.Mappers
{
    public class LlmMappingProfile : Profile
    {
        public LlmMappingProfile()
        {
            CreateMap<AiLlmEntityDomain, AiLlmEntityDto>(MemberList.Destination);

            CreateMap<AiLlmHistoryEntity, AiLlmHistoryEntityDto>(MemberList.Destination);
            
            CreateMap<AiModelPrompt, AiModelPromptDto>(MemberList.Destination);

            CreateMap<PromptSearchRequest, AiModelPromptSearch>(MemberList.Destination)
                .ForMember(s => s.Sorting, opt => opt.MapFrom(
                                        src => src.Sorting != null ? new List<Sorting> { new Sorting(src.Sorting.Field, Enum.Parse<Core.Search.SortDirection>(src.Sorting.Direction.ToString(),true))} : new List<Sorting>()));

            CreateMap<PaginationDto, Pagination>(MemberList.Destination);

            CreateMap<SortingDto, Sorting>(MemberList.Destination);

            CreateMap<AiModelPromptAnalyzedResult, AiModelPromptAnalyzedResultDto>(MemberList.Destination);
            CreateMap<AiModelPromptLlmResult, AiModelPromptLlmResultDto>(MemberList.Destination);
        }
    }
}
