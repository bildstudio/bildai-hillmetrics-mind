using HillMetrics.Core.Search;
using HillMetrics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    public class FluxProcessingSearchRequest
    {
        /// <summary>
        /// The id of the flux
        /// </summary>
        public SearchCriteria<int>? FluxId { get; set; }
        
        /// <summary>
        /// Fetching history id
        /// </summary>
        public SearchCriteria<int>? FetchingHistoryId { get; set; }

        /// <summary>
        /// The date when the flux was fetched
        /// </summary>
        public SearchCriteria<DateTime>? ProcessingDateStart { get; set; }
        public SearchCriteria<DateTime>? ProcessingDateEnd { get; set; }

        /// <summary>
        /// The number of content that was fetched from the flux
        /// </summary>
        public SearchCriteria<int>? NbContent { get; set; }

        /// <summary>
        /// The status of the process
        /// </summary>
        public StatusProcess? Status { get; set; }

        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(FluxId), Core.Search.SortDirection.Ascending);
    }
}
