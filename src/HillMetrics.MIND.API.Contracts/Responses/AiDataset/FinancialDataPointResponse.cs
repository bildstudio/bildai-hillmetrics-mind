using System;
using System.Collections.Generic;
using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset
{
    public class FinancialDataPointResponse
    {
        /// <summary>
        /// The unique identifier of the financial data point
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the financial data point
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date when the financial data point was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The primitive data type of this financial data point for mapping
        /// </summary>
        public MappingTypePrimitive MappingPrimitiveValue { get; set; }
    }

    public class FinancialDataPointListResponse
    {
        /// <summary>
        /// List of financial data points
        /// </summary>
        public List<FinancialDataPointResponse> DataPoints { get; set; }

        /// <summary>
        /// Total number of items matching the criteria
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }
    }
}