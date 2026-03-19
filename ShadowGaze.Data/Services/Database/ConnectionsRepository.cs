using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class ConnectionsRepository(DatabaseContext context) : BaseModelRepository<Connection>(context)
{
    protected override DbSet<Connection> Table => DatabaseContext.Connections;
    
    public async Task<Connection> GetByCompositeKey(int customerId, int configurationId)
    {
        return await Table.FirstOrDefaultAsync(model => 
            model.CustomerId == customerId && 
            model.ConnectionConfigurationId == configurationId);
    }

    public async Task<int> DeleteByConfigurationIdAsync(int configurationId)
    {
        return await Table
            .Where(model => model.ConnectionConfigurationId == configurationId)
            .ExecuteDeleteAsync();
    }
}
