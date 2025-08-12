using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class XrayRepository(DatabaseContext context) : BaseModelRepository<Xray>(context)
{
    protected override DbSet<Xray> Table => DatabaseContext.Xrays;
}