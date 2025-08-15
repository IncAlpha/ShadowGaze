namespace ShadowGaze.Core.Services.XUI.Messages;

public interface IRequestMessage
{
    HttpRequestMessage BuildRequestMessage();
}