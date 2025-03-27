using Bogus;
using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Requests.Llm;

namespace HillMetrics.MIND.API.Tests.Fakers
{
    public class UpdatePromptRequestFaker : Faker<UpdatePromptRequest>
    {
        public UpdatePromptRequestFaker()
        {
            StrictMode(true)
                .RuleFor(s => s.Name, set => set.Name.Random.String())
                .RuleFor(s => s.File, set => null)
                //.RuleFor(s => s.DataType, set => set.Random.Enum<FinancialDataPoint>())
                .RuleFor(s => s.ProductType, set => set.Random.Enum<FinancialType>());
        }
    }
}
