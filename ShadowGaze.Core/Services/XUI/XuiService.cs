using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models.XUI;
using ShadowGaze.Core.Services.XUI.Messages;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI;

public class XuiService(ILogger<XuiService> logger, HttpClient httpClient, CookieContainer cookieContainer)
{
    public async Task<ApiResponse<InboundDto>> GetInbound(Xray xray, int inboundId)
    {
        var inboundRequest = new InboundRequestMessage(xray, inboundId).BuildRequestMessage();
        var inboundResponse = await DoRequest(xray, inboundRequest);
        return await inboundResponse.Content.ReadFromJsonAsync<ApiResponse<InboundDto>>();
    }

    public async Task<Guid?> AddClient(Xray xray, int inboundId, string username, DateTime expiry)
    {
        var guid = Guid.NewGuid();
        var createClientRequest = new AddClientRequestMessage(xray, inboundId, guid, username, expiry).BuildRequestMessage();
        var createClientResponse = await DoRequest(xray, createClientRequest);
        if (createClientResponse is null)
        {
            logger.LogError("Не получилось создать клиента в 3X-ui");
            return null;
        }
        createClientResponse.EnsureSuccessStatusCode();
        return guid;
    }

    private async Task<bool> Authentication(Xray xray)
    {
        var loginRequest = new LoginRequestMessage(xray).BuildRequestMessage();
        var response = await httpClient.SendAsync(loginRequest);
        response.EnsureSuccessStatusCode();
        var authResult = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        if (!authResult.Success)
        {
            logger.LogError($"Не удалось авторизоваться в панели 3X-ui: {authResult.Message}");
            return false;
        }

        logger.LogInformation("Авторизация в панели 3X-ui прошла успешно");
        return true;
    }

    private async Task<HttpResponseMessage> DoRequest(Xray xray, HttpRequestMessage request)
    {
        UriBuilder uriBuilder = new()
        {
            Scheme = Uri.UriSchemeHttps,
            Host = xray.Host,
            Port = xray.Port
        };

        var isAuth = HasAuthentication(uriBuilder.Uri);
        if (!isAuth)
        {
            isAuth = await Authentication(xray);
        }

        if (!isAuth)
        {
            return null;
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