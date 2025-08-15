using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class SniffingSettings
{
    [JsonPropertyName("enabled")]      public bool Enabled { get; set; }
    [JsonPropertyName("destOverride")] public List<string> DestOverride { get; set; } = new();
    [JsonPropertyName("metadataOnly")] public bool MetadataOnly { get; set; }
    [JsonPropertyName("routeOnly")]    public bool RouteOnly { get; set; }
}