namespace ShadowGaze.Data.Models.Database.TelegramFiles;

public class TelegramFile : BaseDatabaseModel
{
    public string FileId { get; init; }
    public string FileUniqueId { get; init; }
    public TelegramFileTypes FileType { get; init; }

    /// <summary>
    /// Служит для технического описания (главное меню, разделы и т.д.)
    /// </summary>
    public string Description { get; init; }
}