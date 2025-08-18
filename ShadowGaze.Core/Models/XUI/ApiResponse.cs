using System.Text.Json.Serialization;

namespace ShadowGaze.Core.Models.XUI;

public sealed class ApiResponse<T>
{
    [JsonPropertyName("success")] 
    public bool Success { get; set; }
    [JsonPropertyName("msg")]     
    public string Message { get; set; }
    [JsonPropertyName("obj")]     
    public T Object { get; set; }
}