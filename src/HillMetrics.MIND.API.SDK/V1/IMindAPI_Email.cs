using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Requests.Email;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Email;
using Refit;

namespace HillMetrics.MIND.API.SDK.V1
{
    /// <summary>
    /// Interface Refit pour l'API Email - expose les endpoints du EmailController
    /// </summary>
    public partial interface IMindAPI
    {
        #region Email Metadata

        /// <summary>
        /// Search for email metadata following the given criteria
        /// </summary>
        [Get("/api/v1/email/search")]
        Task<CustomMindPagedApiResponseBase<EmailMetadataResponse>> SearchEmailMetadataAsync([Query] EmailMetadataSearchRequest request);

        /// <summary>
        /// Get email metadata by ID
        /// </summary>
        [Get("/api/v1/email/{id}")]
        Task<ApiResponseBase<EmailMetadataResponse>> GetEmailMetadataAsync(int id);

        #endregion
    }
} 