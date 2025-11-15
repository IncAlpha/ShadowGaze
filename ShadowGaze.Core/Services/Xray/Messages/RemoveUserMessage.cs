using Google.Protobuf;
using Xray.App.Proxyman.Command;
using Xray.Common.Serial;

namespace ShadowGaze.Core.Services.Xray.Messages;

public class RemoveUserMessage(string tag, string email) : IXrayClientMessage
{
    public async Task SendMessage(HandlerService.HandlerServiceClient client, CancellationToken token)
    {
        var operation = new RemoveUserOperation
        {
            Email = email
        };
        var request = new AlterInboundRequest
        {
            Tag = tag,
            Operation = new TypedMessage
            {
                Type = "xray.app.proxyman.command.RemoveUserOperation",
                Value = operation.ToByteString()
            }
        };
        await client.AlterInboundAsync(request, cancellationToken: token);
    }
}