using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models.XUI;
using ShadowGaze.Core.Services.XUI.Messages;

namespace ShadowGaze.Core.Services.XUI;

public class XuiApiClient(
    ILogger<XuiApiClient> logger,
    HttpClient httpClient,
    CookieContainer cookieContainer,
    Data.Models.Database.Xray xray) : IXuiApiClient
{
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        var isAuth = HasAuthentication();
        if (!isAuth)
        {
            isAuth = await Authentication();
        }

        if (!isAuth)
        {
            return null;
        }

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }
    
    private async Task<bool> Authentication()
    {
        var response = await httpClient.SendAsync(new LoginRequestMessage(xray.Username, xray.Password));
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
    
    private bool HasAuthentication()
    {
        var cookieCollection = cookieContainer.GetCookies(httpClient.BaseAddress!);
        var cookie = cookieCollection.FirstOrDefault(c => c.Name == "3x-ui");
        return !(cookie?.Expired ?? true);
    }
}