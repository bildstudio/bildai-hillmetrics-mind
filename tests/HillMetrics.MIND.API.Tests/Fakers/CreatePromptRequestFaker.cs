using Bogus;
using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Requests.Llm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Tests.Fakers
{
    public class CreatePromptRequestFaker : Faker<CreatePromptRequest>
    {
        public CreatePromptRequestFaker()
        {
            StrictMode(true)
                .RuleFor(s => s.Name, set => set.Name.Random.String())
                //.RuleFor(s => s.DataType, set => set.Random.Enum<FinancialDataPoint>())
                .RuleFor(s => s.ProductType, set => set.Random.Enum<FinancialType>())
                .RuleFor(s => s.File, set => null!);
        }
    }
}
