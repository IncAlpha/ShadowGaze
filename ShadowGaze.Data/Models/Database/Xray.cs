namespace ShadowGaze.Data.Models.Database;

public class Xray : BaseDatabaseModel
{
    public string Host { get; set; } = string.Empty;
    public int  Port { get; set; } = 443;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}