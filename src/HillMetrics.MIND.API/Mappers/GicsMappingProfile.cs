using AutoMapper;
using HillMetrics.Core.Financial.Gics;
using HillMetrics.MIND.API.Contracts.Responses.Sector;

namespace HillMetrics.MIND.API.Mappers
{
    public class GicsMappingProfile : Profile
    {
        public GicsMappingProfile()
        {
            // Sector Mappings
            CreateMap<GicsSectorRequest, GicsSector>()
                .ConstructUsing(src => new GicsSector(0, src.Name));
            CreateMap<GicsSector, GicsSectorResponse>();

            // Industry Group Mappings
            CreateMap<GicsIndustryGroupRequest, GicsIndustryGroup>()
                .ConstructUsing(src => new GicsIndustryGroup(0, src.Name, new GicsSector(src.GicsSectorId, string.Empty)));
            CreateMap<GicsIndustryGroup, GicsIndustryGroupResponse>();

            // Industry Mappings
            CreateMap<GicsIndustryRequest, GicsIndustry>()
                .ConstructUsing(src => new GicsIndustry(0, src.Name, new GicsIndustryGroup(src.GicsIndustryGroupId, string.Empty, new GicsSector(0, string.Empty))));
            CreateMap<GicsIndustry, GicsIndustryResponse>();

            // Sub-Industry Mappings
            CreateMap<GicsSubIndustryRequest, GicsSubIndustry>()
                .ConstructUsing(src => new GicsSubIndustry(0, src.Name, new GicsIndustry(src.GicsIndustryId, string.Empty, new GicsIndustryGroup(0, string.Empty, new GicsSector(0, string.Empty)))));
            CreateMap<GicsSubIndustry, GicsSubIndustryResponse>();
        }
    }
}
