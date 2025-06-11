using HillMetrics.Core.Common;
using HillMetrics.Core.Financial;
using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class CreatePropertyDataTypeRequest
    {
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
        /// The mapping source type that defines how the data is extracted
        /// </summary>
        public MappingSourceType MappingSourceType { get; set; } = MappingSourceType.Column;
    }
}
