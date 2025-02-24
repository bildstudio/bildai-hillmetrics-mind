using Bogus;
using HillMetrics.Normalized.Domain.Contracts.AI;

namespace HillMetrics.MIND.API.Tests.Fakers
{
    public class AiModelPromptFaker : Faker<AiModelPrompt>
    {
        public AiModelPromptFaker()
        {
            int id = 1;

            StrictMode(true)
            .RuleFor(s => s.Id, setter => id++)
            .RuleFor(s => s.ProductType, setter => setter.Random.Enum<Core.Financial.FinancialType>())
            .RuleFor(s => s.DataType, setter => setter.Random.Enum<Core.Financial.FinancialDataPoint>())
            .RuleFor(s => s.Name, setter => setter.Name.Random.String())
            .RuleFor(s => s.FileMetadataId, setter => setter.Random.Int(1, 10000));
        }
    }
}
