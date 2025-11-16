namespace ShadowGaze.Data.Models.Database;

public class Customer : BaseDatabaseModel
{
    public string TelegramName { get; set; } = string.Empty;
    public long TelegramId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
    public Guid ClientId { get; set; }
}