using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class CreateFileMappingRequest
    {
        public int Id { get; set; }
        public required int FileUploadId { get; set; }
        public required int FinancialDataPointId { get; set; }
        public string? InstanceName { get; set; }
        public ICollection<ElementValueRequest> ElementValues { get; set; } = new List<ElementValueRequest>();
    }
}
