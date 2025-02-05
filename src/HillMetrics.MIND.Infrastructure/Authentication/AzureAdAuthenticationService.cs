using FluentResults;
using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.Core.Authentication.Keycloak;
using HillMetrics.Core.Authentication.Objects;
using HillMetrics.Core.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using static HillMetrics.Core.Authentication.Keycloak.KeycloakConfig;

namespace HillMetrics.MIND.Infrastructure.Authentication
{
    public class AzureAdAuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AzureAdAuthenticationService> _logger;
        private readonly IdentityProviderSettings _settings;
        private readonly EndpointsMetadata _endpointsMetadata;

        public AzureAdAuthenticationService(
            ILogger<AzureAdAuthenticationService> logger,
            IOptions<KeycloakConfigMind> options 
            )
        {
            _logger = logger;
            _settings = options.Value.Azure;
            _endpointsMetadata = options.Value.Endpoints();
        }

        public string GetAuthenticationUrl(string state, string scopes = "openid profile")
        {
            return $"{_endpointsMetadata.AuthenticationEndpoint}" +
                     $"?response_type=code" +
                     $"&client_id={_settings.ClientId}" +
                     $"&redirect_uri={_settings.PostLoginRedirectUrl}" +
                     $"&scope={scopes}" +
                     $"&state={state}" +
                     $"&kc_idp_hint={_settings.ProviderAlias}";
        }

        public string GetLogoutUrl(string? tokenId, string state)
        {
            if (!string.IsNullOrEmpty(tokenId))
                return $"{_endpointsMetadata.LogoutEndpoint}?id_token_hint={tokenId}&post_logout_redirect_uri={_settings.PostLogoutRedirectUrl}&state={state}";

            return $"{_endpointsMetadata.LogoutEndpoint}?client_id={_settings.ClientId}&post_logout_redirect_uri={_settings.PostLogoutRedirectUrl}&state={state}";
        }

        public async Task<Result<TokenResponse>> ExhangeCodeForTokenAsync(string code)
        {
            try
            {
                var settings = new AuthorizationCodeSettings(_settings.ClientId, _settings.ClientSecret, code, _settings.PostLoginRedirectUrl);

                using HttpClient client = new HttpClient();
                var response = await client.PostAsync(_endpointsMetadata.TokenEndpoint, new FormUrlEncodedContent(settings.ToFormData()));
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return Result.Fail(new InternalServerError(errorResponse));
                }

                TokenResponse tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return Result.Ok<TokenResponse>(tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExhangeCodeForTokenAsync error : {ExceptionMessage}", ex.Message);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }

        public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var settings = new AuthorizationCodeRefreshTokenSettings(_settings.ClientId, _settings.ClientSecret, refreshToken);

                using HttpClient client = new HttpClient();
                var response = await client.PostAsync(_endpointsMetadata.TokenEndpoint, new FormUrlEncodedContent(settings.ToFormData()));
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return Result.Fail(new InternalServerError(errorResponse));
                }

                TokenResponse tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return Result.Ok<TokenResponse>(tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshTokenAsync error : {ExceptionMessage}", ex.Message);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }
    }
}
