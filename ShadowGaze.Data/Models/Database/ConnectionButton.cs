namespace ShadowGaze.Data.Models.Database;

public class ConnectionButton : BaseDatabaseModel
{
    public string ButtonName { get; set; }
    public int ConnectionConfigurationId { get; set; }
}