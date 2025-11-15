using ShadowGaze.Core.Models.Xray.Messages;
using Xray.App.Proxyman.Command;
using Xray.Proxy.Vless;

namespace ShadowGaze.Core.Services.Xray.Messages;

public class GetInboundUsersMessage(string tag) : IXrayClientMessage<List<XrayUser>>
{
    public async Task<List<XrayUser>> GetData(HandlerService.HandlerServiceClient client, CancellationToken token)
    {
        var response = await client.GetInboundUsersAsync(new GetInboundUserRequest { Tag = tag },
            cancellationToken: token);
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