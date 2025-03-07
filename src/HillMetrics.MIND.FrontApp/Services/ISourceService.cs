using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Source;

namespace HillMetrics.MIND.FrontApp.Services
{
    public interface ISourceService
    {
        Task<SourceSearchResponse> SearchAsync(SourceSearchRequest request);
        Task<SourceResponse> CreateSourceAsync(SourceCreateRequest request);
        Task<SourceResponse> EditSourceAsync(int sourceId, SourceCreateRequest request);
    }
} 