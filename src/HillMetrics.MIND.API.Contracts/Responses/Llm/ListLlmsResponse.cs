using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class ListLlmsResponse : CustomMindPagedApiResponseBase<AiLlmEntityDto>
    {
        public ListLlmsResponse(List<AiLlmEntityDto> data, int totalRecords) : base(data, totalRecords)
        {
            
        }
    }
}
