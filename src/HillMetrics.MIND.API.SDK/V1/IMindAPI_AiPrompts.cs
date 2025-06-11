using HillMetrics.Core.AI;
using HillMetrics.MIND.API.Contracts.Requests.AiPrompts;
using HillMetrics.MIND.API.Contracts.Responses.AiPrompts;
using HillMetrics.MIND.API.Contracts.Responses.Common;
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
        [Get("/api/v1/prompt/{id}")]
        Task<GetAiPromptResponse> GetAiPromptAsync(int id);

        /// <summary>
        /// Search AI Prompts
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="taskType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Get("/api/v1/prompt/search")]
        Task<ListAiPromptsResponse> SearchAiPromptsAsync([Query] int? languageId, [Query] PromptTaskType? taskType, [Query] PromptType? promptType, [Query] int pageNumber = 1, [Query] int pageSize = 25);


        /// <summary>
        /// Create an AI Prompt
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/v1/prompt")]
        Task<GetAiPromptResponse> CreateAiPromptAsync([Body] SaveAiPromptRequest request);

        /// <summary>
        /// Update an AI Prompt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Put("/api/v1/prompt/{id}")]
        Task<GetAiPromptResponse> UpdateAiPromptAsync(int id, [Body] SaveAiPromptRequest request);

        /// <summary>
        /// Delete an AI Prompt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Delete("/api/v1/prompt/{id}")]
        Task<DeletedResponse> DeleteAiPromptAsync(int id);
    }                                                
}                                                    
                                                     