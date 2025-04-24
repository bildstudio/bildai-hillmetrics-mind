using HillMetrics.Core.Financial;
using HillMetrics.Core;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using HillMetrics.Core.Search;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    public class FluxSearchRequest
    {
        /// <summary>
        /// The flux name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Represents the type of financial instrument.
        /// </summary>
        public FinancialType? FinancialType { get; set; }

        /// <summary>
        /// Represents the type of flux.
        /// </summary>
        public FluxType? FluxType { get; set; }

        /// <summary>
        /// Represents the state of a flux.
        /// </summary>
        public FluxState? FluxState { get; set; }

        /// <summary>
        /// Last time the data has been fetched
        /// </summary>
        public SearchCriteria<DateTime>? LastFetching { get; set; }

        /// <summary>
        /// The status of the last fetching
        /// </summary>
        public StatusProcess? LastFetchingStatus { get; set; }

        /// <summary>
        /// The number of errors that occurred during the last fetching
        /// </summary>
        public SearchCriteria<int>? LastFetchingErrorCount { get; set; }

        /// <summary>
        /// Last time the data has been processed
        /// </summary>
        public SearchCriteria<DateTime>? LastProcessing { get; set; }

        /// <summary>
        /// The status of the last processing
        /// </summary>
        public StatusProcess? LastProcessingStatus { get; set; }

        /// <summary>
        /// The number of errors that occurred during the last processing
        /// </summary>
        public SearchCriteria<int>? LastProcessingErrorCount { get; set; }

        /// <summary>
        /// The number of mapping associated
        /// </summary>
        public SearchCriteria<int>? MappingsCount { get; set; }

        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(Name), SortDirection.Asc);
    }
}
