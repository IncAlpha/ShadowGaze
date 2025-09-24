namespace ShadowGaze.Core.Services.XUI.Messages;

public class InboundRequestMessage(int inboundId)
    : HttpRequestMessage(HttpMethod.Get, $"panel/api/inbounds/get/{inboundId}");