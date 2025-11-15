namespace ShadowGaze.Core.Models.Xray.Messages;

public class XrayUser
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string Flow { get; set; } = string.Empty;
}