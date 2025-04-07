
using System;
using System.Collections.Generic;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset
{
    public class FileDataMappingResponse
    {
        /// <summary>
        /// The unique identifier of the file data mapping
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the file upload associated with this mapping
        /// </summary>
        public int FileUploadId { get; set; }

        /// <summary>
        /// The ID of the financial data point associated with this mapping
        /// </summary>
        public int FinancialDataPointId { get; set; }

        /// <summary>
        /// The instance name for this mapping
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// The date when the mapping was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    public class FileDataMappingListResponse
    {
        /// <summary>
        /// List of file data mappings
        /// </summary>
        public List<FileDataMappingResponse> Mappings { get; set; }
    }
}