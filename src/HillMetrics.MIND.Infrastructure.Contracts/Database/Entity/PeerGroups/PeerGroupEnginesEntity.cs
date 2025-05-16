using HillMetrics.Core;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups
{
    public class PeerGroupEnginesEntity : TimeableEntity
    {
        public int PeerGroupId { get; set; }
        public PeerGroupEntity PeerGroup { get; set; } = null!;
        public int SearchPresetId { get; set; }
        public SearchPresetEntity SearchPreset { get; set; } = null!;
        public int Order { get; set; }
        public SetOperator Operator { get; set; }
    }

    public class PeerGroupEnginesEntityConfiguration : IEntityTypeConfiguration<PeerGroupEnginesEntity>
    {
        public void Configure(EntityTypeBuilder<PeerGroupEnginesEntity> builder)
        {
            builder.ToTable("peer_groups_engines");

            builder.HasKey(s => new { s.PeerGroupId, s.SearchPresetId });

            builder.HasOne(s => s.PeerGroup)
                .WithMany(s => s.SearchEngines)
                .HasForeignKey(s => s.PeerGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.SearchPreset)
                .WithMany(s => s.SearchEngines)
                .HasForeignKey(s => s.SearchPresetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.Order).IsRequired();
        }
    }
}
