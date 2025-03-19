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
}
