namespace ShadowGaze.Data.Models.Database;

public class Connection : BaseDatabaseModel
{
    public int CustomerId { get; set; }
    public int VlessInboundId { get; set; }
    public Guid ClientId { get; set; }
    public string Email { get; set; }
    public string ConnectionString { get; set; } = String.Empty;
}