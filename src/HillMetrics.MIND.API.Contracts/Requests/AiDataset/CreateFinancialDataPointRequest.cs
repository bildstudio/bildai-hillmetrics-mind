using System.ComponentModel.DataAnnotations;
using HillMetrics.Core.Financial;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset;

public class CreateFinancialDataPointRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<FinancialDataPointElement>? Elements { get; set; }
}