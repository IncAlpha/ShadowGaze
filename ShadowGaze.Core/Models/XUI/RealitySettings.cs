using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class RealitySettings
{
    [JsonPropertyName("show")]        public bool   Show { get; set; }
    [JsonPropertyName("xver")]        public int    Xver { get; set; }
    [JsonPropertyName("dest")]        public string Dest { get; set; } = "";
    [JsonPropertyName("serverNames")] public List<string> ServerNames { get; set; } = new();
    [JsonPropertyName("privateKey")]  public string PrivateKey { get; set; } = "";
    [JsonPropertyName("minClient")]   public string MinClient { get; set; } = "";
    [JsonPropertyName("maxClient")]   public string MaxClient { get; set; } = "";
    [JsonPropertyName("maxTimediff")] public int    MaxTimediff { get; set; }
    [JsonPropertyName("shortIds")]    public List<string> ShortIds { get; set; } = new();
    [JsonPropertyName("settings")]    public RealityTlsSettings Settings { get; set; } = new();
}