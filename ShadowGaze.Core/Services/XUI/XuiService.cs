using System.Net;
using System.Net.Http.Json;
using ShadowGaze.Core.Models.XUI;
using ShadowGaze.Core.Services.XUI.Messages;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI;
public class XuiService(HttpClient httpClient, CookieContainer cookieContainer)
{
    public async Task<ApiResponse<InboundDto>> GetInbound(Xray xray, int inboundId)
    {
        var inboundRequest = new InboundRequestMessage(xray, inboundId).BuildRequestMessage();
        var inboundResponse = await DoRequest(xray, inboundRequest);
        return await inboundResponse.Content.ReadFromJsonAsync<ApiResponse<InboundDto>>();
    }

    public async Task<Guid> AddClient(Xray xray, int inboundId, string username)
    {
        var guid = Guid.NewGuid();
        var createClientRequest = new AddClientRequestMessage(xray, inboundId, guid, username).BuildRequestMessage();
        var createClientResponse = await DoRequest(xray, createClientRequest);
        createClientResponse.EnsureSuccessStatusCode();
        return guid;
    }
    
    private async Task Authentication(Xray xray)
    {
        var loginRequest = new LoginRequestMessage(xray).BuildRequestMessage();
        var response = await httpClient.SendAsync(loginRequest);
        response.EnsureSuccessStatusCode();    
    }

    private async Task<HttpResponseMessage> DoRequest(Xray xray, HttpRequestMessage request)
    {
        UriBuilder uriBuilder = new()
        {
            Scheme = Uri.UriSchemeHttps,
            Host = xray.Host,
            Port = xray.Port,
        };
        if (!HasAuthentication(uriBuilder.Uri))
        {
            await Authentication(xray);
        }
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }

    private bool HasAuthentication(Uri uri)
    {
        var cookieCollection = cookieContainer.GetCookies(uri);
        var cookie = cookieCollection.FirstOrDefault(c => c.Name == "3x-ui");
        return !(cookie?.Expired ?? true);
    }
}