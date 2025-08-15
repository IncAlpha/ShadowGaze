using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class RealityTlsSettings
{
    [JsonPropertyName("publicKey")]   public string PublicKey { get; set; } = "";
    [JsonPropertyName("fingerprint")] public string Fingerprint { get; set; } = "";
    [JsonPropertyName("serverName")]  public string ServerName { get; set; } = "";
    [JsonPropertyName("spiderX")]     public string SpiderX { get; set; } = "/";
}