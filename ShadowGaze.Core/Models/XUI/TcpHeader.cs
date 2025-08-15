using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class TcpHeader
{
    [JsonPropertyName("type")] public string Type { get; set; } = "none";
}