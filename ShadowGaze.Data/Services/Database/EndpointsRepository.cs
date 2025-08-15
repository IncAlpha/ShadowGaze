using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class EndpointsRepository(DatabaseContext context) : BaseModelRepository<Endpoint>(context)
{
    protected override DbSet<Endpoint> Table => DatabaseContext.Endpoints;
    
    
}