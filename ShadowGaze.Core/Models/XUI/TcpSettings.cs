using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class TcpSettings
{
    [JsonPropertyName("acceptProxyProtocol")] public bool AcceptProxyProtocol { get; set; }
    [JsonPropertyName("header")]              public TcpHeader Header { get; set; } = new();
}