using Microsoft.EntityFrameworkCore;

namespace ShadowGaze.Data.Models.Database.TelegramFiles;

public class BotSection : BaseDatabaseModel
{
    public string SectionName { get; set; }

    public int? TelegramFileId { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public TelegramFile TelegramFile { get; set; }
}