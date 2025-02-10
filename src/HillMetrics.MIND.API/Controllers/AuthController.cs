using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.API.Responses;
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
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet(InternalRoutes.Authentication.Login)]
        public IActionResult Login([FromQuery] string redirectUrl)//
        {
            string authorizationUrl = _authenticationService.GetAuthenticationUrl(redirectUrl);

            return Redirect(authorizationUrl);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet(InternalRoutes.Authentication.Callback)]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var tokenResult = await _authenticationService.ExhangeCodeForTokenAsync(code);
            if (tokenResult.IsFailed)
                return new ErrorApiActionResult(tokenResult.Errors.ToApiResult());

            tokenResult.Value.SaveTokensInCookies(HttpContext.Response.Cookies);
            //StoreTokenResponseInCookie(tokenResult.Value);

            return RedirectPermanent(state);
        }

        
        [HttpPost(InternalRoutes.Authentication.Refresh)]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var tokenResult = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
            if (tokenResult.IsFailed)
                return new ErrorApiActionResult(tokenResult.Errors.ToApiResult());

            tokenResult.Value.SaveTokensInCookies(HttpContext.Response.Cookies);

            return tokenResult.Value;
        }

        //redirect to 
        [HttpGet(InternalRoutes.Authentication.Logout)]
        public IActionResult Logout([FromQuery] string redirectUrl)
        {
            string idToken = Request.Cookies.ContainsKey("id_token") ? Request.Cookies["id_token"]!.ToString() : string.Empty;

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

        private void StoreTokenResponseInCookie(TokenResponse tokenResponse)
        {
            // Store the access token and refresh token in cookies or a session
            HttpContext.Response.Cookies.Append("access_token", tokenResponse.AccessToken);
            HttpContext.Response.Cookies.Append("expires_in", tokenResponse.ExpiresIn.ToString());
            HttpContext.Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken);
            HttpContext.Response.Cookies.Append("refresh_expires_in", tokenResponse.RefreshExpiresIn.ToString());
            HttpContext.Response.Cookies.Append("id_token", tokenResponse.IdToken);
        }

        private void ClearTokens()
        {
            // Store the access token and refresh token in cookies or a session
            HttpContext.Response.Cookies.Delete("access_token");
            HttpContext.Response.Cookies.Delete("expires_in");
            HttpContext.Response.Cookies.Delete("refresh_token");
            HttpContext.Response.Cookies.Delete("refresh_expires_in");
            HttpContext.Response.Cookies.Delete("id_token");
        }
    }
}
