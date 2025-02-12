using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    public class FluxValidateFinancialDataPointRequest
    {
        /// <summary>
        /// The identifier of the financial data point
        /// </summary>
        public int FinancialDataPointId { get; set; }
        
        /// <summary>
        /// Has been validated ?
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// New mapping if needed
        /// </summary>
        public string? HumanMetadataMapping { get; set; }
    }
}
