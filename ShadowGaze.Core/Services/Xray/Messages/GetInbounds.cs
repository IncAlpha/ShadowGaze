using Xray.App.Proxyman;
using Xray.App.Proxyman.Command;
using ProxyConfig = Xray.Proxy.Vless.Inbound.Config;
using RealityConfig = Xray.Transport.Internet.Reality.Config;


namespace ShadowGaze.Core.Services.Xray.Messages;

public class GetInbounds : IXrayClientMessage
{
    public async Task SendMessage(HandlerService.HandlerServiceClient client, CancellationToken token)
    {
        var inbounds = await client.ListInboundsAsync(new ListInboundsRequest(), cancellationToken: token);
        foreach (var inbound in inbounds.Inbounds.Where(i => i.Tag == "vless-in"))
        {
            var proxySettings = ProxyConfig.Parser.ParseFrom(inbound.ProxySettings.Value);
            var receiverSettings = ReceiverConfig.Parser.ParseFrom(inbound.ReceiverSettings.Value);
            var reality = RealityConfig.Parser.ParseFrom(receiverSettings.StreamSettings.SecuritySettings[0].Value);
            Console.WriteLine($"Inbound: {inbound.Tag}");
        }
    }
}