namespace HillMetrics.MIND.Domain.Contracts.Clients
{
    public class ClientFluxRule
    {
        public int Id { get; set; }
        public int DataPointId { get; set; }
        public int PeerGroupId { get; set; }
        public int Ranking { get; set; }
        public List<FluxPriority> FluxPriorityList { get; set; } = [];
    }

    public class FluxPriority
    {
        public int FluxId { get; set; }
        public int Priority { get; set; }
    }
}
