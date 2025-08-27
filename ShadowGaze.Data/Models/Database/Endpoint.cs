using System.ComponentModel.DataAnnotations.Schema;

namespace ShadowGaze.Data.Models.Database;

public class Endpoint : BaseDatabaseModel
{
    public int XrayId { get; set; }
    public Xray Xray { get; set; }
    public int InboundId { get; set; }
    public Guid ClientId { get; set; }
    public string ConnectionString { get; set; }
    [NotMapped]
    public DateTime ExpiryDate { get; set; }
}