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

namespace HillMetrics.MIND.API.Mappers
{
    public class FluxMappingProfile : Profile
    {
        public FluxMappingProfile()
        {
            CreateMap<FluxProvider, FluxResponse>().ReverseMap();
            
            CreateMap<TriggerPeriod, TriggerPeriodDto>().ReverseMap();

            CreateMap<SourceProvider, SourceProviderDto>().ReverseMap();
            CreateMap<FluxProcessingHistory, FluxProcessingHistoryDto>().ReverseMap();
            CreateMap<FluxProcessingContentHistory, FluxProcessingContentHistoryDto>().ReverseMap();
            CreateMap<FluxIdentificationHistory, FluxIdentificationHistoryDto>().ReverseMap();
            CreateMap<FluxIdentificationContentHistory, FluxIdentificationContentHistoryDto>().ReverseMap();
            CreateMap<FluxErrors, FluxErrorsDto>().ReverseMap();
            CreateMap<FluxFinancialDataPoint, FluxFinancialDataPointDto>().ReverseMap();

            CreateMap<FluxMetadataDto, FluxMetadata>().IncludeAllDerived();
            //CreateMap<FluxMetadataDto, FluxMetadata>()
            //    .Include<FluxMetadataMailDto, FluxMetadataMail>()
            //    .Include<FluxMetadataDownloadDto, FluxMetadataDownload>()
            //    .Include<FluxMetadataApiDto, FluxMetadataApi>()
            //    .Include<FluxMetadataFileLocationDto, FluxMetadataFileLocation>();

            //CreateMap<FluxMetadata, FluxMetadataDto>()
            //    .Include<FluxMetadataMail, FluxMetadataMailDto>()
            //    .Include<FluxMetadataDownload, FluxMetadataDownloadDto>()
            //    .Include<FluxMetadataApi, FluxMetadataApiDto>()
            //    .Include<FluxMetadataFileLocation, FluxMetadataFileLocationDto>();

            CreateMap<FluxMetadata, FluxMetadataDto>().IncludeAllDerived();
            CreateMap<FluxMetadataMail, FluxMetadataMailDto>().ReverseMap();
            CreateMap<FluxMetadataDownload, FluxMetadataDownloadDto>().ReverseMap();
            CreateMap<FluxMetadataApi, FluxMetadataApiDto>().ReverseMap();
            CreateMap<FluxMetadataFileLocation, FluxMetadataFileLocationDto>().ReverseMap();

            CreateMap<FluxRuleSettings, FluxRuleSettingsDto>().ReverseMap();
            CreateMap<FluxMetadataCriterion, FluxMetadataCriterionDto>().ReverseMap();
            CreateMap<FluxAttachmentRule, FluxAttachmentRuleDto>().ReverseMap();

            CreateMap<FluxSearchRequest, SearchFluxQuery>();
            CreateMap<SearchFluxQueryItemResult, FluxSearchResponse>();
            
            CreateMap<FluxFetchingSearchRequest, SearchFluxFetchingQuery>();
            CreateMap<SearchFluxFetchingQueryItemResult, FluxFetchingSearchDto>();
            
            CreateMap<FluxProcessingSearchRequest, SearchFluxProcessingFluxQuery>();
            CreateMap<SearchFluxProcessingQueryItemResult, FluxProcessingSearchDto>();

            CreateMap<FluxErrorSearchRequest, SearchFluxErrorQuery>();
            CreateMap<SearchFluxErrrorQueryItemResult, FluxErrorSearchDto>();
            
            CreateMap<FluxFetchHistoryQueryResult, FluxFetchingResponse>();
            CreateMap<FluxIdentificationHistory, FluxFetchingHistoryResponse>();
            CreateMap<FluxIdentificationContentHistory, FluxFetchingContentHistoryResponse>();

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
