using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class ListLlmsResponse : ApiResponseBase<List<AiLlmEntityDto>>
    {
        public ListLlmsResponse(List<AiLlmEntityDto> data) : base(data)
        {
        }
    }
}
