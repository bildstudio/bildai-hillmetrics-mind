using HillMetrics.Core.Search;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Models
{
    public class SearchClientsModel
    {
        public SearchClientsModel(string? name, string? email, Pagination pagination)
        {
            Name = name;
            Email = email;
            Pagination = pagination;
        }

        public string? Name { get; }
        public string? Email { get; }

        public Pagination Pagination { get; }
    }
}
