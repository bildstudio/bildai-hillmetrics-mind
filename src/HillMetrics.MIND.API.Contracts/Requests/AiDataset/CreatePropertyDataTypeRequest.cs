using HillMetrics.Core.Common;
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
    }
}
