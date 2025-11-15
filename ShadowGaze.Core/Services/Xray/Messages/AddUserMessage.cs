using Google.Protobuf;
using Xray.App.Proxyman.Command;
using Xray.Common.Protocol;
using Xray.Common.Serial;
using Xray.Proxy.Vless;

namespace ShadowGaze.Core.Services.Xray.Messages;

public class AddUserMessage(string tag, string email, Guid guid) : IXrayClientMessage
{
    public async Task SendMessage(HandlerService.HandlerServiceClient client, CancellationToken token)
    {
        var account = new Account
        {
            Id = guid.ToString(),
            Flow = "xtls-rprx-vision",
        };

        var localEmail = email;
        if (string.IsNullOrWhiteSpace(email))
        {
            localEmail = guid.ToString();
        }

        var user = new User
        {
            Email = localEmail,
            Level = 0,
            Account = new TypedMessage
            {
                Type = "xray.proxy.vless.Account",
                Value = account.ToByteString()
            }
        };

        var operation = new AddUserOperation()
        {
            User = user
        };
        var request = new AlterInboundRequest
        {
            Tag = tag,
            Operation = new TypedMessage
            {
                Type = "xray.app.proxyman.command.AddUserOperation",
                Value = operation.ToByteString()
            }
        };
        await client.AlterInboundAsync(request, cancellationToken: token);
    }
}