using HillMetrics.Core.Search;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs.Price;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Prices
{
    public class UpdatePriceRequest
    {
        public required int FinancialId { get; set; }
        public required DateTime Date { get; set; }
        public required string CurrencyCode { get; set; }
        public required int FluxId { get; set; }
        
        public required Core.Financial.FinancialType FinancialType { get; set; }
        public required List<PropertyValueModel> Properties { get; set; }
    }

    public class SearchPricesRequest
    {
        public string? Code { get; set; }
        public SearchCriteria<int> FinancialId { get; set; } = new();
        public SearchCriteria<int> FluxId { get; set; } = new();
        public Core.Financial.FinancialType FinancialType { get; set; }
        public string? CurrencyCode { get; set; }
        public SearchCriteria<int> FluxProcessingContentId { get; set; } = new();
        public SearchCriteria<DateTime> From { get; set; } = new();
        public SearchCriteria<DateTime> To { get; set; } = new();
        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting("Date", SortDirection.Ascending);
    }
}
