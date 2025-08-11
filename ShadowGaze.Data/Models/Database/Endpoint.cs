namespace ShadowGaze.Data.Models.Database;

public class Endpoint
{
    public int Id { get; set; }
    public int XrayId { get; set; }
    public Xray? Xray { get; set; }
    public int InboundId { get; set; }
    public Guid ClientId { get; set; }
}