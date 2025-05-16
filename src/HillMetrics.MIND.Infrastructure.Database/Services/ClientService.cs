using FluentResults;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Errors;
using HillMetrics.Core.Storage.Database.Contracts;
using Microsoft.Extensions.Logging;
using HillMetrics.Core.Common;
using HillMetrics.Core.Extensions;
using HillMetrics.Core.EFCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using HillMetrics.MIND.Infrastructure.Database.Database;
using HillMetrics.MIND.Domain.Contracts.Services;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Users;

namespace HillMetrics.MIND.Infrastructure.Database.Services
{
    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly IUnitOfWork<MindApplicationContext> _unitOfWork;
        private readonly IRepository<ClientEntity> _clientRepository;
        private readonly IRepository<ClientFluxRuleEntity> _clientFluxRuleRepository;
        private readonly IRepository<UserAccountEntity> _userAccountsRepository;
        private readonly ITimeProvider _timeProvider;

        public ClientService(
            ILogger<ClientService> logger, 
            IUnitOfWork<MindApplicationContext> unitOfWork, 
            ITimeProvider timeProvider)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _clientRepository = _unitOfWork.GetRepository<ClientEntity>();
            _clientFluxRuleRepository = _unitOfWork.GetRepository<ClientFluxRuleEntity>();
            _userAccountsRepository = _unitOfWork.GetRepository<UserAccountEntity>();
            _timeProvider = timeProvider;
        }

        public async Task<Result<Domain.Contracts.Clients.ClientEntity>> GetAsync(int id, CancellationToken cancellationToken)
        {
            ClientEntity? entity = await _clientRepository.FindAsync([id], cancellationToken);

            if (entity == null || entity.IsDeleted)
                return Result.Fail(new NotFoundError($"Client with Id: {id} not found."));

            Domain.Contracts.Clients.ClientEntity domainEntity = entity.ToDomain();

            return domainEntity;
        }

        public async Task<Result<Domain.Contracts.Clients.ClientEntity>> CreateAsync(Domain.Contracts.Clients.Models.SaveClientModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = new ClientEntity()
                {
                    Email = model.Email,
                    Name = model.Name,
                    DtInsert = _timeProvider.Now,
                    DtUpdate = _timeProvider.Now,
                };

                var clientUnique = await IsClientUniqueAsync(entity);
                if (!clientUnique.Item1)
                    return Result.Fail(new ConflictError(clientUnique.Item2));

                _clientRepository.Add(entity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return entity.ToDomain();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }

        public async Task<Result<Domain.Contracts.Clients.ClientEntity>> UpdateAsync(int id, Domain.Contracts.Clients.Models.SaveClientModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                ClientEntity? entity = await _clientRepository.FindAsync([id], cancellationToken);

                if (entity == null || entity.IsDeleted)
                    return Result.Fail(new NotFoundError($"Client with Id: {id} not found."));

                entity.Name = model.Name;
                entity.Email = model.Email;
                entity.DtUpdate = _timeProvider.Now;

                var clientUnique = await IsClientUniqueAsync(entity);
                if (!clientUnique.Item1)
                    return Result.Fail(new ConflictError(clientUnique.Item2));

                _clientRepository.Update(entity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return entity.ToDomain();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync error: {ExceptionMessage}, when deleting client with id: {clientId}", ex.Message, id);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                ClientEntity? entity = await _clientRepository.FindAsync([id], cancellationToken);

                if (entity == null || entity.IsDeleted)
                    return Result.Fail(new NotFoundError($"Client with Id: {id} not found."));

                _clientRepository.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync error: {ExceptionMessage}, when deleting client with id: {clientId}", ex.Message, id);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }

        public async Task<Result<PagedResponse<Domain.Contracts.Clients.ClientEntity>>> SearchAsync(Domain.Contracts.Clients.Models.SearchClientsModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                Expression<Func<ClientEntity, bool>> filter = s => !s.IsDeleted;
                if(!string.IsNullOrEmpty(model.Name))
                    filter = filter.And(EfLikeBuilder.Build<ClientEntity>(s => s.Name, $"%{model.Name}%"));

                if (!string.IsNullOrEmpty(model.Email))
                    filter = filter.And(EfLikeBuilder.Build<ClientEntity>(s => s.Email, $"%{model.Email}%"));

                var query = _clientRepository.Where(filter);
                var total = await query.CountAsync(cancellationToken);

                var data = await query.OrderBy(s => s.Name).Skip(model.Pagination.Skip).Take(model.Pagination.PageSize).ToListAsync(cancellationToken);

                var domainEntities = data.Select(s => s.ToDomain()).ToList();

                return new PagedResponse<Domain.Contracts.Clients.ClientEntity>(domainEntities, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }
        
        public async Task<Result<Domain.Contracts.Clients.ClientFluxRule>> CreateFluxRuleAsync(SaveClientFluxRuleModel model, CancellationToken cancellationToken)
        {
            //check if there is already existing repo for same client, peerGroup, dataPoint
            ClientFluxRuleEntity entity = await _clientFluxRuleRepository.SingleOrDefaultAsync(s => s.ClientId == model.ClientId && s.FinancialDataPointId == model.DataPointId && s.PeerGroupId == model.PeerGroupId, cancellationToken: cancellationToken);
            if (entity != null)
                return Result.Fail(new ConflictError($"FluxRule for client: {model.ClientId}, financialDataPoint: {model.DataPointId} and peerGroup: {model.PeerGroupId} already exists."));

            entity = new ClientFluxRuleEntity
            {
                ClientId = model.ClientId,
                PeerGroupId = model.PeerGroupId,
                FinancialDataPointId = model.DataPointId,
                Ranking = model.Ranking,
                DtInsert = _timeProvider.Now,
                DtUpdate = _timeProvider.Now
            };

            entity.SetFluxPriorities(model.FluxPriorityList);

            _clientFluxRuleRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.ToDomain();
        }

        public async Task<Result<PagedResponse<Domain.Contracts.Clients.ClientFluxRule>>> ListClientFluxRulesAsync(int clientId, CancellationToken cancellationToken)
        {
            var clientResult = await GetAsync(clientId, cancellationToken);
            if (clientResult.IsFailed)
                return clientResult.ToResult<PagedResponse<Domain.Contracts.Clients.ClientFluxRule>>();

            var fluxRules = await _clientFluxRuleRepository.Where(s => s.ClientId == clientId, includeProperties: nameof(ClientFluxRuleEntity.FluxPriorities)).ToListAsync(cancellationToken);

            var domainEntities = fluxRules.Select(s => s.ToDomain()).ToList();

            return new PagedResponse<Domain.Contracts.Clients.ClientFluxRule>(domainEntities, domainEntities.Count);
        }
        
        public async Task<Result<Domain.Contracts.Clients.ClientFluxRule>> UpdateFluxRuleAsync(int fluxRuleId, SaveClientFluxRuleModel model, CancellationToken cancellationToken)
        {
            ClientFluxRuleEntity? entityToUpdate = await _clientFluxRuleRepository.SingleOrDefaultAsync(s => s.Id == fluxRuleId, includeProperties: nameof(ClientFluxRuleEntity.FluxPriorities), cancellationToken: cancellationToken);
            if (entityToUpdate == null)
                return Result.Fail(new ConflictError($"FluxRule with id: {fluxRuleId} not found."));

            //validate that there is no existing
            ClientFluxRuleEntity existingEntity = await _clientFluxRuleRepository.SingleOrDefaultAsync(s => s.Id != fluxRuleId && s.ClientId == model.ClientId && s.FinancialDataPointId == model.DataPointId && s.PeerGroupId == model.PeerGroupId, cancellationToken: cancellationToken);
            if (existingEntity != null)
                return Result.Fail(new ConflictError($"FluxRule for client: {model.ClientId}, financialDataPoint: {model.DataPointId} and peerGroup: {model.PeerGroupId} already exists."));


            entityToUpdate.Ranking = model.Ranking;
            entityToUpdate.FinancialDataPointId = model.DataPointId;
            entityToUpdate.PeerGroupId = model.PeerGroupId;
            entityToUpdate.DtUpdate = _timeProvider.Now;


            

            int priority = 0;
            foreach (var fluxId in model.FluxPriorityList)
            {
                var existingRulePriority = entityToUpdate.FluxPriorities.FirstOrDefault(s => s.FluxId == fluxId);
                if(existingRulePriority != null)
                {
                    existingRulePriority.Priority = priority;
                }
                else
                {
                    entityToUpdate.FluxPriorities.Add(new ClientFluxPriorityEntity
                    {
                        FluxId = fluxId,
                        Priority = priority
                    });
                }

                priority++;
            }

            List<ClientFluxPriorityEntity> prioritiesToRemove = entityToUpdate.FluxPriorities.Where(s => !model.FluxPriorityList.Contains(s.FluxId)).ToList();
            foreach (var priorityToRemove in prioritiesToRemove)
            {
                entityToUpdate.FluxPriorities.Remove(priorityToRemove);
            }
           
            _clientFluxRuleRepository.Update(entityToUpdate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entityToUpdate.ToDomain();

        }
        
        public async Task<bool> UserHasClientAccessAsync(string userId, int clientId, CancellationToken cancellationToken)
        {
            var user = await _userAccountsRepository.SingleOrDefaultAsync(s => s.SId == userId && s.IsActive && s.Client.Id == clientId, cancellationToken: cancellationToken);

            return user != null;
        }

        private async Task<(bool, string)> IsClientUniqueAsync(ClientEntity client)
        {
            Expression<Func<ClientEntity, bool>> filterExpression = 
                s => s.Id != client.Id && !s.IsDeleted &&
                    (s.Name.ToLower() == client.Name.ToLower() || s.Email.ToLower() == client.Email.ToLower());
           

            var existing = await _clientRepository.SingleOrDefaultAsync(filterExpression);

            if(existing == null)
                return (true, string.Empty);

            string error = string.Empty;
            if (existing.Name.Equals(client.Name, StringComparison.OrdinalIgnoreCase))
                error += $"Client with same name:'{client.Name}' already exists. ";

            if (existing.Email.Equals(client.Email, StringComparison.OrdinalIgnoreCase))
                error += $"Client with same email:'{client.Email}' already exists.";

            return (false, error);
        }
        
    }
}
