using HillMetrics.Core.Workflow;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxErrorResponse
    {
        public FluxErrorHistoryResponse FluxError { get; set; } = null!;
    }

    public class FluxErrorHistoryResponse
    {
        /// <summary>
        /// The primary key
        /// </summary>
        public int Id { get; set; }

        public string FluxErrorType { get; set; } = null!;

        /// <summary>
        /// Flux foreign key
        /// </summary>
        public int FluxId { get; set; }

        /// <summary>
        /// Explicative message of the error
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// The type of action that was performed when the error occurred
        /// </summary>
        public FluxActionType ActionType { get; set; }

        /// <summary>
        /// Optional metadata to store additional information
        /// </summary>
        public string? Metadata { get; set; } = null!;

        public string? ExternalId { get; set; } = null;
    }
}
