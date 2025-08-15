using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI.Messages;

public class InboundRequestMessage(Xray xray, int inboundId) : RequestMessage(xray)
{ 
    public override HttpRequestMessage BuildRequestMessage()
    {
        UriBuilder.Path += $"/panel/api/inbounds/get/{inboundId}";
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = UriBuilder.Uri
        };
    }
}