using HillMetrics.Core.API.Contracts;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.Core.Authentication.Objects;
using HillMetrics.MIND.API.Contracts.Requests;
using HillMetrics.MIND.API.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/v{v:apiVersion}")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IRedirectUrlValidator _redirectUrlValidator;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IAuthenticationService authenticationService,
            IRedirectUrlValidator redirectUrlValidator,
            ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _redirectUrlValidator = redirectUrlValidator;
            _logger = logger;
        }

        [HttpGet(InternalRoutes.Authentication.Login)]
        public IActionResult Login([FromQuery] string redirectUrl)
        {
            if (!_redirectUrlValidator.IsValidRedirectUrl(redirectUrl))
                return Forbid();

            string authorizationUrl = _authenticationService.GetAuthenticationUrl(redirectUrl);

            return Redirect(authorizationUrl);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet(InternalRoutes.Authentication.Callback)]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                //_logger.LogInformation("Callback received: code: {code}, state: {state}", code, state);
                var tokenResult = await _authenticationService.ExhangeCodeForTokenAsync(code);
                if (tokenResult.IsFailed)
                {
                    _logger.LogError("Failed to exchange code for token");
                    return new ErrorApiActionResult(tokenResult.Errors.ToApiResult());
                }

                _logger.LogInformation("Code exchanged for token");
                tokenResult.Value.SaveTokensInCookies(HttpContext.Response.Cookies);


                return Redirect(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Callback error: {ExceptionMessage}", ex.Message);
                throw;
            }
        }
        
        [HttpPost(InternalRoutes.Authentication.Refresh)]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] Contracts.Requests.RefreshTokenRequest request)
        {
            var tokenResult = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
            if (tokenResult.IsFailed)
                return new ErrorApiActionResult(tokenResult.Errors.ToApiResult());

            tokenResult.Value.SaveTokensInCookies(HttpContext.Response.Cookies);

            return tokenResult.Value;
        }

        [HttpGet(InternalRoutes.Authentication.Logout)]
        public IActionResult Logout([FromQuery] string redirectUrl)
        {
            if (!_redirectUrlValidator.IsValidRedirectUrl(redirectUrl))
                return Forbid();

            Request.Cookies.TryGetValue(AuthConstants.Cookie.IdToken, out string? idToken);

            //string idToken = Request.Cookies.ContainsKey(Constants.Cookie.IdToken) ? Request.Cookies[Constants.Cookie.IdToken]!.ToString() : string.Empty;

            string authorizationUrl = _authenticationService.GetLogoutUrl(idToken, redirectUrl);
            return Redirect(authorizationUrl);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet(InternalRoutes.Authentication.LogoutCallback)]
        public IActionResult LogoutCallback([FromQuery] string state)
        {
            HttpContext.Response.Cookies.ClearAuthenticationCookies();
            return Redirect(state);
        }
    }
}
