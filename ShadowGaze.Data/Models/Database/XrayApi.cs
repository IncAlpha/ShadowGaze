namespace ShadowGaze.Data.Models.Database;

public class XrayApi: BaseDatabaseModel
{
    public bool Obsolete { get; set; }
    public string ApiUri { get; set; }
    public string InboundTag { get; set; }
}