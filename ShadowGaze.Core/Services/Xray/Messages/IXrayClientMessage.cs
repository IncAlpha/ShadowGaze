using Xray.App.Proxyman.Command;

namespace ShadowGaze.Core.Services.Xray.Messages;

public interface IXrayClientMessage
{
    public Task SendMessage(HandlerService.HandlerServiceClient client, CancellationToken token);
}

public interface IXrayClientMessage<TOut>
{
    public Task<TOut> GetData(HandlerService.HandlerServiceClient client, CancellationToken token);
}