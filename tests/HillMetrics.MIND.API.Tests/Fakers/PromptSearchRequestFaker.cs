using Bogus;
using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Requests.Common;
using HillMetrics.MIND.API.Contracts.Requests.Llm;

namespace HillMetrics.MIND.API.Tests.Fakers
{
    public class PromptSearchRequestFaker : Faker<PromptSearchRequest>
    {
        public PromptSearchRequestFaker() : base("en")
        {
            
            StrictMode(true)
                .RuleFor(s => s.Name, set => set.Name.Random.String())
                //.RuleFor(s => s.Content, set => set.Random.String())
                .RuleFor(s => s.DataType, set => set.Random.Enum<FinancialDataPoint>())
                .RuleFor(s => s.ProductType, set => set.Random.Enum<FinancialType>())
                .RuleFor(s => s.Pagination, set => PaginationDto.Default)
                .RuleFor(s => s.Sorting, set => new SortingDto() { Direction = SortDirection.Ascending, Field = "name" });
                
        }
    }
}
