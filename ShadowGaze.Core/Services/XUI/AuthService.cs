using System.Net;
using System.Net.Http.Json;
using ShadowGaze.Core.Models.XUI;
using ShadowGaze.Core.Services.XUI.Messages;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI;

public class AuthService(HttpClient httpClient)
{
    private readonly CookieContainer _cookieContainer = new();

    public async Task LoginAsync(Xray xray)
    {
        var loginRequest = new LoginRequestMessage(xray).BuildRequestMessage();
        var response = await httpClient.SendAsync(loginRequest);
        response.EnsureSuccessStatusCode();
        
        var inboundRequest = new InboundRequestMessage(xray, 1).BuildRequestMessage();
        var inboundResponse = await httpClient.SendAsync(inboundRequest);
        inboundResponse.EnsureSuccessStatusCode();
        var inbound = await inboundResponse.Content.ReadFromJsonAsync<ApiResponse<InboundDto>>();
        Console.WriteLine($"Вижу клиентов в количестве: {inbound.Obj.Settings.Clients.Count}");
    }
}