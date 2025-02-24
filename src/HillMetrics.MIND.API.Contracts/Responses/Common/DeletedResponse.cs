using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Common
{
    public class DeletedResponse
    {
        public DeletedResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
