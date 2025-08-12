using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class ApiResponse<T>
{
    [JsonPropertyName("success")] 
    public bool Success { get; set; }
    [JsonPropertyName("msg")]     
    public string? Msg { get; set; }
    [JsonPropertyName("obj")]     
    public T? Obj { get; set; }
}


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

public sealed class InboundSettings
{
    [JsonPropertyName("clients")]    public List<ClientDto> Clients { get; set; } = new();
    [JsonPropertyName("decryption")] public string Decryption { get; set; } = "none";
    [JsonPropertyName("fallbacks")]  public List<JsonElement> Fallbacks { get; set; } = new();
}

public sealed class StreamSettings
{
    [JsonPropertyName("network")]       public string Network { get; set; } = "tcp";
    [JsonPropertyName("security")]      public string Security { get; set; } = "";
    [JsonPropertyName("externalProxy")] public List<JsonElement> ExternalProxy { get; set; } = new();
    [JsonPropertyName("realitySettings")] public RealitySettings RealitySettings { get; set; } = new();
    [JsonPropertyName("tcpSettings")]     public TcpSettings TcpSettings { get; set; } = new();
}

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

public sealed class RealityTlsSettings
{
    [JsonPropertyName("publicKey")]   public string PublicKey { get; set; } = "";
    [JsonPropertyName("fingerprint")] public string Fingerprint { get; set; } = "";
    [JsonPropertyName("serverName")]  public string ServerName { get; set; } = "";
    [JsonPropertyName("spiderX")]     public string SpiderX { get; set; } = "/";
}

public sealed class TcpSettings
{
    [JsonPropertyName("acceptProxyProtocol")] public bool AcceptProxyProtocol { get; set; }
    [JsonPropertyName("header")]              public TcpHeader Header { get; set; } = new();
}

public sealed class TcpHeader
{
    [JsonPropertyName("type")] public string Type { get; set; } = "none";
}

public sealed class SniffingSettings
{
    [JsonPropertyName("enabled")]      public bool Enabled { get; set; }
    [JsonPropertyName("destOverride")] public List<string> DestOverride { get; set; } = new();
    [JsonPropertyName("metadataOnly")] public bool MetadataOnly { get; set; }
    [JsonPropertyName("routeOnly")]    public bool RouteOnly { get; set; }
}

public sealed class AllocateSettings
{
    [JsonPropertyName("strategy")]    public string Strategy { get; set; } = "always";
    [JsonPropertyName("refresh")]     public int    Refresh { get; set; }
    [JsonPropertyName("concurrency")] public int    Concurrency { get; set; }
}