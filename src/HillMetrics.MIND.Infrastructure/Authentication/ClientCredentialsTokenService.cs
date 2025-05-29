using FluentResults;
using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.Core.Authentication.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HillMetrics.Core.Authentication.Keycloak;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;

namespace HillMetrics.MIND.Infrastructure.Authentication
{
    public class ClientCredentialsTokenService : IClientCredentialsTokenService
    {
        private readonly ILogger<ClientCredentialsTokenService> _logger;
        private readonly KeycloakConfigMind _keycloakConfig;
        private TokenResponse? _tokenResponse;

        public ClientCredentialsTokenService(ILogger<ClientCredentialsTokenService> logger, IOptions<KeycloakConfigMind> options)
        {
            _logger = logger;
            _keycloakConfig = options.Value;
        }

        public async Task<Result<TokenResponse>> GetAccessTokenAsync()
        {
            try
            {
                if (!IsTokenExpired())
                    return Result.Ok<TokenResponse>(_tokenResponse!);

                using HttpClient client = new HttpClient();

                var clientCredentials = new ClientCredentialsSettings(_keycloakConfig.Private.ClientId, _keycloakConfig.Private.ClientSecret);

                var response = client.PostAsync(_keycloakConfig.TokenUrl(), new FormUrlEncodedContent(clientCredentials.ToFormData())).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Result.Fail(error);
                }

                _tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                return Result.Ok<TokenResponse>(_tokenResponse!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAccessTokenAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail(ex.Message);
            }
        }

        //check if token has been expired or going to expire in less than 30 seconds
        private bool IsTokenExpired()
        {
            if (_tokenResponse == null)
                return true;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(_tokenResponse.AccessToken);

            return jwtToken.ValidTo < DateTime.UtcNow.AddSeconds(-30);
        }
    }
}
