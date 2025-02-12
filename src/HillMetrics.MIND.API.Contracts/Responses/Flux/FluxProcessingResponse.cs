using HillMetrics.Core;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxProcessingResponse
    {
        public FluxProcessingHistoryResponse FluxProcessingHistory { get; set; } = null!;
    }

    public class FluxProcessingHistoryResponse
    {
        public int Id { get; set; }

        /// <summary>
        /// The flux related to this history
        /// </summary>
        public int FluxId { get; set; }

        /// <summary>
        /// When the flux is started
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// When the flux has ended
        /// </summary>
        public DateTime? EndedAt { get; set; }

        /// <summary>
        /// Optional message
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// The correlationId to keep track of this flux in the log
        /// </summary>
        public string CorrelationId { get; set; } = null!;

        public List<FluxProcessingContentHistoryResponse> FluxProcessingContentHistory { get; set; } = [];
    }

    public class FluxProcessingContentHistoryResponse
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Flux processing history foreign key
        /// </summary>
        public int FluxProcessingHistoryId { get; set; }

        /// <summary>
        /// Foreign key to the content identification history
        /// </summary>
        public int FluxIdentificationContentHistoryId { get; set; }

        /// <summary>
        /// When the flux content processing is started
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// When the flux content processing has ended
        /// </summary>
        public DateTime? EndedAt { get; set; }

        /// <summary>
        /// The status of the flux content processing
        /// </summary>
        public StatusProcess Status { get; set; }
    }
}
