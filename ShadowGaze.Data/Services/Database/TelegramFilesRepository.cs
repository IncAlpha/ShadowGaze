using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database.TelegramFiles;

namespace ShadowGaze.Data.Services.Database;

public class TelegramFilesRepository(DatabaseContext context) : BaseModelRepository<TelegramFile>(context)
{
    protected override DbSet<TelegramFile> Table => DatabaseContext.TelegramFiles;
}