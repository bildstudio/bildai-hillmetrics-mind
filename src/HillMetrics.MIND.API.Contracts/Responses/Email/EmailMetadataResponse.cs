using System;
using System.Collections.Generic;

namespace HillMetrics.MIND.API.Contracts.Responses.Email
{
    public class EmailMetadataResponse
    {
        public required int Id { get; set; }
        public required string EmailId { get; set; }
        public string? Subject { get; set; }
        public string? FromAddress { get; set; }
        public string? ToAddress { get; set; }
        public DateTime? SentDateTime { get; set; }
        public DateTime? ReceivedDateTime { get; set; }
        public required DateTime IndexedAt { get; set; }
        public required bool HasAttachments { get; set; }
        public string? Folder { get; set; }
        public int AttachmentCount { get; set; }
        public List<EmailAttachmentResponse> Attachments { get; set; } = new();
    }

    public class EmailAttachmentResponse
    {
        public required int Id { get; set; }
        public required string AttachmentId { get; set; }
        public string? Name { get; set; }
        public required string ContentType { get; set; }
        public string? MimeContentType { get; set; }
        public required long Size { get; set; }
        public required bool IsProcessed { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int? FetchingHistoryId { get; set; }
    }
} 