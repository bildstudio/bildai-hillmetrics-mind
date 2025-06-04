using HillMetrics.Core;
using HillMetrics.Core.Extensions;
using HillMetrics.Core.Time;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups
{
    public class PeerGroupComputationHistoryEntity : TimeableEntity
    {
        public int Id { get; set; }

        public int PeerGroupId { get; set; }
        public PeerGroupEntity PeerGroup { get; set; } = null!;

        public DateTime ComputedAt { get; set; }

        public int TotalRecords { get; set; }

        public ComputationStatus Status { get; set; }

        public string? StatusDetails { get; set; }

        public ICollection<PeerGroupComputationHistoryResultEntity> ComputationResults { get; set; } = new List<PeerGroupComputationHistoryResultEntity>();

        public void Error(string error)
        {
            Status = ComputationStatus.Failed;
            StatusDetails = error;
            ComputedAt = DateTime.UtcNow.AsUtc();
        }

        public void Completed(IEnumerable<int> financialsIds)
        {
            TotalRecords = 0;
            if (!financialsIds.IsNullOrEmpty())
            {
                foreach (var financialId in financialsIds)
                {
                    ComputationResults.Add(new PeerGroupComputationHistoryResultEntity
                    {
                        FinancialId = financialId
                    });
                }

                TotalRecords = financialsIds.Count();
            }
            else
                TotalRecords = 0;

                Status = ComputationStatus.Completed;
            StatusDetails = "Completed";
            ComputedAt = DateTime.UtcNow.AsUtc();
        }
    }

    public class PeerGroupComputationEntityConfiguration : IEntityTypeConfiguration<PeerGroupComputationHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<PeerGroupComputationHistoryEntity> builder)
        {
            builder.ToTable("peer_group_computation_history");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.ComputedAt).IsRequired(true);

            builder.HasOne(s => s.PeerGroup)
                .WithMany(s => s.ComputationHistory)
                .HasForeignKey(s => s.PeerGroupId);
        }
    }
}
