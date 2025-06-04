using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    /// <summary>
    /// Request to force processing of normalized financial prices
    /// </summary>
    public class ForceFinancialPriceProcessingRequest
    {
        /// <summary>
        /// List of financial product identifiers
        /// </summary>
        public List<int> FinancialIds { get; set; } = new();

        /// <summary>
        /// Optional flux identifier. If not provided, the system will use the flux associated with the most recent price data.
        /// </summary>
        public int? FluxId { get; set; }

        /// <summary>
        /// List of price dates to process
        /// </summary>
        public List<DateTime> Dates { get; set; } = new();
    }
}
