using AutoMapper;
using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.ElementValue;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FileDataMapping;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FileUpload;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FinancialDataPoint;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.PropertyDataType;

namespace HillMetrics.MIND.API.Mappers
{
    public class AiDatasetMappingProfile : Profile
    {
        public AiDatasetMappingProfile()
        {
            // FileUpload mappings
            CreateMap<FileUpload, FileUploadResponse>();
            CreateMap<FileUploadRequest, CreateFileUploadCommand>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.File.ContentType))
                .ForMember(dest => dest.FileStream, opt => opt.Ignore())
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty));

            // FileDataMapping mappings
            CreateMap<FileDataMapping, FileDataMappingResponse>();
            CreateMap<CreateFileMappingRequest, CreateFileMappingCommand>();
            CreateMap<ElementValueRequest, FileDataElementValue>();
            CreateMap<List<FileDataMapping>, FileDataMappingListResponse>()
                .ForMember(dest => dest.Mappings, opt => opt.MapFrom(src => src));

            // ElementValue mappings
            CreateMap<FileDataElementValue, ElementValueResponse>();
            CreateMap<List<FileDataElementValue>, ElementValueListResponse>()
                .ForMember(dest => dest.Elements, opt => opt.MapFrom(src => src));

            // FinancialDataPoint mappings
            CreateMap<FinancialDataPoint, FinancialDataPointResponse>();
            CreateMap<CreateFinancialDataPointRequest, CreateFinancialDataPointCommand>();
            CreateMap<FinancialDataPointElementRequest, FinancialDataPointElement>();
            CreateMap<List<FinancialDataPoint>, FinancialDataPointListResponse>()
                .ForMember(dest => dest.DataPoints, opt => opt.MapFrom(src => src));

            // DataPointElement mappings
            CreateMap<FinancialDataPointElement, DataPointElementResponse>();
            CreateMap<List<FinancialDataPointElement>, DataPointElementListResponse>()
                .ForMember(dest => dest.Elements, opt => opt.MapFrom(src => src));

            CreateMap<SearchPropertyDataTypeRequest, SearchPropertyDataTypeQuery>();
            CreateMap<CreatePropertyDataTypeRequest, CreatePropertyDataTypeCommand>();
            CreateMap<CreatePropertyDataTypeRequest, CreatePropertyDataTypeCommand>();
            CreateMap<PropertyDataType, PropertyDataTypeResponse>();
            CreateMap<SearchPropertyDataTypeQueryItem, PropertyDataTypeResponse>();
        }
    }
}
