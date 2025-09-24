using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class PromotionalCodeRepository(DatabaseContext context) : BaseModelRepository<PromotionalCode>(context)
{
    protected override DbSet<PromotionalCode> Table => DatabaseContext.PromotionalCodes;
    
    public async Task<PromotionalCode> GetByValueAsync(string value)
    {
        var codes = await DatabaseContext.PromotionalCodes.ToListAsync();
        return codes.FirstOrDefault(pc => pc.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));   
    } 
}