using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Clients;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Users
{
    //we'll add more info later
    //user entity
    public class UserAccountEntity : TimeableEntity, ISoftDelete
    {
        public int Id { get; set; }

        /// <summary>
        /// id from external services(keycloak, etc.)
        /// </summary>
        public string? SId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public ICollection<PeerGroupEntity> PeerGroups { get; set; } = new List<PeerGroupEntity>();
        public ICollection<SearchPresetEntity> SearchPresets { get; set; } = new List<SearchPresetEntity>();

        public ClientEntity? Client { get; set; }
        public int? ClientId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UserAccountEntityConfiguration : IEntityTypeConfiguration<UserAccountEntity>
    {
        public void Configure(EntityTypeBuilder<UserAccountEntity> builder)
        {
            builder.ToTable("user_accounts");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Email).IsRequired();

            builder.HasQueryFilter(s => !s.IsDeleted);

            builder.HasOne(s => s.Client)
                .WithMany(s => s.Users)
                .HasForeignKey(s => s.ClientId)
                .IsRequired(false);
        }
    }
}
