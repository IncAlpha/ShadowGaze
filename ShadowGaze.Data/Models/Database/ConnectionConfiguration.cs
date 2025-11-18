namespace ShadowGaze.Data.Models.Database;

public class ConnectionConfiguration: BaseDatabaseModel
{
    public string ConnectionUri { get; set; }
    public string Protocol { get; set; }
    public string Flow { get; set; }
    public string Network { get; set; }
    public string Security { get; set; }
    public string ServerName { get; set; }
    public string PublicKey { get; set; }
    public string ShortId  { get; set; }
    public string ConnectionName { get; set; }
}