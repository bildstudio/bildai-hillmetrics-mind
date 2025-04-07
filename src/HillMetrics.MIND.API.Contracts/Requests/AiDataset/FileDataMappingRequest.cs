namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class FileDataMappingRequest
    {
        /// <summary>
        /// The ID of the file upload associated with this mapping
        /// </summary>
        public string FileUploadId { get; set; }

        /// <summary>
        /// The ID of the financial data point associated with this mapping
        /// </summary>
        public string FinancialDataPointId { get; set; }

        /// <summary>
        /// The instance name for this mapping
        /// </summary>
        public string InstanceName { get; set; }
    }
}