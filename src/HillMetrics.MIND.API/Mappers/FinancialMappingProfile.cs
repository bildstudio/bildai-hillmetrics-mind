using AutoMapper;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Prices;
using HillMetrics.Normalized.Domain.Contracts.Market;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using static HillMetrics.MIND.API.Contracts.Responses.Prices.SearchPricesResponse;

namespace HillMetrics.MIND.API.Mappers
{
    public class FinancialMappingProfile : Profile
    {
        public FinancialMappingProfile()
        {
            CreateMap<PriceEntityData, SearchPricesResponse>().ReverseMap();
            CreateMap<Normalized.Domain.Contracts.Market.PriceEntityData.PropertyValue, PropertyValueResponse>().ReverseMap();
        }
    }
}
