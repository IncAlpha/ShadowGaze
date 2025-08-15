using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class InboundDto
{
    [JsonPropertyName("id")]          public int    Id { get; set; }
    [JsonPropertyName("up")]          public long   Up { get; set; }
    [JsonPropertyName("down")]        public long   Down { get; set; }
    [JsonPropertyName("total")]       public long   Total { get; set; }
    [JsonPropertyName("remark")]      public string Remark { get; set; } = "";
    [JsonPropertyName("enable")]      public bool   Enable { get; set; }
    [JsonPropertyName("expiryTime")]  public long   ExpiryTime { get; set; }
    [JsonPropertyName("clientStats")] public JsonElement ClientStats { get; set; } // в JSON = null → ValueKind = Null
    [JsonPropertyName("listen")]      public string Listen { get; set; } = "";
    [JsonPropertyName("port")]        public int    Port { get; set; }
    [JsonPropertyName("protocol")]    public string Protocol { get; set; } = "";
    [JsonPropertyName("tag")]         public string Tag { get; set; } = "";

    // ---------- settings ----------
    [JsonIgnore] public InboundSettings Settings { get; set; } = new();
    [JsonPropertyName("settings")]
    public string SettingsRaw
    {
        get => JsonSerializer.Serialize(Settings, InnerJsonOptions);
        set => Settings = string.IsNullOrWhiteSpace(value)
            ? new InboundSettings()
            : (JsonSerializer.Deserialize<InboundSettings>(value, InnerJsonOptions) ?? new InboundSettings());
    }

    // ---------- streamSettings ----------
    [JsonIgnore] public StreamSettings StreamSettings { get; set; } = new();
    [JsonPropertyName("streamSettings")]
    public string StreamSettingsRaw
    {
        get => JsonSerializer.Serialize(StreamSettings, InnerJsonOptions);
        set => StreamSettings = string.IsNullOrWhiteSpace(value)
            ? new StreamSettings()
            : (JsonSerializer.Deserialize<StreamSettings>(value, InnerJsonOptions) ?? new StreamSettings());
    }

    // ---------- sniffing ----------
    [JsonIgnore] public SniffingSettings Sniffing { get; set; } = new();
    [JsonPropertyName("sniffing")]
    public string SniffingRaw
    {
        get => JsonSerializer.Serialize(Sniffing, InnerJsonOptions);
        set => Sniffing = string.IsNullOrWhiteSpace(value)
            ? new SniffingSettings()
            : (JsonSerializer.Deserialize<SniffingSettings>(value, InnerJsonOptions) ?? new SniffingSettings());
    }

    // ---------- allocate ----------
    [JsonIgnore] public AllocateSettings Allocate { get; set; } = new();
    [JsonPropertyName("allocate")]
    public string AllocateRaw
    {
        get => JsonSerializer.Serialize(Allocate, InnerJsonOptions);
        set => Allocate = string.IsNullOrWhiteSpace(value)
            ? new AllocateSettings()
            : (JsonSerializer.Deserialize<AllocateSettings>(value, InnerJsonOptions) ?? new AllocateSettings());
    }

    private static readonly JsonSerializerOptions InnerJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };
}