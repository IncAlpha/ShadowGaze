using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class ClientDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    [JsonPropertyName("flow")]
    public string Flow { get; set; } = "";
    [JsonPropertyName("email")]
    public string Email { get; set; } = "";
    [JsonPropertyName("limitIp")]
    public int LimitIp { get; set; }
    [JsonPropertyName("totalGB")]
    public long TotalGB { get; set; }     // байты
    [JsonPropertyName("expiryTime")]
    public long ExpiryTime { get; set; }  // unix seconds
    [JsonPropertyName("enable")]
    public bool Enable { get; set; }
    [JsonPropertyName("reset")]
    public int Reset { get; set; }
}

public sealed class Client
{
    public Guid Id { get; init; }
    public DateTime ExpiryTime { get; set; }
}