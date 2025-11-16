using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class InboundButtonRepository(DatabaseContext context) : BaseModelRepository<ConnectionButton>(context)
{
    protected override DbSet<ConnectionButton> Table => DatabaseContext.ConnectionButtons;
}