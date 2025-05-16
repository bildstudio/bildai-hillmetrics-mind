namespace HillMetrics.MIND.Domain.Contracts.Clients.Models
{
    public class SaveClientFluxRuleModel
    {
        public SaveClientFluxRuleModel(int dataPointId, int peerGroupId, int ranking, List<int> fluxPriorityList, int clientId)
        {
            DataPointId = dataPointId;
            PeerGroupId = peerGroupId;
            Ranking = ranking;
            FluxPriorityList = fluxPriorityList;
            ClientId = clientId;
        }

        public int DataPointId { get; set; }
        public int PeerGroupId { get; set; }
        public int Ranking { get; set; }
        public List<int> FluxPriorityList { get; set; } = [];
        public int ClientId { get; set; }
    }
}
