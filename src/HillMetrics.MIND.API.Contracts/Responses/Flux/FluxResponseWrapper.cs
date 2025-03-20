using HillMetrics.Core.Financial;
using HillMetrics.Core.Time.Trigger;
using HillMetrics.MIND.API.Contracts.Responses.Source;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using System;
using System.Collections.Generic;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux;

public class FluxResponseWrapper
{
    public int Id { get; set; }
    public string FluxName { get; set; }
    public string Description { get; set; }
    public string Comment { get; set; }
    public SourceProviderDto Source { get; set; }
    public FluxType FluxType { get; set; }
    public FinancialType? FinancialType { get; set; }
    public FluxState FluxState { get; set; }
    public TriggerPeriodDto FetchTriggerPeriod { get; set; }
    public TriggerPeriodDto ProcessTriggerPeriod { get; set; }
    public bool CanHaveConcurrencyMultiFetching { get; set; }
    
    // Concrete metadata properties - no abstract classes/interfaces
    public FluxMetadataMailDto EmailMetadata { get; set; }
    public FluxMetadataApiDto ApiMetadata { get; set; }
    public FluxMetadataDownloadDto DownloadMetadata { get; set; }
    public FluxMetadataFileLocationDto FileLocationMetadata { get; set; }
    
    // Other properties from FluxResponse
    public List<FluxIdentificationHistoryDto> FluxIdentificationHistory { get; set; } = new();
    public List<FluxProcessingHistoryDto> FluxProcessingHistory { get; set; } = new();
    
    public FluxResponseWrapper() { }
    
    public FluxResponseWrapper(FluxResponse response)
    {
        // Copy base properties
        Id = response.Id;
        FluxName = response.FluxName;
        Description = response.Description;
        Comment = response.Comment;
        Source = response.Source;
        FluxType = response.FluxType;
        FinancialType = response.FinancialType;
        FluxState = response.FluxState;
        FetchTriggerPeriod = response.FetchTriggerPeriod;
        ProcessTriggerPeriod = response.ProcessTriggerPeriod;
        CanHaveConcurrencyMultiFetching = response.CanHaveConcurrencyMultiFetching;
        FluxIdentificationHistory = response.FluxIdentificationHistory;
        FluxProcessingHistory = response.FluxProcessingHistory;
        
        // Copy metadata to the appropriate concrete property
        if (response.FluxMetadata != null)
        {
            if (response.FluxMetadata is FluxMetadataMailDto mailMetadata)
                EmailMetadata = mailMetadata;
            else if (response.FluxMetadata is FluxMetadataApiDto apiMetadata)
                ApiMetadata = apiMetadata;
            else if (response.FluxMetadata is FluxMetadataDownloadDto downloadMetadata)
                DownloadMetadata = downloadMetadata;
            else if (response.FluxMetadata is FluxMetadataFileLocationDto fileLocationMetadata)
                FileLocationMetadata = fileLocationMetadata;
        }
    }
} 