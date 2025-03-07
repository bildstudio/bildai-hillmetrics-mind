//using HillMetrics.MIND.API.Contracts.Requests.Flux;
//using HillMetrics.MIND.API.Contracts.Responses;
//using HillMetrics.MIND.API.Contracts.Responses.Flux;
//using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Collect;
//using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Process;

//namespace HillMetrics.MIND.Web.Services
//{
//    public interface IFluxService
//    {
//        Task<PagedApiResponseBase<FluxSearchResponse>> SearchAsync(FluxSearchRequest request);
//        Task<FluxResponse> GetAsync(int id);
//        Task<bool> CreateFluxAsync(FluxRequest request);
//        Task<bool> UpdateFluxAsync(int fluxId, FluxRequest request);
//        Task<bool> DeleteFluxAsync(int fluxId);
//        Task<FetchFluxCommandResult> ForceFetchAsync(int id);
//        Task<ProcessFluxCommandResult> ForceProcessAsync(int id);
//    }
//} 