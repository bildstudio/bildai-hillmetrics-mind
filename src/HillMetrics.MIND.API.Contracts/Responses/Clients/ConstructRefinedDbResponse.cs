using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Clients
{
    public class ConstructRefinedDbResponse : ApiResponseBase<string>
    {
        public ConstructRefinedDbResponse(string data) : base(data)
        {
        }
    }
}
