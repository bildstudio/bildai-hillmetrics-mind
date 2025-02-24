using HillMetrics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxProcessingSearchReponse
    {
        public int Id { get; set; }
        public int FluxId { get; set; }
        public DateTime ProcessingDateStart { get; set; }
        public DateTime? ProcessingDateEnd { get; set; }
        public int NbContent { get; set; }
        public StatusProcess? Status { get; set; }
    }
}
