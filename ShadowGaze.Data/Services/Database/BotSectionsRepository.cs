using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database.TelegramFiles;

namespace ShadowGaze.Data.Services.Database;

public class BotSectionsRepository(DatabaseContext context) : BaseModelRepository<BotSection>(context)
{
    protected override DbSet<BotSection> Table => DatabaseContext.BotSections;
    
    public Task<BotSection> GetByNameAsync(string name)
    {
        return Table.Where(model => model.SectionName == name)
            .Include(section => section.TelegramFile)
            .FirstOrDefaultAsync();
    }
}