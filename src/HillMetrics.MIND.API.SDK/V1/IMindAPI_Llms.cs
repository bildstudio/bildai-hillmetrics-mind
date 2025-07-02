using HillMetrics.MIND.API.Contracts.Requests.Llm;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Contracts.Responses.Llm;
using Refit;

namespace HillMetrics.MIND.API.SDK.V1
{
    
    public partial interface IMindAPI
    {
        /// <summary>
        /// Get Llm model by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Get("/api/v1/llm/models/{id}")]
        Task<GetLlmResponse> GetLlmModelAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// List llm models 
        /// </summary>
        /// <param name="active">if null, all models are retrieved, if true, only active, if false only inactive are returned</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Get("/api/v1/llm/models/search")]
        Task<ListLlmsResponse> SearchLlmModelsAsync([Query] bool? active, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create Llm model
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Post("/api/v1/llm/models")]
        Task<GetLlmResponse> CreateLlmModelAsync([Body] CreateLlmRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update Llm model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Put("/api/v1/llm/models/{id}")]
        Task<GetLlmResponse> UpdateLlmModelAsync(int id, [Body] UpdateLlmRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete Llm model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Delete("/api/v1/llm/models/{id}")]
        Task<DeletedResponse> DeleteLlmModelAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update Llm task types settings
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Put("/api/v1/llm/models/{id}/task-type-settings")]
        Task<GetLlmTaskTypeSettingsResponse> SaveTaskTypeSettingsAsync(int id, [Body] SaveTaskTypeSettingsRequest request, CancellationToken cancellationToken = default);
    }
}                                                    
                                                     