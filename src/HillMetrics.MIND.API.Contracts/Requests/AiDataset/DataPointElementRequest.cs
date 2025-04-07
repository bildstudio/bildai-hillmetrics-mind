using System;
using System.Collections.Generic;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class DataPointElementRequest
    {
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
    }

    public class DataPointElementsRequest
    {
        /// <summary>
        /// A collection of data point elements to create
        /// </summary>
        public List<DataPointElementRequest> Elements { get; set; }
    }
}