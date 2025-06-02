using HillMetrics.MIND.API.Contracts.Responses.Flux;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace HillMetrics.MIND.API.Contracts.Converter;

public class FluxMetadataDtoJsonConverter : JsonConverter<FluxMetadataDto>
{
    public override FluxMetadataDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDocument = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDocument.RootElement;

            // Read the discriminator field "type"
            if (!root.TryGetProperty("type", out var typeProperty))
                throw new JsonException("The 'type' property is required for polymorphic deserialization.");

            var typeValue = typeProperty.GetString();
            if (typeValue == null)
                throw new JsonException("The 'type' property value is null or invalid.");

            return typeValue switch
            {
                nameof(FluxMetadataMailDto) => JsonSerializer.Deserialize<FluxMetadataMailDto>(root.GetRawText(), options),
                nameof(FluxMetadataDownloadDto) => JsonSerializer.Deserialize<FluxMetadataDownloadDto>(root.GetRawText(), options),
                nameof(FluxMetadataApiDto) => JsonSerializer.Deserialize<FluxMetadataApiDto>(root.GetRawText(), options),
                nameof(FluxMetadataFileLocationDto) => JsonSerializer.Deserialize<FluxMetadataFileLocationDto>(root.GetRawText(), options),
                nameof(FluxMetadataManualDto) => JsonSerializer.Deserialize<FluxMetadataManualDto>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown flux metadata type: {typeValue}")
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, FluxMetadataDto value, JsonSerializerOptions options)
    {
        // Make sure we include the type discriminator
        var actualType = value.GetType();
        var tempOptions = new JsonSerializerOptions(options);
        
        // Create a copy without this converter to avoid infinite recursion
        tempOptions.Converters.Clear();
        foreach (var converter in options.Converters)
        {
            if (converter is not FluxMetadataDtoJsonConverter)
                tempOptions.Converters.Add(converter);
        }
        
        JsonSerializer.Serialize(writer, (object)value, actualType, tempOptions);
    }
}