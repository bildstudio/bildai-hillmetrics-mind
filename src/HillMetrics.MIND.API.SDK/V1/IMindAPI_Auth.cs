using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Source;
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
        /// Get login page url
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        [Get($"/api/v1/{InternalRoutes.Authentication.Login}")]
        Task<HttpResponseMessage> LoginAsync([Query] string redirectUrl);
    }
}
