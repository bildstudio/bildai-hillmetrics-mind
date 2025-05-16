namespace HillMetrics.MIND.Domain.Contracts.Clients.Models
{
    public class SaveClientModel
    {
        public SaveClientModel(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; }
        public string Email { get; }
    }
}
