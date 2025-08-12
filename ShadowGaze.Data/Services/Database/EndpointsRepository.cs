using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class EndpointsRepository : BaseModelRepository<Endpoint>
{
    protected override DbSet<Endpoint> Table => DatabaseContext.Endpoints;
}