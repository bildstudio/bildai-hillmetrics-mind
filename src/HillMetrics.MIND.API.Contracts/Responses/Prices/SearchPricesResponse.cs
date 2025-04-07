using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Prices
{
    public class SearchPricesResponse// : PagedApiResponseBase<PriceEntityData>
    {
        public string Code { get; set; } = string.Empty;
        public FinancialCodeIdentifier FinancialCodeIdentifier { get; set; }
        public string Mic { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int FinancialId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int FluxId { get; set; }

        public int FluxProcessingContentId { get; set; }
        public string? AuditReference { get; set; }
        public DateTime DtInsert { get; set; }
        public DateTime DtUpdate { get; set; }

        public Core.Financial.FinancialType FinancialType { get; set; }

        public List<PropertyValueResponse> FinancialProperties { get; set; } = [];

        public class PropertyValueResponse
        {
            public PropertyValueResponse(string name, object? value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public object? Value { get; set; }
        }
    }
}
