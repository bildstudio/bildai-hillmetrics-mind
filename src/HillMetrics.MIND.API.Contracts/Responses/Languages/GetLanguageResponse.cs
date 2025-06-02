using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Languages
{
    public class GetLanguageResponse : ApiResponseBase<LanguageDto>
    {
        public GetLanguageResponse(LanguageDto data) : base(data)
        {
        }
    }
}
