using HillMetrics.Core;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxFetchingResponse
    {
        public FluxFetchingHistoryResponse FluxFetching { get; set; } = null!;
    }

    public class FluxFetchingHistoryResponse
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The associated flux
        /// </summary>
        public int FluxId { get; set; }

        /// <summary>
        /// Id from the data that has been been matched
        /// </summary>
        public string? ExternalDataId { get; set; }

        /// <summary>
        /// Optional metadata with data that has been matched (a mail for example)
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// The datetime when the flux has identify external data
        /// </summary>
        public DateTime IdentifiedAt { get; set; }

        /// <summary>
        /// The list of content identified for this instance
        /// </summary>
        public List<FluxFetchingContentHistoryResponse> FluxFetchingContentHistories { get; set; } = [];
    }

    public class FluxFetchingContentHistoryResponse
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Flux id
        /// </summary>
        public int FluxId { get; set; }

        /// <summary>
        /// Flux matching history foreign key
        /// </summary>
        public int FluxIdentificationHistoryId { get; set; }

        /// <summary>
        /// Does the content has been correctly identified ?
        /// </summary>
        public StatusProcess ContentStatus { get; set; }

        /// <summary>
        /// Primary key of the flux content (for example the email attachment id)
        /// </summary>
        public string ExternalContentId { get; set; } = null!;

        /// <summary>
        /// The content name (for example the email attachment name)
        /// </summary>
        public string ContentName { get; set; } = null!;

        /// <summary>
        /// Keep track of Financial Raw identifier
        /// </summary>
        public string? RawId { get; set; } = null!;

        /// <summary>
        /// The datetime when the flux has identify external data
        /// </summary>
        public DateTime IdentifiedAt { get; set; }
    }
}
