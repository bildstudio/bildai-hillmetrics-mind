using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Responses.Llm;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace HillMetrics.MIND.API.Contracts.Requests.Llm
{
    public class UpdatePromptRequest
    {
        public string? Name { get; set; }
        public IFormFile? File { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FinancialDataPoint DataType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FinancialType ProductType { get; set; }
    }
}
