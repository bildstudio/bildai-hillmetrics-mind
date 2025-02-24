using HillMetrics.Core.Financial;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace HillMetrics.MIND.API.Contracts.Requests.Llm
{
    public class CreatePromptRequest
    {
        public string? Name { get; set; }

        public required IFormFile File { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FinancialDataPoint DataType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FinancialType ProductType { get; set; }
    }
}
