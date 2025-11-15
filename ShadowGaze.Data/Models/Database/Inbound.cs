namespace ShadowGaze.Data.Models.Database;

public class Inbound: BaseDatabaseModel
{
    public bool Obsolete { get; set; }
    public int TunnelPort { get; set; }
    public string InboundTag { get; set; }
    public string Protocol { get; set; }
    public string Address { get; set; }
    public int Port { get; set; }
    public string Flow { get; set; }
    public string Network { get; set; }
    public string Security { get; set; }
    public string ServerName { get; set; }
    public string PublicKey { get; set; }
    public string ShortId  { get; set; }
    public string ConnectionTag { get; set; }
}