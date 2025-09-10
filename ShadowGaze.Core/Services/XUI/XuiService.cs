using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models.XUI;
using ShadowGaze.Core.Services.XUI.Messages;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI;

public class XuiService(ILogger<XuiService> logger, IXuiClientFactory factory)
{
    public async Task<ApiResponse<InboundDto>> GetInbound(int xrayId, int inboundId)
    {
        var client = factory.GetClient(xrayId);
        var inboundRequest = new InboundRequestMessage(inboundId);
        var inboundResponse = await client.SendAsync(inboundRequest);
        return await inboundResponse.Content.ReadFromJsonAsync<ApiResponse<InboundDto>>();
    }
    
    public async Task<Client> AddDefaultClient(int xrayId, int inboundId, string username)
    {
        var guid = Guid.NewGuid();
        var expiryDate = DateTime.Now.AddDays(20);
        var clientDto = new ClientDto()
        {
            Id = guid.ToString(),
            Flow = "xtls-rprx-vision",
            Email = username,
            LimitIp = 0,
            TotalGB = 0,
            ExpiryTime = new DateTimeOffset(expiryDate).ToUnixTimeMilliseconds(),
            Enable = true,
            TgId = "",
            SubId = "",
            Reset = 0,
        };
        
        var createClientRequest = new AddClientRequestMessage(inboundId, clientDto);
        var client = factory.GetClient(xrayId);
        var createClientResponse = await client.SendAsync(createClientRequest);
        
        if (createClientResponse is null)
        {
            logger.LogError("Не получилось создать клиента в 3X-ui");
            return null;
        }
        createClientResponse.EnsureSuccessStatusCode();
        return new Client
        {
            Id = guid,
            ExpiryTime = expiryDate
        };
    }

    public async Task<ClientDto> GetClientAsync(int xrayId, int inboundId, Guid guid)
    {
        var inboundResponse = await GetInbound(xrayId, inboundId);
        var clients = inboundResponse.Object.Settings.Clients;
        return clients.FirstOrDefault(x => x.Id == guid.ToString());
    }

    public async Task UpdateClientAsync(int xrayId, int inboundId, ClientDto xuiClient)
    {
        var createClientRequest = new UpdateClientRequestMessage(inboundId, xuiClient);
        var client = factory.GetClient(xrayId);
        var createClientResponse = await client.SendAsync(createClientRequest);
        
        if (createClientResponse is null)
        {
            logger.LogError("Не получилось обновить клиента в 3X-ui");
            return;
        }
        createClientResponse.EnsureSuccessStatusCode();
    }
}


