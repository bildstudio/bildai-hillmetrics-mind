using HillMetrics.MIND.API.Contracts.Responses.Flux;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace HillMetrics.MIND.API.Converter;

public class FluxMetadataDtoJsonConverter : JsonConverter<FluxMetadataDto>
{
    public override FluxMetadataDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDocument = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDocument.RootElement;

            // Lire le champ discriminant "type"
            var typeProperty = root.GetProperty("type").GetString();
            if (typeProperty == null)
                throw new JsonException("The 'type' property is required for polymorphic deserialization.");

            return typeProperty switch
            {
                nameof(FluxMetadataMailDto) => JsonSerializer.Deserialize<FluxMetadataMailDto>(root.GetRawText(), options),
                nameof(FluxMetadataDownloadDto) => JsonSerializer.Deserialize<FluxMetadataDownloadDto>(root.GetRawText(), options),
                nameof(FluxMetadataApiDto) => JsonSerializer.Deserialize<FluxMetadataApiDto>(root.GetRawText(), options),
                nameof(FluxMetadataFileLocationDto) => JsonSerializer.Deserialize<FluxMetadataFileLocationDto>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown type: {typeProperty}")
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, FluxMetadataDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}