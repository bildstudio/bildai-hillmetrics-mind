using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Source
{
    public class SourceCreateRequest
    {
        /// <summary>
        /// The name of the source. Should be unique
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The reliability of the source
        /// </summary>
        public double Reliability { get; set; }

        /// <summary>
        /// Is the source active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
