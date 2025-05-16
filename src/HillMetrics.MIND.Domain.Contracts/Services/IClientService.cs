using FluentResults;
using HillMetrics.Core.Common;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Domain.Contracts.Services
{
    public interface IClientService
    {
        Task<Result<ClientEntity>> GetAsync(int id, CancellationToken cancellationToken);
        Task<Result<ClientEntity>> CreateAsync(SaveClientModel model, CancellationToken cancellationToken = default);
        Task<Result<ClientEntity>> UpdateAsync(int id, SaveClientModel model, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<ClientEntity>>> SearchAsync(SearchClientsModel model, CancellationToken cancellationToken = default);
        Task<Result<ClientFluxRule>> CreateFluxRuleAsync(SaveClientFluxRuleModel model, CancellationToken cancellationToken);
        Task<Result<PagedResponse<ClientFluxRule>>> ListClientFluxRulesAsync(int clientId, CancellationToken cancellationToken);
        Task<Result<ClientFluxRule>> UpdateFluxRuleAsync(int fluxRuleId, SaveClientFluxRuleModel model, CancellationToken cancellationToken);
        Task<bool> UserHasClientAccessAsync(string userId, int clientId, CancellationToken cancellationToken);
    }
}
