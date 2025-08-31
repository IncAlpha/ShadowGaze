// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace ShadowGaze.Data.Models.Database.Instructions;

public class PlatformInstruction : BaseDatabaseModel
{
    public Platforms? Platform { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationUrl { get; set; }
    public string Description { get; set; }
    public string FileId { get; set; }
}