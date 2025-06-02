using HillMetrics.Core.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.SDK
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Convert Refit ApiException to HillMetrics ApiException object
        /// </summary>
        /// <param name="apiException"></param>
        /// <returns></returns>
        public static ApiException ToHillMetricsApiException(this Refit.ApiException apiException)
        {
            try
            {
                if (apiException.HasContent)
                {
                    ApiException? temp = JsonSerializer.Deserialize<ApiException>(apiException.Content, _options);

                    if (temp != null)
                        return temp;


                }

                return new ApiException(apiException.Message);

            }
            catch (Exception ex)
            {
                return new ApiException(apiException.Message);
            }
        }
    }
}
