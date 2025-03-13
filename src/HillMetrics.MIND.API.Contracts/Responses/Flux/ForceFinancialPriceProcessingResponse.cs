using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    /// <summary>
    /// Response after forcing financial price processing
    /// </summary>
    public class ForceFinancialPriceProcessingResponse
    {
        /// <summary>
        /// Indicates if the message was published successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Number of financial IDs processed
        /// </summary>
        public int FinancialIdCount { get; set; }

        /// <summary>
        /// Number of dates processed
        /// </summary>
        public int DatesCount { get; set; }
    }
}
