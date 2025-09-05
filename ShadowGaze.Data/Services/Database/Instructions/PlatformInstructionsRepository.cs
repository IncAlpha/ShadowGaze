using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models;
using ShadowGaze.Data.Models.Database.Instructions;

namespace ShadowGaze.Data.Services.Database.Instructions;

public class PlatformInstructionsRepository(DatabaseContext context) : BaseModelRepository<PlatformInstruction>(context)
{
    protected override DbSet<PlatformInstruction> Table => context.PlatformInstructions;
    
    public PlatformInstruction[] GetByPlatform(Platforms platform)
    {
        return Table
            .Where(model => model.Platform == platform)
            .ToArray();
    }

    public async Task<PlatformInstruction[]> GetByPlatformAsync(Platforms platform)
    {
        return await Table
            .Where(model => model.Platform == platform)
            .ToArrayAsync();
    }

    public async Task<PlatformInstruction> GetForMessageAsync(int entityIg)
    {
        return await Table
            .Include(p => p.TelegramFile)
            .FirstOrDefaultAsync(model => model.Id == entityIg);
    }
}