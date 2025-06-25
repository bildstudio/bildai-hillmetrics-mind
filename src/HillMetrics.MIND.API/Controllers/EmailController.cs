using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.API.Contracts.Requests.Email;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Email;
using HillMetrics.Normalized.Domain.Contracts.Files.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class EmailController(
        IHMediator mediator,
        ILogger<EmailController> logger) : BaseHillMetricsController(mediator)
    {
        #region Email Metadata Search

        /// <summary>
        /// Search for email metadata following the given criteria
        /// </summary>
        /// <param name="request">Criteria search</param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<ActionResult<CustomMindPagedApiResponseBase<EmailMetadataResponse>>> SearchEmailMetadataAsync([FromQuery] EmailMetadataSearchRequest request)
        {
            try
            {
                // Manual mapping from request to query
                var query = new SearchMailMetadataQuery
                {
                    EmailId = request.EmailId,
                    Subject = request.Subject,
                    FromAddress = request.FromAddress,
                    ToAddress = request.ToAddress,
                    SentDateTime = request.SentDateTime,
                    ReceivedDateTime = request.ReceivedDateTime,
                    IndexedAt = request.IndexedAt,
                    HasAttachments = request.HasAttachments,
                    Folder = request.Folder,
                    AttachmentCount = request.AttachmentCount,
                    AttachmentName = request.AttachmentName,
                    AttachmentContentType = request.AttachmentContentType,
                    AttachmentMimeContentType = request.AttachmentMimeContentType,
                    AttachmentSize = request.AttachmentSize,
                    AttachmentIsProcessed = request.AttachmentIsProcessed,
                    AttachmentProcessedAt = request.AttachmentProcessedAt,
                    AttachmentFetchingHistoryId = request.AttachmentFetchingHistoryId,
                    IsProcessed = request.IsProcessed,
                    Pagination = request.Pagination,
                    Sorting = request.Sorting
                };

                var result = await Mediator.Send(query);

                if (result.IsFailed)
                    return new ErrorApiActionResult(result.Errors.ToApiResult());

                // Manual mapping from query result to response
                var mappedResults = result.Value.Results.Select(item => new EmailMetadataResponse
                {
                    Id = item.Id,
                    EmailId = item.EmailId,
                    Subject = item.Subject,
                    FromAddress = item.FromAddress,
                    ToAddress = item.ToAddress,
                    SentDateTime = item.SentDateTime,
                    ReceivedDateTime = item.ReceivedDateTime,
                    IndexedAt = item.IndexedAt,
                    HasAttachments = item.HasAttachments,
                    Folder = item.Folder,
                    AttachmentCount = item.AttachmentCount,
                    Attachments = item.Attachments.Select(attachment => new EmailAttachmentResponse
                    {
                        Id = attachment.Id,
                        AttachmentId = attachment.AttachmentId,
                        Name = attachment.Name,
                        ContentType = attachment.ContentType,
                        MimeContentType = attachment.MimeContentType,
                        Size = attachment.Size,
                        IsProcessed = attachment.IsProcessed,
                        ProcessedAt = attachment.ProcessedAt,
                        FetchingHistoryId = attachment.FetchingHistoryId
                    }).ToList()
                }).ToList();

                return new CustomMindPagedApiResponseBase<EmailMetadataResponse>(mappedResults, result.Value.NbTotalRows);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching email metadata");
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error searching email metadata: {ex.Message}"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Get email metadata by ID
        /// </summary>
        /// <param name="id">The email metadata ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseBase<EmailMetadataResponse>>> GetEmailMetadataAsync(int id)
        {
            try
            {
                // For now, we'll use the search functionality with specific ID
                var searchQuery = new SearchMailMetadataQuery();
                var result = await Mediator.Send(searchQuery);

                if (result.IsFailed)
                    return new ErrorApiActionResult(result.Errors.ToApiResult());

                var email = result.Value.Results.FirstOrDefault(x => x.Id == id);
                if (email == null)
                    return NotFound($"Email metadata with ID {id} not found");

                // Manual mapping from query result to response
                var mappedResult = new EmailMetadataResponse
                {
                    Id = email.Id,
                    EmailId = email.EmailId,
                    Subject = email.Subject,
                    FromAddress = email.FromAddress,
                    ToAddress = email.ToAddress,
                    SentDateTime = email.SentDateTime,
                    ReceivedDateTime = email.ReceivedDateTime,
                    IndexedAt = email.IndexedAt,
                    HasAttachments = email.HasAttachments,
                    Folder = email.Folder,
                    AttachmentCount = email.AttachmentCount,
                    Attachments = email.Attachments.Select(attachment => new EmailAttachmentResponse
                    {
                        Id = attachment.Id,
                        AttachmentId = attachment.AttachmentId,
                        Name = attachment.Name,
                        ContentType = attachment.ContentType,
                        MimeContentType = attachment.MimeContentType,
                        Size = attachment.Size,
                        IsProcessed = attachment.IsProcessed,
                        ProcessedAt = attachment.ProcessedAt,
                        FetchingHistoryId = attachment.FetchingHistoryId
                    }).ToList()
                };

                return new ApiResponseBase<EmailMetadataResponse>(mappedResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting email metadata with ID {EmailId}", id);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error getting email metadata: {ex.Message}"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        #endregion
    }
} 