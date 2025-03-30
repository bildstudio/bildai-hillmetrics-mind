using System.ComponentModel.DataAnnotations;
using HillMetrics.Core.Financial;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset;

public class CreateFinancialDataPointRequest
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public FinancialType FinancialType { get; set; }

    public List<FinancialDataPointElementRequest> Elements { get; set; } = new List<FinancialDataPointElementRequest>();
}

public class FinancialDataPointElementRequest
{
    public int Id { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public List<string> PotentialValues { get; set; } = [];
    public string? Description { get; set; }
    public int? Position { get; set; }
    public int? FinancialDataPointId { get; set; }
}