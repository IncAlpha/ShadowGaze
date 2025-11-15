using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class InboundRepository(DatabaseContext context) : BaseModelRepository<Inbound>(context)
{
    protected override DbSet<Inbound> Table => DatabaseContext.Inbounds;
}