using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class InboundSettings
{
    [JsonPropertyName("clients")]    public List<ClientDto> Clients { get; set; } = new();
    [JsonPropertyName("decryption")] public string Decryption { get; set; } = "none";
    [JsonPropertyName("fallbacks")]  public List<JsonElement> Fallbacks { get; set; } = new();
}