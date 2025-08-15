using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class StreamSettings
{
    [JsonPropertyName("network")]       public string Network { get; set; } = "tcp";
    [JsonPropertyName("security")]      public string Security { get; set; } = "";
    [JsonPropertyName("externalProxy")] public List<JsonElement> ExternalProxy { get; set; } = new();
    [JsonPropertyName("realitySettings")] public RealitySettings RealitySettings { get; set; } = new();
    [JsonPropertyName("tcpSettings")]     public TcpSettings TcpSettings { get; set; } = new();
}