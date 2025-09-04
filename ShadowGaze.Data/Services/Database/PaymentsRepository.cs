using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class PaymentsRepository(DatabaseContext context) : BaseModelRepository<Payment>(context)
{
    protected override DbSet<Payment> Table => DatabaseContext.Payments;
}