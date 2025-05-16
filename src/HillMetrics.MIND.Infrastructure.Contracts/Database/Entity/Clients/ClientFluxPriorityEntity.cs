using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Clients
{
    public class ClientFluxPriorityEntity
    {
        public ClientFluxRuleEntity ClientFluxRuleEntity { get; set; } = null!;
        public int ClientFluxRuleEntityId { get; set; }

        //id to FluxEntity normalizedDb
        public int FluxId { get; set; }

        /// <summary>
        /// Priority is in ascending order
        /// </summary>
        public int Priority { get; set; }
    }

    public class FluxRuleOrderEntityConfiguration : IEntityTypeConfiguration<ClientFluxPriorityEntity>
    {
        public void Configure(EntityTypeBuilder<ClientFluxPriorityEntity> builder)
        {
            builder.ToTable("clients_flux_priorities");

            builder.HasKey(s => new { s.ClientFluxRuleEntityId, s.FluxId });

            builder.HasOne(s => s.ClientFluxRuleEntity)
                .WithMany(s => s.FluxPriorities)
                .HasForeignKey(s => s.ClientFluxRuleEntityId);
        }
    }
}
