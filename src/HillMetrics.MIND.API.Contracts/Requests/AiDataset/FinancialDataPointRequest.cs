namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class FinancialDataPointRequest
    {
        /// <summary>
        /// The name of the financial data point
        /// </summary>
        public string Name { get; set; }
    }

    public class FinancialDataPointSearchRequest
    {
        /// <summary>
        /// Filter by data point name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Pagination page number (1-based)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}