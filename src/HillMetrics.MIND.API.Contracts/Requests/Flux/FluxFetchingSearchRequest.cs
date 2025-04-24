using HillMetrics.Core.Search;
using HillMetrics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    public class FluxFetchingSearchRequest
    {
        /// <summary>
        /// The id of the flux
        /// </summary>
        public SearchCriteria<int>? FluxId { get; set; }

        /// <summary>
        /// The date when the flux was fetched
        /// </summary>
        public SearchCriteria<DateTime>? FetchingDate { get; set; }

        /// <summary>
        /// The number of content that was fetched from the flux
        /// </summary>
        public SearchCriteria<int>? NbContent { get; set; }

        /// <summary>
        /// The number of processing that was done on the flux
        /// </summary>
        public SearchCriteria<int>? NbProcessing { get; set; }

        /// <summary>
        /// The data external id
        /// </summary>
        public string? ExternalDataId { get; set; }

        /// <summary>
        /// The optional metadata
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// The status of the content
        /// </summary>
        public StatusProcess? ContentStatus { get; set; }

        /// <summary>
        /// The name of the content
        /// </summary>
        public string? ContentName { get; set; }

        /// <summary>
        /// The id from the raw database
        /// </summary>
        public string? RawId { get; set; }

        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(FluxId), Core.Search.SortDirection.Asc);
    }
}
