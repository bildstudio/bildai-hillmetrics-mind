using HillMetrics.Core;
using HillMetrics.Core.Extensions;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Clients;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups
{
    public class PeerGroupEntity : TimeableEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ComputationFrequency ComputationFrequency { get; set; }
        public DateTime DtCreate { get; set; }
        public UserAccountEntity? User { get; set; } = null!;
        public int? UserId { get; set; }
        public ICollection<PeerGroupEnginesEntity> SearchEngines { get; set; } = new List<PeerGroupEnginesEntity>();
        public ICollection<PeerGroupComputationHistoryEntity> ComputationHistory { get; set; } = new List<PeerGroupComputationHistoryEntity>();
        public ICollection<ClientFluxRuleEntity> ClientFluxRules { get; set; } = new List<ClientFluxRuleEntity>();

        public void Update(PeerGroupEntity entity)
        {
            this.Name = entity.Name;
            this.ComputationFrequency = entity.ComputationFrequency;
            this.DtUpdate = entity.DtUpdate;
            this.UserId = entity.UserId;
        }

        public void OrderSearchEngines()
        {
            //if there are 2 same orders, set order based on list items 
            if(!SearchEngines.IsNullOrEmpty() && SearchEngines.GroupBy(s => s.Order).Count() > 1)
            {
                int order = 1;
                foreach (var searchEngine in SearchEngines)
                {
                    searchEngine.Order = order;
                    order++;
                }
            }
        }
    }

    public class PeerGroupEntityConfiguration : IEntityTypeConfiguration<PeerGroupEntity>
    {
        public void Configure(EntityTypeBuilder<PeerGroupEntity> builder)
        {
            builder.ToTable("peer_groups");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Name).IsRequired();

            builder
                .HasOne(s => s.User)
                .WithMany(s => s.PeerGroups)
                .HasForeignKey(s => s.UserId)
                .IsRequired(false);
        }
    }
}
