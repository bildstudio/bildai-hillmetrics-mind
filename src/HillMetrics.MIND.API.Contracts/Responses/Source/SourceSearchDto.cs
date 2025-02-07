using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Source
{
    public class SourceSearchDto
    {
        /// <summary>
        /// The source identifier
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// The source name
        /// </summary>
        public string SourceName { get; set; } = null!;

        /// <summary>
        /// The reliability of the source
        /// </summary>
        public double Reliability { get; set; }

        /// <summary>
        /// Is the source currently active ?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The number of flux associated to this source
        /// </summary>
        public int NbFlux { get; set; }
    }
}
