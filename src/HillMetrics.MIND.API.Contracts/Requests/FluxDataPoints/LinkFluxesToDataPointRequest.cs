using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.FluxDataPoints
{
    public class LinkFluxesToDataPointRequest
    {
        public int FinancialDataPointId { get; set; }
        public List<int> FluxIds { get; set; } = [];
    }
}
