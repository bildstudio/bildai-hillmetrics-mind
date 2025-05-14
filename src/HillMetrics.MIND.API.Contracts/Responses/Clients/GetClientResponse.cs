using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Clients
{
    public class GetClientResponse : ApiResponseBase<ClientDto>
    {
        public GetClientResponse(ClientDto data) : base(data)
        {
        }
    }
}
