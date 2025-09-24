namespace ShadowGaze.Core.Services.XUI.Messages;
public class LoginRequestMessage : HttpRequestMessage
{
    public LoginRequestMessage(string username, string password): base(HttpMethod.Post, "login")
    {
        var body = new List<KeyValuePair<string, string>>
        {
            new("username", username),
            new("password", password)
        };
        Content = new FormUrlEncodedContent(body);
    }
}
