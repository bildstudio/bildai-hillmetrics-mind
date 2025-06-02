using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Languages
{
    public class SaveLanguageRequest
    {
        public required string Name { get; set; }
        public required string TwoLetterCode { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
