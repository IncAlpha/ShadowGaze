namespace ShadowGaze.Core.Services.XUI;

public interface IXuiApiClient
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}