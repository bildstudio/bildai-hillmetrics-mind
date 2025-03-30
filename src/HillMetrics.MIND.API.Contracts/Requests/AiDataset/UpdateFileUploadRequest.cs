using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class UpdateFileUploadRequest2
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileDifficulty Difficulty { get; set; }
        public MappingStatus MappingStatus { get; set; }
    }
}
