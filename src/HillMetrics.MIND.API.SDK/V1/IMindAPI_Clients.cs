using HillMetrics.MIND.API.Contracts.Requests.Clients;
using HillMetrics.MIND.API.Contracts.Responses.Clients;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Endpoints;
using Refit;

namespace HillMetrics.MIND.API.SDK.V1
{
    /// <summary>
    /// Interface Refit pour l'API MIND - expose les endpoints du FluxController
    /// </summary>
    public partial interface IMindAPI
    {
        /// <summary>
        /// Get Client info by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/api/v1/clients/{id}")]
        Task<GetClientResponse> GetClientAsync(int id);

        /// <summary>
        /// Search clients
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Get("/api/v1/clients/search")]
        Task<ListClientsResponse> SearchClientsAsync([Query] string? name, [Query] string? email, [Query] int pageNumber = 1, [Query] int pageSize = 25);


        /// <summary>
        /// Create a client
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/v1/clients")]
        Task<GetClientResponse> CreateClientAsync([Body] SaveClientRequest request);

        /// <summary>
        /// Update a client
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Put("/api/v1/clients/{id}")]
        Task<GetClientResponse> UpdateClientAsync(int id, [Body] SaveClientRequest request);

        /// <summary>
        /// Delete a client by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Delete("/api/v1/clients/{id}")]
        Task<DeletedResponse> DeleteClientAsync(int id);
    }                                                
}                                                    
                                                     