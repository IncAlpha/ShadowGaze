namespace ShadowGaze.Data.Models.Database;

public class Connection : BaseDatabaseModel
{
    public int CustomerId { get; set; }
    public int VlessInboundId { get; set; }
    public Guid ClientId { get; set; }
    public string ConnectionString { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
}