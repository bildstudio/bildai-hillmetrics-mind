using HillMetrics.Core.Common;
using HillMetrics.Core.Financial;
using HillMetrics.Core.Time.Trigger;
using HillMetrics.Core;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using HillMetrics.Core.Workflow;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using HillMetrics.Core.Financial.DataPoint;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxResponse
    {
        public int Id { get; set; }
        public FluxType FluxType { get; set; }
        public FinancialType FinancialType { get; set; }
        public string FluxName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Comment { get; set; }
        public FluxState FluxState { get; set; }
        public TriggerPeriodDto FetchTriggerPeriod { get; set; } = new TriggerPeriodDto();
        public TriggerPeriodDto? ProcessTriggerPeriod { get; set; }
        public bool CanHaveConcurrencyMultiFetching { get; set; }
        public FluxMetadataDto FluxMetadata { get; set; } = null!;
        public SourceProviderDto? Source { get; set; }
        public List<FluxProcessingHistoryDto> FluxProcessingHistory { get; set; } = new();
        public List<FluxIdentificationHistoryDto> FluxIdentificationHistory { get; set; } = new();
        public List<FluxErrorsDto> FluxErrors { get; set; } = new();
        public List<FluxFinancialDataPointDto> FinancialDataPoints { get; set; } = new();
        public bool HasCustomFetching { get; set; }
        public bool HasCustomProcessing { get; set; }
    }

    public class TriggerPeriodDto
    {
        public int Id { get; set; }
        public FrequencyType FrequencyType { get; set; }
        public int? Interval { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class SourceProviderDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Reliability { get; set; }
    }

    public class FluxProcessingHistoryDto
    {
        public int Id { get; set; }
        public int FluxId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? Metadata { get; set; }
        public string CorrelationId { get; set; } = string.Empty;
        public List<FluxProcessingContentHistoryDto> FluxProcessingContentHistory { get; set; } = new();
    }

    public class FluxProcessingContentHistoryDto
    {
        public int Id { get; set; }
        public int FluxProcessingHistoryId { get; set; }
        public int FluxIdentificationContentHistoryId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public StatusProcess Status { get; set; }
    }

    public class FluxIdentificationHistoryDto
    {
        public int Id { get; set; }
        public int FluxId { get; set; }
        public string? ExternalDataId { get; set; }
        public string? Metadata { get; set; }
        public DateTime IdentifiedAt { get; set; }
        public List<FluxIdentificationContentHistoryDto> IdentificationContentHistories { get; set; } = new();
    }

    public class FluxIdentificationContentHistoryDto
    {
        public int Id { get; set; }
        public int FluxIdentificationHistoryId { get; set; }
        public StatusProcess ContentStatus { get; set; }
        public string ExternalContentId { get; set; } = string.Empty;
        public string ContentName { get; set; } = string.Empty;
        public string? RawId { get; set; }
        public DateTime IdentifiedAt { get; set; }
    }

    public class FluxErrorsDto
    {
        public int Id { get; set; }
        public string FluxErrorType { get; set; } = string.Empty;
        public int FluxId { get; set; }
        public string Message { get; set; } = string.Empty;
        public FluxActionType ActionType { get; set; }
        public string? Metadata { get; set; }
        public string? ExternalId { get; set; }
    }

    public class FluxFinancialDataPointDto
    {
        public int Id { get; set; }
        public int FluxId { get; set; }
        public FinancialDataPoint DataPoint { get; set; }
        public bool HasBeenApproved { get; set; } = false;
        public string MappingMetadata { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public ActionProviding Provider { get; set; }
        public FileDataMapping FluxMapping { get; set; }
    }

    public abstract class FluxMetadataDto
    {
        [JsonPropertyName("type")]
        public abstract string TypeDiscriminator { get; }
        public int FluxId { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class FluxMetadataMailDto : FluxMetadataDto
    {
        public override string TypeDiscriminator => nameof(FluxMetadataMailDto);

        public FluxMailContentLocation ContentLocation { get; set; }
        public FluxRuleSettingsDto FluxRuleSettings { get; set; } = new();
        public FluxAttachmentRuleDto FluxAttachmentRule { get; set; } = new();
    }

    public class FluxMetadataDownloadDto : FluxMetadataDto
    {
        public override string TypeDiscriminator => nameof(FluxMetadataDownloadDto);

        public string DownloadUrl { get; set; } = string.Empty;
        public ContentType ContentType { get; set; }
    }

    public class FluxMetadataApiDto : FluxMetadataDto
    {
        public override string TypeDiscriminator => nameof(FluxMetadataApiDto);
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }
    }

    public class FluxMetadataFileLocationDto : FluxMetadataDto
    {
        public override string TypeDiscriminator => nameof(FluxMetadataFileLocationDto);
        public FluxRuleSettingsDto? FluxRuleSettings { get; set; }
        public FluxAttachmentRuleDto? FluxAttachmentRule { get; set; }
    }

    public class FluxMetadataManualDto : FluxMetadataDto
    {
        public override string TypeDiscriminator => nameof(FluxMetadataManualDto);
        public ContentType ContentType { get; set; }
    }

    public class FluxRuleSettingsDto
    {
        public int Id { get; set; }
        public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
        public List<FluxMetadataCriterionDto> Criteria { get; set; } = new();
        public List<FluxRuleSettingsDto>? ChildRules { get; set; }
        public int? ParentRuleId { get; set; }
        public int FluxId { get; set; }
    }

    public class FluxMetadataCriterionDto
    {
        public int Id { get; set; }
        public KeyCriterion Key { get; set; }
        public string Value { get; set; } = string.Empty;
        public MetadataOperator Operator { get; set; } = MetadataOperator.Equal;
    }

    public class FluxAttachmentRuleDto
    {
        public int Id { get; set; }
        public bool ProcessAll { get; set; } = true;
        public List<FluxMetadataCriterionDto> Criteria { get; set; } = new();
        public int FluxId { get; set; }
    }

}
