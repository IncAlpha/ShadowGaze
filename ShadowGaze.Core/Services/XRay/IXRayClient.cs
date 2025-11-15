using ShadowGaze.Core.Services.XRay.Messages;

namespace ShadowGaze.Core.Services.XRay;

public interface IXRayClient
{
    public Task SendMessage(IXrayClientMessage message);
    public Task<T> GetData<T>(IXrayClientMessage<T> message);
}