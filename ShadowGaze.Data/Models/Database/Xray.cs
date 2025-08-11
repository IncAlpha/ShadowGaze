namespace ShadowGaze.Data.Models.Database;

public class Xray
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}