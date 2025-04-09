using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class ProcessStartedResponse
    {
        public required string Message { get; set; }
        public required int FluxId { get; set; }
        public required Guid WorkflowId { get; set; }
    }
}
