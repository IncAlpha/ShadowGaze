namespace ShadowGaze.Core.Services.XUI;

public interface IXuiClientFactory
{
    public IXuiApiClient GetClient(int xrayId);
}