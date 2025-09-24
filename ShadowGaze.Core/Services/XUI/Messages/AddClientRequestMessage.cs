using System.Net.Http.Json;
using ShadowGaze.Core.Models.XUI;

namespace ShadowGaze.Core.Services.XUI.Messages;

public class AddClientRequestMessage : HttpRequestMessage
{
    public AddClientRequestMessage(int inboundId, ClientDto client): base(HttpMethod.Post, "panel/api/inbounds/addClient")
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