namespace ShadowGaze.Core.Services.XRay;

public interface ICheckAlive
{
    Task<bool> CheckAliveAsync();
}