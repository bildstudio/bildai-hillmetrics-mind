using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.Core.Authentication.Objects;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace HillMetrics.MIND.Infrastructure.Authentication
{
    public class TokenExchangeService : ITokenExchangeService
    {
        private readonly IDatabase _database;
        private readonly ILogger<TokenExchangeService> _logger;

        public TokenExchangeService(IConnectionMultiplexer connectionMultiplexer, ILogger<TokenExchangeService> logger)
        {
            _database = connectionMultiplexer.GetDatabase();
            _logger = logger;
        }

        public async Task<TokenResponse?> GetTokenByCodeAsync(string code)
        {
            try
            {
                var json = await _database.StringGetDeleteAsync(code);
                if (json.IsNull)
                    return default;

                return json.HasValue ? JsonSerializer.Deserialize<TokenResponse>(json.ToString()) : default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTokenByCodeAsync error: {ExceptionMessage}", ex.Message);
                throw;
            }
        }

        public async Task<string> SaveTokenForCodeAsync(TokenResponse token, TimeSpan expiration)
        {
            try
            {
                string json = JsonSerializer.Serialize(token);

                string code = Guid.NewGuid().ToString();

                await _database.StringSetAsync(code, json, expiration);

                return code;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveTokenForCodeAsync error: {ExceptionMessage}", ex);
                throw;
            }
        }
    }
}
