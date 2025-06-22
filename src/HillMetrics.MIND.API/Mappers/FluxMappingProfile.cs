using HillMetrics.Core.Time.Trigger;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using HillMetrics.Normalized.Domain.Contracts.Providing;
using AutoMapper;
using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Get;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.Normalized.Domain.Contracts.Providing.Source.Cqrs.Get;
using HillMetrics.MIND.API.Contracts.Responses.Source;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Process;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Collect;
using static HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Process.ProcessFluxCommandResult;
using static HillMetrics.MIND.API.Contracts.Responses.Flux.FluxForceProcessResponse;

namespace HillMetrics.MIND.API.Mappers
{
    public class FluxMappingProfile : Profile
    {
        public FluxMappingProfile()
        {
            CreateMap<FluxQueryResult, FluxResponse>()
                .ForMember(dest => dest.HasCustomFetching, opt => opt.MapFrom(src => src.HasCustomFetching))
                .ForMember(dest => dest.HasCustomProcessing, opt => opt.MapFrom(src => src.HasCustomProcessing))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Flux.Id))
                .ForMember(dest => dest.FluxType, opt => opt.MapFrom(src => src.Flux.FluxType))
                .ForMember(dest => dest.FinancialType, opt => opt.MapFrom(src => src.Flux.FinancialType))
                .ForMember(dest => dest.FluxName, opt => opt.MapFrom(src => src.Flux.FluxName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Flux.Description))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Flux.Comment))
                .ForMember(dest => dest.FluxState, opt => opt.MapFrom(src => src.Flux.FluxState))
                .ForMember(dest => dest.FetchTriggerPeriod, opt => opt.MapFrom(src => src.Flux.FetchTriggerPeriod))
                .ForMember(dest => dest.ProcessTriggerPeriod, opt => opt.MapFrom(src => src.Flux.ProcessTriggerPeriod))
                .ForMember(dest => dest.CanHaveConcurrencyMultiFetching, opt => opt.MapFrom(src => src.Flux.CanHaveConcurrencyMultiFetching))
                .ForMember(dest => dest.FluxMetadata, opt => opt.MapFrom(src => src.Flux.FluxMetadata))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Flux.Source))
                .ForMember(dest => dest.FluxProcessingHistory, opt => opt.MapFrom(src => src.Flux.FluxProcessingHistory))
                .ForMember(dest => dest.FluxIdentificationHistory, opt => opt.MapFrom(src => src.Flux.FluxFetchingHistory))
                .ForMember(dest => dest.FluxErrors, opt => opt.MapFrom(src => src.Flux.FluxErrors))
                .ForMember(dest => dest.FinancialDataPoints, opt => opt.MapFrom(src => src.Flux.FinancialDataPoints));

            CreateMap<FluxProvider, FluxResponse>().ReverseMap();

            CreateMap<TriggerPeriod, TriggerPeriodDto>().ReverseMap();

            CreateMap<SourceProvider, SourceProviderDto>().ReverseMap();
            CreateMap<FluxProcessingHistory, FluxProcessingHistoryDto>().ReverseMap();
            CreateMap<FluxProcessingContentHistory, FluxProcessingContentHistoryDto>().ReverseMap();
            CreateMap<FluxFetchingHistory, FluxIdentificationHistoryDto>().ReverseMap();
            CreateMap<FluxFetchingContentHistory, FluxIdentificationContentHistoryDto>().ReverseMap();
            CreateMap<FluxErrors, FluxErrorsDto>().ReverseMap();
            CreateMap<FluxFinancialDataPoint, FluxFinancialDataPointDto>().ReverseMap();

            CreateMap<FetchFluxCommandResult, FluxForceFetchResponse>().ReverseMap();
            CreateMap<ProcessFluxCommandResult, FluxForceProcessResponse>().ReverseMap();
            CreateMap<ProcessFluxCommandResultItem, ProcessFluxCommandResultItemDto>().ReverseMap();

            CreateMap<FluxMetadataDto, FluxMetadata>()
                .Include<FluxMetadataMailDto, FluxMetadataMail>()
                .Include<FluxMetadataDownloadDto, FluxMetadataDownload>()
                .Include<FluxMetadataApiDto, FluxMetadataApi>()
                .Include<FluxMetadataFileLocationDto, FluxMetadataFileLocation>()
                .Include<FluxMetadataManualDto, FluxMetadataManual>();

            CreateMap<FluxMetadata, FluxMetadataDto>()
                .Include<FluxMetadataMail, FluxMetadataMailDto>()
                .Include<FluxMetadataDownload, FluxMetadataDownloadDto>()
                .Include<FluxMetadataApi, FluxMetadataApiDto>()
                .Include<FluxMetadataFileLocation, FluxMetadataFileLocationDto>()
                .Include<FluxMetadataManual, FluxMetadataManualDto>();

            CreateMap<FluxMetadataMail, FluxMetadataMailDto>().ReverseMap();
            CreateMap<FluxMetadataDownload, FluxMetadataDownloadDto>().ReverseMap();
            CreateMap<FluxMetadataApi, FluxMetadataApiDto>().ReverseMap();
            CreateMap<FluxMetadataFileLocation, FluxMetadataFileLocationDto>().ReverseMap();
            CreateMap<FluxMetadataManual, FluxMetadataManualDto>().ReverseMap();

            CreateMap<FluxRuleSettings, FluxRuleSettingsDto>().ReverseMap();
            CreateMap<FluxMetadataCriterion, FluxMetadataCriterionDto>().ReverseMap();
            CreateMap<FluxAttachmentRule, FluxAttachmentRuleDto>().ReverseMap();

            CreateMap<FluxSearchRequest, SearchFluxQuery>();
            CreateMap<SearchFluxQueryItemResult, FluxSearchResponse>();

            CreateMap<FluxFetchingSearchRequest, SearchFluxFetchingQuery>();
            CreateMap<SearchFluxFetchingQueryItemResult, FluxFetchingSearchResponse>()
                .ForMember(dest => dest.WorkflowId, opt => opt.MapFrom(src => src.WorkflowId));

            CreateMap<FluxProcessingSearchRequest, SearchFluxProcessingFluxQuery>();
            CreateMap<SearchFluxProcessingQueryItemResult, FluxProcessingSearchReponse>();

            CreateMap<FluxErrorSearchRequest, SearchFluxErrorQuery>();
            CreateMap<SearchFluxErrrorQueryItemResult, FluxErrorSearchResponse>();

            CreateMap<RuleErrorSearchRequest, SearchRuleErrorQuery>();
            CreateMap<SearchRuleErrorQueryItemResult, RuleErrorSearchResponse>();

            CreateMap<FluxFetchHistoryQueryResult, FluxFetchingResponse>();
            CreateMap<FluxFetchingHistory, FluxFetchingHistoryResponse>();
            CreateMap<FluxFetchingContentHistory, FluxFetchingContentHistoryResponse>();

            CreateMap<FluxProcessingQueryResult, FluxProcessingResponse>();
            CreateMap<FluxProcessingHistory, FluxProcessingHistoryResponse>();
            CreateMap<FluxProcessingContentHistory, FluxProcessingContentHistoryResponse>();

            CreateMap<FluxErrorQueryResult, FluxErrorResponse>();
            CreateMap<FluxErrors, FluxErrorHistoryResponse>();
        }
    }

    public class SourceMappingProfile : Profile
    {
        public SourceMappingProfile()
        {
            CreateMap<SourceSearchRequest, SearchSourceQuery>();
            CreateMap<SearchSourceQueryItemResult, SourceSearchDto>();
        }
    }
}
