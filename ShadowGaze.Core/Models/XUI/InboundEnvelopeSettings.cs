using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class InboundEnvelopeSettings
{
    [JsonPropertyName("clients")]
    public List<ClientDto> Clients { get; set; } = new();
}