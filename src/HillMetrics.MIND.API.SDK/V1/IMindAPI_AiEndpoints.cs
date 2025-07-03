using HillMetrics.MIND.API.Contracts.Requests.AiEndpoints;
using HillMetrics.MIND.API.Contracts.Responses.AiEndpoints;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using Refit;

namespace HillMetrics.MIND.API.SDK.V1
{
    
    public partial interface IMindAPI
    {
        /// <summary>
        /// Get AI endpoint by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Get("/api/v1/ai/internal/endpoints/{id}")]
        Task<GetAiEndpointResponse> GetAiEndpointAsync(
            int id, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search AI endpoints
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Get("/api/v1/ai/internal/endpoints/search")]
        Task<ListAiEndpointsResponse> SearchAiEndpointsAsync(
            [Query] string? searchTerm,
            [Query] int pageNumber = 1,
            [Query] int pageSize = 25, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create AI endpoint
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Post("/api/v1/ai/internal/endpoints")]
        Task<GetAiEndpointResponse> CreateAiEndpointAsync(
            [Body] SaveAiEndpointRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update AI endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Put("/api/v1/ai/internal/endpoints/{id}")]
        Task<GetAiEndpointResponse> UpdateAiEndpointAsync(
            int id,
            [Body] SaveAiEndpointRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete AI endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Delete("/api/v1/ai/internal/endpoints/{id}")]
        Task<DeletedResponse> DeleteAiEndpointAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}                                                    
                                                     