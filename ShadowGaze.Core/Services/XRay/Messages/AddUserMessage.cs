using Google.Protobuf;
using Xray.App.Proxyman;
using Xray.App.Proxyman.Command;
using Xray.Common.Protocol;
using Xray.Common.Serial;
using Xray.Proxy.Vless;
using Config = Xray.Proxy.Vless.Inbound.Config;

namespace ShadowGaze.Core.Services.XRay.Messages;

public class AddUserMessage(string tag, string email, Guid guid) : IXrayClientMessage
{
    public async Task SendMessage(HandlerService.HandlerServiceClient client)
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
                Value =  account.ToByteString()    
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
        await client.AlterInboundAsync(request);
    }
}

public class GetInbounds : IXrayClientMessage
{
    public async Task SendMessage(HandlerService.HandlerServiceClient client)
    {
        var inbounds = await client.ListInboundsAsync(new ListInboundsRequest());
        foreach (var inbound in inbounds.Inbounds.Where(i => i.Tag == "vless-in"))
        {
            var proxySettings = Config.Parser.ParseFrom(inbound.ProxySettings.Value);
            var receiverSettings = ReceiverConfig.Parser.ParseFrom(inbound.ReceiverSettings.Value);
            var reality = Xray.Transport.Internet.Reality.Config.Parser.ParseFrom(receiverSettings.StreamSettings.SecuritySettings[0].Value);
            Console.WriteLine($"Inbound: {inbound.Tag}");
        }
        
    }
}