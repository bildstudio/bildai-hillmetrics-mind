using HillMetrics.MIND.Domain.Contracts.Services;
using NSubstitute;
using HillMetrics.MIND.Infrastructure.Database.Services;
using Microsoft.Extensions.Logging;
using HillMetrics.Core.Storage.Database.Contracts;
using HillMetrics.MIND.Infrastructure.Database.Database;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Storage.Database.UOW;
using HillMetrics.Core.Shared.Tests.DbContainers;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups;

namespace HillMetrics.MIND.Infrastructure.Database.Tests.Services
{
    [TestFixture]
    public class ClientServiceTests //: DatabaseTests
    {

        private IClientService _clientService;
        private ILogger<ClientService> _logger;
        private IUnitOfWork<MindApplicationContext> _unitOfWork;
        private ITimeProvider _timeProvider;
        private ContainerContext<MindApplicationContext> _containerContext;

        [SetUp]
        public async Task Setup()
        {
            _timeProvider = Substitute.For<ITimeProvider>();
            _containerContext = await DbContextTestContainerBuilder.BuildMindApplicationDbContextAsync("mind-test-db", _timeProvider);


            _logger = Substitute.For<ILogger<ClientService>>();
            _unitOfWork = new UnitOfWork<MindApplicationContext>(_containerContext.DbContext);

            _clientService = new ClientService(_logger, _unitOfWork, _timeProvider);
        }

        [TearDown]
        public void Cleanup()
        {
            if(_unitOfWork != null)
                _unitOfWork.Dispose();

            _containerContext.Dispose();
        }

        [Test]
        public async Task CreateAsync()
        {
            var model = new SaveClientModel("test name", "test@test.com");
            var client = await CreateTestClientAsync(model);

            var clientEntityResult = await _clientService.GetAsync(client.Id, CancellationToken.None);
            Assert.That(clientEntityResult, Is.Not.Null);
            Assert.That(clientEntityResult.IsSuccess, Is.True);

            var entity = clientEntityResult.Value;
            Assert.That(entity.Name, Is.EqualTo(model.Name));
            Assert.That(entity.Email, Is.EqualTo(model.Email));
        }


        [Test]
        public async Task UpdateAsync()
        {
            var model = new SaveClientModel("test name", "test@test.com");
            var client = await CreateTestClientAsync(model);

            model = new Domain.Contracts.Clients.Models.SaveClientModel("test name", "test123@1.com");

            var result = await _clientService.UpdateAsync(client.Id, model, CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);

            var clientEntityResult = await _clientService.GetAsync(client.Id, CancellationToken.None);
            Assert.That(clientEntityResult, Is.Not.Null);
            Assert.That(clientEntityResult.IsSuccess, Is.True);

            var entity = clientEntityResult.Value;
            Assert.That(entity.Name, Is.EqualTo(model.Name));
            Assert.That(entity.Email, Is.EqualTo(model.Email));
        }

        [Test]
        public async Task DeleteAsync()
        {
            var model = new SaveClientModel("test name", "test@test.com");
            var client = await CreateTestClientAsync(model);


            var deleteResult = await _clientService.DeleteAsync(client.Id, CancellationToken.None);
            Assert.That(deleteResult, Is.Not.Null);
            Assert.That(deleteResult.IsSuccess, Is.True);

            var clientEntityResult = await _clientService.GetAsync(client.Id, CancellationToken.None);

            Assert.That(clientEntityResult, Is.Not.Null);
            Assert.That(clientEntityResult.IsSuccess, Is.False);
        }

        [Test]
        public async Task CreateAsync_ExistingEmail()
        {
            var model = new SaveClientModel("test name", "test@test.com");
            await CreateTestClientAsync(model);

            var newCreateResult = await _clientService.CreateAsync(model, CancellationToken.None);
            Assert.That(newCreateResult, Is.Not.Null);
            Assert.That(newCreateResult.IsSuccess, Is.False);
        }

        [Test]
        public async Task CreateFluxRule_RuleCreated()
        {
            var clientModel = new SaveClientModel("test name", "test@test.com");
            var client = await CreateTestClientAsync(clientModel);
            var peerGroupRepo = _unitOfWork.GetRepository<PeerGroupEntity>();
            var peerGroup = new PeerGroupEntity { Name = "test" };
            peerGroupRepo.Add(peerGroup);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            var fluxModel = new SaveClientFluxRuleModel(1, peerGroup.Id, 1, [1, 2, 3], client.Id); 
            var result = await _clientService.CreateFluxRuleAsync(fluxModel, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
        }


        [Test]
        public async Task CreateFluxRule_RuleExisting_RuleNotCreated()
        {
            var clientModel = new SaveClientModel("test name", "test@test.com");
            var client = await CreateTestClientAsync(clientModel);
            var peerGroupRepo = _unitOfWork.GetRepository<PeerGroupEntity>();
            var peerGroup = new PeerGroupEntity { Name = "test" };
            peerGroupRepo.Add(peerGroup);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            var fluxModel = new SaveClientFluxRuleModel(1, peerGroup.Id, 1, [1, 2, 3], client.Id);
            var result = await _clientService.CreateFluxRuleAsync(fluxModel, CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);

            var resultNew = await _clientService.CreateFluxRuleAsync(fluxModel, CancellationToken.None);

            Assert.That(resultNew, Is.Not.Null);
            Assert.That(resultNew.IsSuccess, Is.False);
        }


        [Test]
        public async Task UpdateFluxRule_RuleUpdated()
        {
            var clientModel = new SaveClientModel("test name", "test@test.com");
            var client = await CreateTestClientAsync(clientModel);
            var peerGroupRepo = _unitOfWork.GetRepository<PeerGroupEntity>();
            var peerGroup = new PeerGroupEntity { Name = "test" };
            peerGroupRepo.Add(peerGroup);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            var fluxModel = new SaveClientFluxRuleModel(1, peerGroup.Id, 1, [1, 2, 3], client.Id);
            var result = await _clientService.CreateFluxRuleAsync(fluxModel, CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);

            fluxModel = new SaveClientFluxRuleModel(1, peerGroup.Id, 10, [1, 6, 8, 9, 2], client.Id);

            var resultNew = await _clientService.UpdateFluxRuleAsync(result.Value.Id, fluxModel, CancellationToken.None);

            Assert.That(resultNew, Is.Not.Null);
            Assert.That(resultNew.IsSuccess, Is.True);
        }


        private async Task<ClientEntity> CreateTestClientAsync(SaveClientModel model)
        {
            var result = await _clientService.CreateAsync(model, CancellationToken.None);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);

            return result.Value;
        }
    }
}