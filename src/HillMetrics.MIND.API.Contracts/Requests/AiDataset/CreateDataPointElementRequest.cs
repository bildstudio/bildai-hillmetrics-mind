using System.ComponentModel.DataAnnotations;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset;

public class CreateDataPointElementRequest
{
    [Required]
    public int FinancialDataPointId { get; set; }

    [Required]
    [StringLength(100)]
    public string PropertyName { get; set; } = string.Empty;

    public string? PotentialValues { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public int Position { get; set; }
}