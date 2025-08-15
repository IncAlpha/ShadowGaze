using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class AllocateSettings
{
    [JsonPropertyName("strategy")]    public string Strategy { get; set; } = "always";
    [JsonPropertyName("refresh")]     public int    Refresh { get; set; }
    [JsonPropertyName("concurrency")] public int    Concurrency { get; set; }
}