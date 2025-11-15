namespace ShadowGaze.Core.Services.Xray;

public interface ICheckAlive
{
    Task<bool> CheckAliveAsync();
}