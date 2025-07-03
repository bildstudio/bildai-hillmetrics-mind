using HillMetrics.Normalized.Domain.Contracts.AI.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiEndpoints
{
    public class SaveAiEndpointRequest
    {
        public required AiEndpoint Endpoint { get; set; }
    }
}
