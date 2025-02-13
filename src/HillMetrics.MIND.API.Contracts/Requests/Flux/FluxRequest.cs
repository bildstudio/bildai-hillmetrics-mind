using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    public class FluxRequest
    {
        /// <summary>
        /// Represents the type of flux.
        /// </summary>
        public FluxType? FluxType { get; set; }

        /// <summary>
        /// The flux name
        /// </summary>
        public string? FluxName { get; set; }

        /// <summary>
        /// The financial type of the flux
        /// </summary>
        public FinancialType? FluxFinancialType { get; set; }

        /// <summary>
        /// Represents the state of a flux.
        /// </summary>
        public FluxState? FluxState { get; set; }

        /// <summary>
        /// The frequence / period the flux has to fetch the data
        /// </summary>
        public TriggerPeriodDto? FetchTriggerPeriod { get; set; }

        /// <summary>
        /// The frequence / period the flux has to process the data
        /// </summary>
        public TriggerPeriodDto? ProcessTriggerPeriod { get; set; }

        /// <summary>
        /// Determine if the flux can have concurrency multi fetching (ie. multiple fetches at the same time)
        /// </summary>
        public bool? CanHaveConcurrencyMultiFetching { get; set; }

        /// <summary>
        /// The flux metadata that depend on the flux type
        /// </summary>
        public FluxMetadataDto? FluxMetadata { get; set; }

        /// <summary>
        /// The source provider of the flux
        /// </summary>
        public int? SourceId { get; set; }
    }
}
