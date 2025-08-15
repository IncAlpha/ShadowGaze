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