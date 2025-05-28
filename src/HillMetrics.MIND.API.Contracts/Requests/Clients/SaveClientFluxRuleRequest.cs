namespace HillMetrics.MIND.API.Contracts.Requests.Clients
{
    public class SaveClientFluxRuleRequest
    {
        public int DataPointId { get; set; }
        public int PeerGroupId { get; set; }
        public int Ranking { get; set; }
        public List<int> FluxPriorityList { get; set; } = [];

        public bool UseHmDefaultRules { get; set; } = false;
    }
}
