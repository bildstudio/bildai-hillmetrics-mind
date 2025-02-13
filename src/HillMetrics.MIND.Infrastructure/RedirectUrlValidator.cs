using HillMetrics.Core.API.Configs;
using HillMetrics.Core.API.Contracts;
using Microsoft.Extensions.Options;

namespace HillMetrics.MIND.Infrastructure
{
    public class RedirectUrlValidator : IRedirectUrlValidator
    {
        private readonly CorsConfig _corsConfig;
        public RedirectUrlValidator(IOptions<CorsConfig> options)
        {
            _corsConfig = options.Value;
        }
        public bool IsValidRedirectUrl(string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
                return false;

            if (!Uri.TryCreate(redirectUrl, UriKind.Absolute, out Uri? uri))
                return false;

            if (_corsConfig.AllowAll)
                return true;

            return _corsConfig.AllowedOrigins.Any(allowedOrigin => redirectUrl.StartsWith(allowedOrigin, StringComparison.OrdinalIgnoreCase));
        }
    }
}
