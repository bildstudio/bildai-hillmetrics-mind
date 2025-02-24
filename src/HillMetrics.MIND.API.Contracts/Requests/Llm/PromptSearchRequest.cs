using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Requests.Common;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Contracts.Requests.Llm
{
    public class PromptSearchRequest
    {
        [FromQuery(Name = "dataType")]
        public FinancialDataPoint? DataType { get; set; }

        [FromQuery(Name = "productType")]
        public FinancialType? ProductType { get; set; }

        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        //[FromQuery(Name = "content")]
        //public string? Content { get; set; }
        public PaginationDto Pagination { get; set; } = PaginationDto.Default;
        public SortingDto? Sorting { get; set; }
    }
}
