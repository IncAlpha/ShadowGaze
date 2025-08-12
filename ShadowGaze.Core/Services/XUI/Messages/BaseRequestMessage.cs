using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI.Messages;

public abstract class RequestMessage(Xray xray) : IRequestMessage
{
    protected UriBuilder UriBuilder = new()
    {
        Scheme = Uri.UriSchemeHttps,
        Host = xray.Host,
        Port = xray.Port,
        Path = "/TdUQOleIawWR5DpT91"
    };

    public abstract HttpRequestMessage BuildRequestMessage();
}

public interface IRequestMessage
{
    HttpRequestMessage BuildRequestMessage();
}

public class LoginRequestMessage(Xray xray) : RequestMessage(xray)
{
    public override HttpRequestMessage BuildRequestMessage()
    {
        var body = new List<KeyValuePair<string, string>>
        {
            new("username", xray.Username),
            new("password", xray.Password)
        };
        UriBuilder.Path += "/login";
        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = UriBuilder.Uri,
            Content = new FormUrlEncodedContent(body)
        };
    }
}

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
