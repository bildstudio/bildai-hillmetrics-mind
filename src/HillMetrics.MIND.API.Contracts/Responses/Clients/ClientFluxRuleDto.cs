namespace HillMetrics.MIND.API.Contracts.Responses.Clients
{
    public class ClientFluxRuleDto
    {
        public ClientFluxRuleDto(int id, int dataPointId, int peerGroupId, int ranking, List<FluxPriorityDto> fluxPriorityList)
        {
            Id = id;
            DataPointId = dataPointId;
            PeerGroupId = peerGroupId;
            Ranking = ranking;
            FluxPriorityList = fluxPriorityList;
        }

        public int Id { get; set; }
        public int DataPointId { get; set; }
        public int PeerGroupId { get; set; }
        public int Ranking { get; set; }
        public List<FluxPriorityDto> FluxPriorityList { get; set; } = [];
    }

    public class FluxPriorityDto
    {
        public FluxPriorityDto(int fluxId, int priority)
        {
            FluxId = fluxId;
            Priority = priority;
        }

        public int FluxId { get; set; }
        public int Priority { get; set; }
    }
}
