using HillMetrics.Core.AI;
using HillMetrics.Core.Financial;
using HillMetrics.Core.Financial.DataPoint;
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

    public class AiModelPromptLlmResultDto
    {
        public int Id { get; set; }
        public int PromptId { get; set; }
        public int AiLlmModelId { get; set; }
        public string? Result { get; set; }
        public int? ValidityIndex { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LlmProcessingStatus Status { get; set; }
    }
}
