namespace HillMetrics.MIND.Domain.Contracts.Clients.Models
{
    public class SaveClientModel
    {
        public SaveClientModel(string name, string email, bool isActive)
        {
            Name = name;
            Email = email;
            IsActive = isActive;
        }

        public string Name { get; }
        public string Email { get; }
        public bool IsActive { get; }
    }
}
