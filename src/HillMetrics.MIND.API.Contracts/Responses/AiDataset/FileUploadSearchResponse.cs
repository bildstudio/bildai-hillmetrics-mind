using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset
{
    public class FileUploadSearchResponse
    {
        public required int Id { get; set; }
        public required string FileName { get; set; }
        public required string FileType { get; set; }
        public required DateTime UploadedAt { get; set; }
        public required MappingStatus MappingStatus { get; set; }
        public required FileDifficulty Difficulty { get; set; }
        public required FinancialType FinancialType { get; set; }
        public int? FluxId { get; set; }

        public List<DataPointInfoResponse> DataPoints { get; set; } = new();
    }

    public class DataPointInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
