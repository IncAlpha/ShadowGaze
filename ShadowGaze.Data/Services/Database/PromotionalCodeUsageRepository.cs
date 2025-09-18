using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class PromotionalCodeUsageRepository(DatabaseContext context)
{
    public async Task<PromotionalCodeUsage> GetByPrimaryKey(int customerId, int promotionalCodeId)
    {
        return await context
            .PromotionalCodeUsages
            .FirstOrDefaultAsync(pc => pc.CustomerId.Equals(customerId) && pc.PromotionalCodeId.Equals(promotionalCodeId));
    }

    public async Task<PromotionalCodeUsage> AddAsync(PromotionalCodeUsage model)
    {
        var res =  await context.PromotionalCodeUsages.AddAsync(model);
        await context.SaveChangesAsync();
        return res.Entity;
    }
}