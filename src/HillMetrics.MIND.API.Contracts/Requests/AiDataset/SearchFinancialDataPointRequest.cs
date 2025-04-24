using HillMetrics.Core.Financial;
using HillMetrics.Core.Search;
using HillMetrics.Core.Storage.Database.Search;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset;

public class SearchFinancialDataPointRequest
{
    /// <summary>
    /// Filter by name (partial match)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filter by financial type
    /// </summary>
    public FinancialType? FinancialType { get; set; }

    /// <summary>
    /// Pagination information
    /// </summary>
    public Pagination Pagination { get; set; } = new(10, 1);

    /// <summary>
    /// Sorting information
    /// </summary>
    public Sorting? Sorting { get; set; } = new Sorting("Name", SortDirection.Asc);
}