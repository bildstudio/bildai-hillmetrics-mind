using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using Microsoft.AspNetCore.Http;
using System;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class FileUploadRequest
    {
        /// <summary>
        /// The file to upload
        /// </summary>
        public IFormFile File { get; set; }

        public FileDifficulty Difficulty { get; set; }
    }

    public class UpdateFileUploadRequest
    {
        public int FileUploadId { get; set; }

        public string FileName { get; set; } = "";

        /// <summary>
        /// The mapping status of the file upload
        /// </summary>
        public MappingStatus MappingStatus { get; set; }

        /// <summary>
        /// The difficulty level of processing this file
        /// </summary>
        public FileDifficulty Difficulty { get; set; }

        /// <summary>
        /// The financial type
        /// </summary>
        public FinancialType FinancialType { get; set; }
    }

    //public class FileUploadSearchRequest
    //{
    //    /// <summary>
    //    /// Filter by file name
    //    /// </summary>
    //    public string FileName { get; set; }

    //    /// <summary>
    //    /// Filter by mapping status
    //    /// </summary>
    //    public string MappingStatus { get; set; }

    //    /// <summary>
    //    /// Filter by upload date range (start)
    //    /// </summary>
    //    public DateTime? UploadedFromDate { get; set; }

    //    /// <summary>
    //    /// Filter by upload date range (end)
    //    /// </summary>
    //    public DateTime? UploadedToDate { get; set; }

    //    /// <summary>
    //    /// Pagination page number (1-based)
    //    /// </summary>
    //    public int Page { get; set; } = 1;

    //    /// <summary>
    //    /// Number of items per page
    //    /// </summary>
    //    public int PageSize { get; set; } = 20;
    //}
}