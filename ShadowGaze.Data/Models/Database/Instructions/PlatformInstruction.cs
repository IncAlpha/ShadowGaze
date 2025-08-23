namespace ShadowGaze.Data.Models.Database.Instructions;

public class PlatformInstruction : BaseDatabaseModel
{
    public Platforms Platform { get; init; }
    public string ApplicationName { get; init; }
    public string ApplicationUrl { get; init; }
    public string Description { get; init; }
    public string VideoPath { get; init; }
}