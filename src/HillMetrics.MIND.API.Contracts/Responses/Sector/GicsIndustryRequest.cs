using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Sector
{
    public class GicsIndustryRequest
    {
        public string Name { get; set; } = null!;
        public int GicsIndustryGroupId { get; set; }
    }

}
