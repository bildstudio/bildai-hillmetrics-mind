using HillMetrics.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset
{
    public class PropertyDataTypeResponse
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the property data type
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description of the property data type
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Content type associated with this property data type
        /// </summary>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// Number of element values using this type
        /// </summary>
        public int ElementValuesCount { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update date
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
