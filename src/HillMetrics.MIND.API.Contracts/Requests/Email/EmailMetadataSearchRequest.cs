using HillMetrics.Core;
using HillMetrics.Core.Search;
using System;

namespace HillMetrics.MIND.API.Contracts.Requests.Email
{
    public class EmailMetadataSearchRequest
    {
        /// <summary>
        /// Email unique identifier
        /// </summary>
        public string? EmailId { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Email sender address
        /// </summary>
        public string? FromAddress { get; set; }

        /// <summary>
        /// Email recipient address
        /// </summary>
        public string? ToAddress { get; set; }

        /// <summary>
        /// Sent date and time criteria
        /// </summary>
        public SearchCriteria<DateTime>? SentDateTime { get; set; }

        /// <summary>
        /// Received date and time criteria
        /// </summary>
        public SearchCriteria<DateTime>? ReceivedDateTime { get; set; }

        /// <summary>
        /// Indexed date and time criteria
        /// </summary>
        public SearchCriteria<DateTime>? IndexedAt { get; set; }

        /// <summary>
        /// Filter by emails with or without attachments
        /// </summary>
        public bool? HasAttachments { get; set; }

        /// <summary>
        /// Office 365 folder where the email is stored
        /// </summary>
        public string? Folder { get; set; }

        /// <summary>
        /// Number of attachments criteria
        /// </summary>
        public SearchCriteria<int>? AttachmentCount { get; set; }

        // Attachment-specific filters

        /// <summary>
        /// Attachment name
        /// </summary>
        public string? AttachmentName { get; set; }

        /// <summary>
        /// Attachment content type
        /// </summary>
        public string? AttachmentContentType { get; set; }

        /// <summary>
        /// Attachment MIME content type
        /// </summary>
        public string? AttachmentMimeContentType { get; set; }

        /// <summary>
        /// Attachment size criteria
        /// </summary>
        public SearchCriteria<long>? AttachmentSize { get; set; }

        /// <summary>
        /// Filter by processed/unprocessed attachments
        /// </summary>
        public bool? AttachmentIsProcessed { get; set; }

        /// <summary>
        /// Attachment processed date criteria
        /// </summary>
        public SearchCriteria<DateTime>? AttachmentProcessedAt { get; set; }

        /// <summary>
        /// Filter by fetching history ID
        /// </summary>
        public SearchCriteria<int>? AttachmentFetchingHistoryId { get; set; }

        /// <summary>
        /// Filter by emails that have been processed by flux or not
        /// </summary>
        public bool? IsProcessed { get; set; }

        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(ReceivedDateTime), SortDirection.Desc);
    }
} 