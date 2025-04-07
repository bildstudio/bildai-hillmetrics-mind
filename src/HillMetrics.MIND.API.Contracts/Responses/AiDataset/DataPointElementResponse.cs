using System;
using System.Collections.Generic;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset
{
    public class DataPointElementResponse
    {
        /// <summary>
        /// The unique identifier of the data point element
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the data point element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the data point element
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The date associated with this data point element
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The ID of the financial data point this element belongs to
        /// </summary>
        public string FinancialDataPointId { get; set; }

        /// <summary>
        /// The date when the data point element was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    public class DataPointElementListResponse
    {
        /// <summary>
        /// List of data point elements
        /// </summary>
        public List<DataPointElementResponse> Elements { get; set; }
    }
}