
namespace HillMetrics.MIND.API.Contracts.Responses.Clients
{
    public class ListClientFluxRulesResponse : ApiPagedResponseBase<ClientFluxRuleDto>
    {
        public ListClientFluxRulesResponse(IEnumerable<ClientFluxRuleDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
