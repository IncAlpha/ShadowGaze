namespace ShadowGaze.Data.Models.Database.Tunels;

public class XrayTunel : BaseDatabaseModel
{
    public string Ip { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool Obsolete { get; set; }
}