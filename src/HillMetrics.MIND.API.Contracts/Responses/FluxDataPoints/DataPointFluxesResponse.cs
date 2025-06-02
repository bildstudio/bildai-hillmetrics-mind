using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.FluxDataPoints
{
    public class ListDataPointFluxesResponse : CustomMindPagedApiResponseBase<FluxDataPointDto>
    {
        public ListDataPointFluxesResponse(IEnumerable<FluxDataPointDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
