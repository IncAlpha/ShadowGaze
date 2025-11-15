using Xray.App.Proxyman.Command;

namespace ShadowGaze.Core.Services.XRay.Messages;

public interface IXrayClientMessage
{
    public Task SendMessage(HandlerService.HandlerServiceClient client);
}

public interface IXrayClientMessage<TOut>
{
    public Task<TOut> GetData(HandlerService.HandlerServiceClient client);
}