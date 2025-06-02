using HillMetrics.Core.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset.DocumentTypes
{
    public class DocumentTypeDto
    {
        public DocumentTypeDto(int id, string name, FinancialType financialType)
        {
            Id = id;
            Name = name;
            FinancialType = financialType;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public FinancialType FinancialType { get; set; }
    }

    public class GetDocumentTypeResponse : ApiResponseBase<DocumentTypeDto>
    {
        public GetDocumentTypeResponse(DocumentTypeDto data) : base(data)
        {
        }
    }

    public class ListDocumentTypesResponse : ApiPagedResponseBase<DocumentTypeDto>
    {
        public ListDocumentTypesResponse(IEnumerable<DocumentTypeDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
