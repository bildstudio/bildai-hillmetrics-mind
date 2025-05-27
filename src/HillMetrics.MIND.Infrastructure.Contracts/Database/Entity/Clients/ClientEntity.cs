using HillMetrics.Core.Time;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Clients
{
    public class ClientEntity : TimeableEntity, ISoftDelete
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;
        public required string Name { get; set; }
        public required string Email { get; set; }
        public ICollection<ClientFluxRuleEntity> FluxRules { get; set; } = new List<ClientFluxRuleEntity>();
        public ICollection<UserAccountEntity> Users { get; set; } = new List<UserAccountEntity>();

        public static ClientEntity FromDomain(Domain.Contracts.Clients.ClientEntity domainEntity)
        {
            ClientEntity clientEntity = new ClientEntity
            {
                Email = domainEntity.Email,
                Name = domainEntity.Name,
                DtInsert = DateTime.UtcNow.AsUtc(),
                DtUpdate = DateTime.UtcNow.AsUtc(),
                Id = domainEntity.Id
            };

            return clientEntity;
        }

        public Domain.Contracts.Clients.ClientEntity ToDomain()
        {
            Domain.Contracts.Clients.ClientEntity domainEntity = new(Id, Name, Email, IsActive);

            return domainEntity;
        }

        public void Update(ClientEntity clientEntity)
        {
            DtUpdate = clientEntity.DtUpdate;
            Name = clientEntity.Name;
            Email = clientEntity.Email;
        }

        public void UpdateFromDomain(Domain.Contracts.Clients.ClientEntity domainEntity)
        {
            DtUpdate = DateTime.UtcNow.AsUtc();
            Name = domainEntity.Name;
            Email = domainEntity.Email;
            IsActive = domainEntity.IsActive;
        }
    }

    public class ClientEntityConfiguration : IEntityTypeConfiguration<ClientEntity>
    {
        public void Configure(EntityTypeBuilder<ClientEntity> builder)
        {
            builder.ToTable("clients");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Name).IsRequired(true);
            builder.Property(s => s.Email).IsRequired(true);
            builder.Property(s => s.IsActive).HasDefaultValue(true);

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
