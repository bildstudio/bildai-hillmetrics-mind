using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common;
using HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.PeerGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Clients
{
    public class ClientFluxRuleEntity : TimeableEntity, ISoftDelete
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public ClientEntity Client { get; set; } = null!;
        public bool IsDeleted { get; set; }

        public PeerGroupEntity PeerGroup { get; set; } = null!;
        public int PeerGroupId { get; set; }

        //FinancialDataPointEntity (from normalized dbContext)
        public int FinancialDataPointId { get; set; }

        /// <summary>
        /// Ranking is in ascending order
        /// </summary>cd 
        public int Ranking { get; set; }

        /// <summary>
        /// If true, defined Fluxes by HillMetrics for PeerGroupId will also be used when processing Financial
        /// </summary>
        public bool UseHmDefaultRules { get; set; } = false;

        public ICollection<ClientFluxPriorityEntity> FluxPriorities { get; set; } = new List<ClientFluxPriorityEntity>(); //[2,8,4]

        public void SetFluxPriorities(List<int> fluxIds)
        {
            FluxPriorities.Clear();
            int priority = 1;
            foreach (var fluxId in fluxIds)
            {
                FluxPriorities.Add(new ClientFluxPriorityEntity()
                {
                    FluxId = fluxId,
                    Priority = priority
                });
                priority++;
            }
        }

        public ClientFluxRule ToDomain()
        {
            ClientFluxRule clientFluxRule = new ClientFluxRule
            {
                DataPointId = this.FinancialDataPointId,
                Id = this.Id,
                PeerGroupId = this.PeerGroupId,
                Ranking = this.Ranking,
                FluxPriorityList = this.FluxPriorities.Select(s => new FluxPriority { FluxId = s.FluxId, Priority = s.Priority}).ToList()
            };

            return clientFluxRule;
        }
    }

    public class ClientFluxRuleConfiguration : IEntityTypeConfiguration<ClientFluxRuleEntity>
    {
        public void Configure(EntityTypeBuilder<ClientFluxRuleEntity> builder)
        {
            builder.ToTable("clients_flux_rules");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.HasQueryFilter(s => !s.IsDeleted);

            builder.HasOne(s => s.Client)
                .WithMany(s => s.FluxRules)
                .HasForeignKey(s => s.ClientId);

            builder.HasOne(s => s.PeerGroup)
                .WithMany(s => s.ClientFluxRules)
                .HasForeignKey(s => s.PeerGroupId);
        }
    }
}
