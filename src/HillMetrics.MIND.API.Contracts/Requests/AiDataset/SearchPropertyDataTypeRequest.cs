using HillMetrics.Core.Common;
using HillMetrics.Core.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class SearchPropertyDataTypeRequest
    {
        /// <summary>
        /// Optional name filter
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Optional content type filter
        /// </summary>
        public ContentType? ContentType { get; set; }

        /// <summary>
        /// Pagination parameters
        /// </summary>
        public Pagination Pagination { get; set; } = Pagination.Default;

        /// <summary>
        /// Optional sorting parameters
        /// </summary>
        public Sorting Sorting { get; set; } = new Sorting(nameof(Name), SortDirection.Ascending);
    }
}
