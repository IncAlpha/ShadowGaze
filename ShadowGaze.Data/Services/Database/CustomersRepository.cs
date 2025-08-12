using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class CustomersRepository(DatabaseContext context) : BaseModelRepository<Customer>(context)
{
    protected override DbSet<Customer> Table => DatabaseContext.Customers;
}