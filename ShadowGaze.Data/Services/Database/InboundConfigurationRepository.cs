using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class InboundConfigurationRepository(DatabaseContext context) : BaseModelRepository<ConnectionConfiguration>(context)
{
    protected override DbSet<ConnectionConfiguration> Table => DatabaseContext.ConnectionConfigurations;

    public async Task<ConnectionConfiguration> GetByNameAsync(string name)
    {
        return await Table.FirstOrDefaultAsync(model => model.ConnectionName == name);    
    }
}