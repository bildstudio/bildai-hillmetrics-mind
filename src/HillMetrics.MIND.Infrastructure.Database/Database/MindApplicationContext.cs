
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Time;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Clients;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Search;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Infrastructure.Database.Database
{
    /// <summary>
    /// https://www.npgsql.org/doc/types/datetime.html
    /// </summary>
    public class MindApplicationContext : DbContext
    {
        private readonly ITimeProvider _timeProvider;
        private readonly ILogger<MindApplicationContext> _logger;
        public MindApplicationContext(
            DbContextOptions<MindApplicationContext> options,
            ITimeProvider timeProvider,
            ILogger<MindApplicationContext> logger) : base(options)
        {
            _timeProvider = timeProvider;
            _logger = logger;

        }

        #region Clients

        public DbSet<ClientEntity> Clients { get; set; } = null!;
        public DbSet<ClientFluxRuleEntity> ClientsFluxRules { get; set; } = null!;
        public DbSet<ClientFluxPriorityEntity> ClientsFluxPriorities { get; set; } = null!;

        #endregion

        public DbSet<SearchPresetEntity> SearchPresets { get; set; }
        public DbSet<PeerGroupEntity> PeerGroups { get; set; }
        public DbSet<PeerGroupEnginesEntity> PeerGroupsEngines { get; set; }
        public DbSet<PeerGroupComputationHistoryEntity> PeerGroupComputationHistory { get; set; }
        public DbSet<PeerGroupComputationHistoryResultEntity> PeerGroupComputationHistoryResults { get; set; }
        public DbSet<UserAccountEntity> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // All entities that implement ISoftDelete should not return soft deleted elements

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientEntity).Assembly);
            _logger.LogInformation($"{nameof(MindApplicationContext)}.OnModelCreating done");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSaveChangesInternal();

            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            HandleSaveChangesInternal();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            HandleSaveChangesInternal();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Actions applied before saving changes
        /// </summary>
        private void HandleSaveChangesInternal()
        {
            //save data in tracking Dictionary?

            SetDates();
            EnsureDateIsUtc();
            HandleSoftDelete();
            SetAuditsUniqueReferences();
        }

        /// <summary>
        /// Set DtInsert and DtUpdate automatically
        /// </summary>
        private void SetDates()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is TimeableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var timeableEntity = (TimeableEntity)entry.Entity;

                if (timeableEntity.DtUpdate != default)
                    timeableEntity.DtUpdate = _timeProvider.Now;

                if (timeableEntity.DtInsert == default && entry.State == EntityState.Added)
                {
                    timeableEntity.DtInsert = _timeProvider.Now;
                    timeableEntity.DtUpdate = _timeProvider.Now;
                }
            }
        }

        /// <summary>
        /// Ensure that every <see cref="DateTime"/> is UTC
        /// </summary>
        private void EnsureDateIsUtc()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                foreach (var property in entry.Properties.Where(p => p.Metadata.ClrType == typeof(DateTime)))
                {
                    if (property.CurrentValue is DateTime dt && dt.Kind != DateTimeKind.Utc)
                    {
                        property.CurrentValue = DateChecker.EnsureDateIsUtc(dt);
                    }
                }
            }
        }

        /// <summary>
        /// Change deleted entries to modified and set the IsDeleted flag to true
        /// </summary>
        private void HandleSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ISoftDelete entity && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;

                    //CascadeSoftDelete(entry.Entity);
                }
            }
        }

        private void CascadeSoftDelete(object entity)
        {
            var navigations = Entry(entity).Navigations;

            foreach (var navigation in navigations)
            {
                if (navigation.CurrentValue is IEnumerable<object> collection)
                {
                    foreach (var child in collection)
                    {
                        if (child is ISoftDelete childEntity && !childEntity.IsDeleted)
                        {
                            childEntity.IsDeleted = true;
                            CascadeSoftDelete(child);
                        }
                    }
                }
                else if (navigation.CurrentValue is ISoftDelete singleChild && !singleChild.IsDeleted)
                {
                    singleChild.IsDeleted = true;
                    CascadeSoftDelete(singleChild);
                }
            }
        }

        /// <summary>
        /// Set Audits uniq references
        /// </summary>
        private void SetAuditsUniqueReferences()
        {
            var auditableEntries = ChangeTracker.Entries()
                                                .Where(s => s.Entity is IAuditable auditable &&
                                                    string.IsNullOrEmpty(auditable.AuditReference) &&
                                                    (s.State == EntityState.Added || s.State == EntityState.Modified)
                                                 );

            foreach (var entry in auditableEntries)
            {
                //set auditReference on new and old entities that are missing it
                IAuditable entity = (IAuditable)entry.Entity;
                entity.AuditReference = Guid.NewGuid().ToString();
            }
        }
    }
}
