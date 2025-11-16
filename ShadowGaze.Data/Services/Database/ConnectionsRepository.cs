using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class ConnectionsRepository(DatabaseContext context) : BaseModelRepository<Connection>(context)
{
    protected override DbSet<Connection> Table => DatabaseContext.Connections;

    public async Task<Connection> GetByInboundId(int inboundId)
    {
        return await Table.FirstOrDefaultAsync(model => model.VlessInboundId == inboundId);
    }
}