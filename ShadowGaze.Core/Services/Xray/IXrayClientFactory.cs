namespace ShadowGaze.Core.Services.Xray;

public interface IXrayClientFactory
{
    public IXrayClient GetClient(Uri uri);
}