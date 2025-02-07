using HillMetrics.Core.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Source
{
    public class SourceSearchRequest
    {
        public string? Name { get; set; }

        public SearchCriteria<double>? Reliability { get; set; }
        public SearchCriteria<int>? NbFluxAssociated { get; set; }

        public Pagination Pagination { get; set; } = Pagination.Default;
        public Sorting Sorting { get; set; } = new Sorting(nameof(Name), SortDirection.Ascending);
    }
}
