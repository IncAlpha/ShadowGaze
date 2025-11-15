using ShadowGaze.Core.Services.Xray.Messages;

namespace ShadowGaze.Core.Services.Xray;

public interface IXrayClient
{
    public Task SendMessage(IXrayClientMessage message, CancellationToken token);
    public Task<T> GetData<T>(IXrayClientMessage<T> message, CancellationToken token);
}