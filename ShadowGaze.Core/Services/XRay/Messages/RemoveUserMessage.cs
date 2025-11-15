using Google.Protobuf;
using Xray.App.Proxyman.Command;
using Xray.Common.Serial;
using Xray.Proxy.Vless;

namespace ShadowGaze.Core.Services.XRay.Messages;

public class RemoveUserMessage(string tag, string email) : IXrayClientMessage
{
    public async Task SendMessage(HandlerService.HandlerServiceClient client)
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
        await client.AlterInboundAsync(request);
    }
}

public class GetInboundUsersMessage(string tag) : IXrayClientMessage<List<XrayUser>>
{
    public async Task<List<XrayUser>> GetData(HandlerService.HandlerServiceClient client)
    {
        var response = await client.GetInboundUsersAsync(new GetInboundUserRequest { Tag = tag });
        return response.Users.Select(u =>
        {
            var account = Account.Parser.ParseFrom(u.Account.Value);
            return new XrayUser
            {
                Email = u.Email,
                Id = Guid.Parse(account.Id),
                Flow = account.Flow
            };
        }).ToList();
    }
}

public class XrayUser
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string Flow { get; set; } = string.Empty;
}