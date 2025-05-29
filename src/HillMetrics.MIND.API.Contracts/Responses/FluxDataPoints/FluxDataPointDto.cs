using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;

namespace HillMetrics.MIND.API.Contracts.Responses.FluxDataPoints
{
    public record FluxDataPointDto
    {
        public int Id { get; set; }
        public FluxType Type { get; set; }
        public FinancialType? FinancialType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public FluxState State { get; set; }
        public int Priority { get; set; }
    }

    public static class FluxDataPointDtoMapper
    {
        public static FluxDataPointDto FromDomain(this Normalized.Domain.Contracts.FluxDataPoints.FluxDataPointDto domain)
        {
            return new FluxDataPointDto
            {
                Description = domain.Description,
                FinancialType = domain.FinancialType,
                Name = domain.Name,
                State = domain.State,
                Type = domain.Type,
                Id = domain.Id,
                Priority = domain.Priority
            };
        }
    }
}
