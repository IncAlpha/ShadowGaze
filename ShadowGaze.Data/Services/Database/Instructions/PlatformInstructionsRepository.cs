using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models;
using ShadowGaze.Data.Models.Database.Instructions;

namespace ShadowGaze.Data.Services.Database.Instructions;

public class PlatformInstructionsRepository(DatabaseContext context) : BaseModelRepository<PlatformInstruction>(context)
{
    protected override DbSet<PlatformInstruction> Table => context.PlatformInstructions;
    
    public virtual PlatformInstruction[] GetForPlatform(Platforms platform)
    {
        return Table.Where(model => model.Platform == platform).ToArray();
    }

    public virtual async Task<PlatformInstruction[]> GetForPlatformAsync(Platforms platform)
    {
        return await Table.Where(model => model.Platform == platform).ToArrayAsync();
    }
}