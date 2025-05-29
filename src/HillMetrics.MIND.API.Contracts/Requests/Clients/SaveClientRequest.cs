using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Clients
{
    public class SaveClientRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
    }
}
