using HillMetrics.Core.Financial;
using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset;

public class FinancialDataPointSearchResponse
{
    /// <summary>
    /// The unique identifier of the financial data point
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the financial data point
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the financial data point
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The type of financial instrument
    /// </summary>
    public FinancialType FinancialType { get; set; }

    /// <summary>
    /// The number of elements in this data point
    /// </summary>
    public int ElementsCount { get; set; }

    /// <summary>
    /// The date when the data point was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date when the data point was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public ICollection<FinancialDataPointElement> Elements { get; set; } = new List<FinancialDataPointElement>();
}