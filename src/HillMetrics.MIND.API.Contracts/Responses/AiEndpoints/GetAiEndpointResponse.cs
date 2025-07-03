using HillMetrics.Normalized.Domain.Contracts.AI.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.AiEndpoints
{
    public class GetAiEndpointResponse : ApiResponseBase<AiEndpoint>
    {
        public GetAiEndpointResponse(AiEndpoint data) : base(data)
        {
        }
    }
}
