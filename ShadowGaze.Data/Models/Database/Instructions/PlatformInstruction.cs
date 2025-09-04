// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

using ShadowGaze.Data.Models.Database.TelegramFiles;

namespace ShadowGaze.Data.Models.Database.Instructions;

public class PlatformInstruction : BaseDatabaseModel
{
    public Platforms? Platform { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationUrl { get; set; }
    public string Description { get; set; }
    public int? TelegramFileId { get; set; }
    public TelegramFile TelegramFile { get; set; }
}