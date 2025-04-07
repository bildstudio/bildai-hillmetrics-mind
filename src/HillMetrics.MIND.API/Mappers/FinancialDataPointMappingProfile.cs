using AutoMapper;
using HillMetrics.Core.Financial;
using HillMetrics.Core.Storage.Database.Search;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FinancialDataPoint;

namespace HillMetrics.MIND.API.Mappers
{
    public class FinancialDataPointMappingProfile : Profile
    {
        public FinancialDataPointMappingProfile()
        {
            CreateMap<SearchFinancialDataPointRequest, SearchFinancialDataPointQuery>();
            CreateMap<SearchFinancialDataPointQueryItem, FinancialDataPointSearchResponse>();
        }
    }
}