using HillMetrics.Core.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class AiModelPromptDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FinancialDataPoint DataType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required FinancialType ProductType { get; set; }
    }
}
