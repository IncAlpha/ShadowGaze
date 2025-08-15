using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI.Messages;

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