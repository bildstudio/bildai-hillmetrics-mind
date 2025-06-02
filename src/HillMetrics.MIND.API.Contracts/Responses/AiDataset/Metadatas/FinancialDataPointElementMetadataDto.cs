using HillMetrics.Core.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset.Metadatas
{
    public class FinancialDataPointElementMetadataDto
    {
        public int ElementId { get; set; }

        public int DocumentTypeId { get; set; }

        public int LanguageId { get; set; }

        public Dictionary<FinancialDataPointElementMetadataKey, string?> Values { get; set; } = new Dictionary<FinancialDataPointElementMetadataKey, string?>()
        {
            { FinancialDataPointElementMetadataKey.Question, "" },
            { FinancialDataPointElementMetadataKey.Context, "" },
        };
    }
}
