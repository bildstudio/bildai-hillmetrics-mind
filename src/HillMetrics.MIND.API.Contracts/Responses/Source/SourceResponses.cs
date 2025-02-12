using HillMetrics.MIND.API.Contracts.Responses.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Source
{
    public class SourceSearchResponse(List<SourceSearchDto> data) : ApiResponseBase<List<SourceSearchDto>>(data) { }
}
