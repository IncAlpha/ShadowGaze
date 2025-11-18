using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class XrayApiRepository(DatabaseContext context) : BaseModelRepository<XrayApi>(context)
{
    protected override DbSet<XrayApi> Table => DatabaseContext.XrayApis;
}