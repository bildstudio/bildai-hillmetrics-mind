namespace HillMetrics.MIND.API.Contracts.Responses.Clients
{
    public class ListClientsResponse : ApiPagedResponseBase<ClientDto>
    {
        public ListClientsResponse(IEnumerable<ClientDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
