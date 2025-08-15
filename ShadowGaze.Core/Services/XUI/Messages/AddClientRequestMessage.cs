using System.Net.Http.Json;
using ShadowGaze.Core.Models.XUI;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI.Messages;

public class AddClientRequestMessage(Xray xray, int inboundId, Guid guid, string email) : RequestMessage(xray)
{
    public override HttpRequestMessage BuildRequestMessage()
    {
        UriBuilder.Path += $"/panel/api/inbounds/addClient";
        var client = new ClientDto()
        {
            Id = guid.ToString(),
            Flow = "xtls-rprx-vision",
            Email = email,
            LimitIp = 0,
            TotalGB = 0,
            ExpiryTime = 0,
            Enable = true,
            TgId = "",
            SubId = "",
            Reset = 0,
        };
        var inboundSettings = new InboundEnvelopeSettings()
        {
            Clients = [client]
        };
        var body = new InboundEnvelope()
        {
            Id = inboundId,
            Settings = inboundSettings
        };
        
        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = UriBuilder.Uri,
            Content = JsonContent.Create(body)
        };
    }   
}