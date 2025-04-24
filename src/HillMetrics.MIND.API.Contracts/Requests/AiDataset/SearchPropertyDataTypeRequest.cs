using HillMetrics.Core.Common;
using HillMetrics.Core.Financial;
using HillMetrics.Core.Search;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
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
        /// The primitive data type of this property for mapping
        /// </summary>
        public MappingTypePrimitive? MappingPrimitiveValue { get; set; }

        /// <summary>
        /// Pagination parameters
        /// </summary>
        public Pagination Pagination { get; set; } = Pagination.Default;

        /// <summary>
        /// Optional sorting parameters
        /// </summary>
        public Sorting Sorting { get; set; } = new Sorting(nameof(Name), SortDirection.Asc);
    }
}
