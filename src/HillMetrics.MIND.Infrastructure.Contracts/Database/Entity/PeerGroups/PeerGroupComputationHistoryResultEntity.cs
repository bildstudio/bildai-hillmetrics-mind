using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups
{
    public class PeerGroupComputationHistoryResultEntity
    {
        /// <summary>
        /// reference of financialId from Refined DB
        /// </summary>
        public int FinancialId { get; set; }
        public PeerGroupComputationHistoryEntity History { get; set; } = null!;
        public int HistoryId { get; set; }
    }

    public class PeerGroupComputationHistoryResultEntityConfiguration : IEntityTypeConfiguration<PeerGroupComputationHistoryResultEntity>
    {
        public void Configure(EntityTypeBuilder<PeerGroupComputationHistoryResultEntity> builder)
        {
            builder.ToTable("peer_group_computation_history_results");
            builder.HasKey(s => new { s.FinancialId, s.HistoryId});

            builder.HasOne(s => s.History)
                .WithMany(s => s.ComputationResults)
                .HasForeignKey(s => s.HistoryId);
        }
    }
}
