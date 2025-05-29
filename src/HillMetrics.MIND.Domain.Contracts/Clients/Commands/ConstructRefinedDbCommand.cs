using FluentResults;
using HillMetrics.Core.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Commands
{
    public class ConstructRefinedDbCommand : AuditedCommand<Result<bool>>
    {
        public int ClientId { get; set; }
        public ConstructRefinedDbCommand(int clientId)
        {
            ClientId = clientId;
        }
    }
}
