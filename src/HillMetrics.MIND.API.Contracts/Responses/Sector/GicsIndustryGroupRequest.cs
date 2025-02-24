using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Sector
{
    public class GicsIndustryGroupRequest
    {
        public string Name { get; set; } = null!;
        public int GicsSectorId { get; set; }
    }

}
