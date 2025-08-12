namespace ShadowGaze.Data.Models.Database;

public class Xray : BaseDatabaseModel
{
    public string Address { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}