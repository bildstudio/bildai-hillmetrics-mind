using HillMetrics.Core.Financial;
using HillMetrics.Core.Search;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class FileUploadSearchRequest
    {
        public string? FileName { get; set; }
        public MappingStatus? Status { get; set; }
        public FileDifficulty? Difficulty { get; set; }
        public FinancialType? FinancialType { get; set; }
        public SearchCriteria<DateTime>? UploadedAt { get; set; }
        public int? FluxId { get; set; }
        public FileOriginType OriginType { get; set; } = FileOriginType.All;
        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(FileName), SortDirection.Asc);
    }
}
