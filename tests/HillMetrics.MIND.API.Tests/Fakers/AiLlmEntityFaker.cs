using Bogus;
using HillMetrics.Core.AI;
using HillMetrics.Normalized.Domain.Contracts.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Tests.Fakers
{
    public class AiLlmEntityFaker : Faker<AiLlmEntityDomain>
    {
        public AiLlmEntityFaker(int historyEntities = 2)
        {
            int id = 1;
            StrictMode(true)
            .RuleFor(s => s.Id, set => id + set.IndexFaker)
            .RuleFor(s => s.DocumentationUrl, s => s.Internet.Url())
            .RuleFor(s => s.LogoUrl, s => s.Internet.Url())
            .RuleFor(s => s.Provider, s => s.Company.CompanyName())
            .RuleFor(s => s.HostProvider, s => s.Random.Enum<AiProvider>())
            .RuleFor(s => s.IsActive, true)
            .RuleFor(s => s.Name, s => s.Name.Random.String())
            .RuleFor(s => s.History, set => new AiLlmHistoryEntityFaker(id + set.IndexFaker).Generate(historyEntities));
        }
    }

    public class AiLlmHistoryEntityFaker : Faker<AiLlmHistoryEntity>
    {
        public AiLlmHistoryEntityFaker(int aiLlmId)
        {
            int id = 1;

            RuleFor(s => s.Id, set => id++)
           .RuleFor(s => s.AiLlmId, aiLlmId)
           .RuleFor(s => s.Context, s => s.Random.Enum<Core.AiLlmContext>())
           .RuleFor(s => s.Prompt, s => s.Random.Word())
           .RuleFor(s => s.Response, s => s.Random.Word());
        }
    }
}
