using System.Net.Http.Json;
using ShadowGaze.Core.Models.XUI;

namespace ShadowGaze.Core.Services.XUI.Messages;

public class UpdateClientRequestMessage : HttpRequestMessage
{
    public UpdateClientRequestMessage(int inboundId, ClientDto client): base(HttpMethod.Post, $"/panel/api/inbounds/updateClient/{client.Id}")
    {
        var inboundSettings = new InboundEnvelopeSettings()
        {
            Clients = [client]
        };
        var body = new InboundEnvelope()
        {
            Id = inboundId,
            Settings = inboundSettings
        };
        Content = JsonContent.Create(body);
    }
}