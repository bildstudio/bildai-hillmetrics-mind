using HillMetrics.Core.Storage.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Domain.Contracts.Clients
{
    public class ClientEntity
    {
        public ClientEntity(int id, string name, string email, bool isActive)
        {
            Id = id;
            Name = name;
            Email = email;
            IsActive = isActive;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; }

        public string RefinedDbName => $"Refined_{Name}_{Id}";
    }
}
