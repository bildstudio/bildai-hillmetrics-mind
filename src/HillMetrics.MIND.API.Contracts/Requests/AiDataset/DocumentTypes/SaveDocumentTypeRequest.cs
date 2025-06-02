using HillMetrics.Core.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset.DocumentTypes
{
    public class SaveDocumentTypeRequest
    {
        public required string Name { get; set; }
        public FinancialType FinancialType { get; set; }
    }
}
