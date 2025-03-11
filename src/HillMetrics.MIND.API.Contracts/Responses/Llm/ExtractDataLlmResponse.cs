using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class ExtractDataLlmResponse : ApiResponseBase<List<AiModelPromptAnalyzedResultDto>>
    {
        //list of result dtos.
        public ExtractDataLlmResponse(List<AiModelPromptAnalyzedResultDto> data) : base(data)
        {
            
        }
    }
}