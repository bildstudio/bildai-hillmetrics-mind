using HillMetrics.MIND.API.Contracts.Responses.Flux;
using System.Text.Json.Serialization;

namespace HillMetrics.MIND.API.Contracts.Requests.Flux
{
    /// <summary>
    /// Base class for flux metadata
    /// </summary>
    public interface IFluxDiscriminator
    {
        /// <summary>
        /// Type discriminator for deserialization
        /// </summary>
        [JsonPropertyName("$type")]
        public string? TypeDiscriminator { get; }
    }

    /// <summary>
    /// Metadata for Email flux type
    /// </summary>
    public class FluxMetadataMailRequestDto : FluxMetadataMailDto, IFluxDiscriminator
    {
        public string? TypeDiscriminator => nameof(FluxMetadataMailDto);
    }

    /// <summary>
    /// Metadata for API flux type
    /// </summary>
    public class FluxMetadataApiRequestDto : FluxMetadataApiDto, IFluxDiscriminator
    {
        public string? TypeDiscriminator => nameof(FluxMetadataApiDto);
    }

    /// <summary>
    /// Metadata for HTTP Download and Scraping flux types
    /// </summary>
    public class FluxMetadataDownloadRequestDto : FluxMetadataDownloadDto, IFluxDiscriminator
    {
        public string? TypeDiscriminator => nameof(FluxMetadataDownloadDto);
    }

    /// <summary>
    /// Metadata for SFTP flux type
    /// </summary>
    public class FluxMetadataFileLocationRequestDto : FluxMetadataFileLocationDto, IFluxDiscriminator
    {
        public string? TypeDiscriminator => nameof(FluxMetadataFileLocationDto);
    }
} 