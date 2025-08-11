namespace ShadowGaze.Data.Models.Database;

public class Customer
{
    public int Id { get; set; }
    public string TelegramName { get; set; } = string.Empty;
    public double Balance { get; set; }
    public int EndpointId { get; set; }
    public Endpoint? Endpoint { get; set; }
}