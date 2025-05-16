using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Search
{
    public class SearchPresetEntity : TimeableEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Filters { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserAccountEntity? User { get; set; } = null!;
        /// <summary>
        /// if null, means it is created by the system
        /// </summary>
        public int? UserId { get; set; }//clientEntity table

        public ICollection<PeerGroupEnginesEntity> SearchEngines { get; set; } = new List<PeerGroupEnginesEntity>();
    }

    public class SearchPresetConfiguration : IEntityTypeConfiguration<SearchPresetEntity>
    {
        public void Configure(EntityTypeBuilder<SearchPresetEntity> builder)
        {
            builder.ToTable("search_preset");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Name).IsRequired();
            builder.Property(s => s.Filters).IsRequired();

            builder
                .HasOne(s => s.User)
                .WithMany(s => s.SearchPresets)
                .HasForeignKey(s => s.UserId)
                .IsRequired(false);
        }
    }
}
