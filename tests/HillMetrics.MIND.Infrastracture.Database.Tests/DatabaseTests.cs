using HillMetrics.Core;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Time;
using HillMetrics.MIND.Infrastructure.Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HillMetrics.MIND.Infrastructure.Database.Tests
{
    public abstract class DatabaseTests
    {
        private readonly DbProvider _dbProvider = DbProvider.InMemory;
        public DatabaseTests(DbProvider dbProvider = DbProvider.InMemory)
        {
            _dbProvider = dbProvider;
        }

        protected MindApplicationContext _db = null!;
        protected ITimeProvider _timeProvider;
        protected ICorrelationService _correlationService;

        protected static DateTime MockNow = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        protected static DateTime MockNowABitAfter = MockNow.AddMinutes(1);



        [SetUp]
        public void SetupBase()
        {
            _timeProvider = Substitute.For<ITimeProvider>();
            _timeProvider.Now.Returns(MockNow);

            _correlationService = new CorrelationService();

            if(_dbProvider == DbProvider.InMemory)
            {
                var contextOption =
                    new DbContextOptionsBuilder<MindApplicationContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;


                _db = new MindApplicationContext(contextOption, _timeProvider, Substitute.For<ILogger<MindApplicationContext>>());

                _db.Database.EnsureCreated();
            }
            //to be able to test linq quries translated to sql, with UseInMemoryDatabase they are not translated to SQL
            else if (_dbProvider == DbProvider.SqlLiteInMemory)
            {
                
                var contextOption =
                    new DbContextOptionsBuilder<MindApplicationContext>()
                            .UseSqlite("DataSource=:memory:")
                            .Options;


                _db = new MindApplicationContext(contextOption, _timeProvider, Substitute.For<ILogger<MindApplicationContext>>());

                _db.Database.OpenConnection();
                _db.Database.EnsureCreated();
            }
        }

        [TearDown]
        public void TearDown()
        {
            _db.Database.EnsureDeleted();

            if (_dbProvider == DbProvider.SqlLiteInMemory)
                _db.Database.CloseConnection();

            _db.Dispose();
        }
    }

    public enum DbProvider
    {
        InMemory,
        SqlLiteInMemory
    }
}
