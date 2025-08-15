using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Core.Services.XUI.Messages;

public abstract class RequestMessage(Xray xray) : IRequestMessage
{
    protected UriBuilder UriBuilder = new()
    {
        Scheme = Uri.UriSchemeHttps,
        Host = xray.Host,
        Port = xray.Port,
        Path = xray.Path
    };

    public abstract HttpRequestMessage BuildRequestMessage();
}