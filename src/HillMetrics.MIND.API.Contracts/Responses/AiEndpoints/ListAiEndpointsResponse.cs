using HillMetrics.Normalized.Domain.Contracts.AI.Endpoints;

namespace HillMetrics.MIND.API.Contracts.Responses.AiEndpoints
{
    public class ListAiEndpointsResponse : ApiPagedResponseBase<AiEndpoint>
    {
        public ListAiEndpointsResponse(IEnumerable<AiEndpoint> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
