using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class InboundEnvelope
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonIgnore]
    public InboundEnvelopeSettings Settings { get; set; } = new();

    // Хранит/читает поле "settings" как строку, но вы работаете с типом InboundSettings
    [JsonPropertyName("settings")]
    public string SettingsJson
    {
        get => JsonSerializer.Serialize(Settings, InnerJsonOptions);
        set
        {
            Settings = string.IsNullOrWhiteSpace(value)
                ? new InboundEnvelopeSettings()
                : (JsonSerializer.Deserialize<InboundEnvelopeSettings>(value, InnerJsonOptions) ?? new InboundEnvelopeSettings());
        }
    }

    private static readonly JsonSerializerOptions InnerJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };
}