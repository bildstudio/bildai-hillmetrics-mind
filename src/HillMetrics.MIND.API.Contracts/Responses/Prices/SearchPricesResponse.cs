using HillMetrics.Normalized.Domain.Contracts.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Prices
{
    public class SearchPricesResponse : PagedApiResponseBase<PriceEntityData>
    {
        public SearchPricesResponse(List<PriceEntityData> data, int totalRecords) : base(data, totalRecords)
        {
            
        }
    }
}
