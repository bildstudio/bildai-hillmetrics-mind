using FluentResults;
using HillMetrics.Core.Mediator;
using HillMetrics.Core.Messaging.Bus;
using HillMetrics.Core.Messaging.Notification.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class PopulateClientRefinedDbCommandHandler : Handler<PopulateClientRefinedDbCommandHandler, bool, PopulateClientRefinedDbCommand>
    {
        public PopulateClientRefinedDbCommandHandler(
            ILogger<PopulateClientRefinedDbCommandHandler> logger) 
            : base(logger)
        {
        }

        public override async Task<Result<bool>> HandleInnerAsync(PopulateClientRefinedDbCommand request, CancellationToken cancellationToken)
        {
            //TODO:Selvir
            return true;
        }
    }
}
