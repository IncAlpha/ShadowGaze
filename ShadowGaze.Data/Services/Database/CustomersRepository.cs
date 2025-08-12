using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class CustomersRepository : BaseModelRepository<Customer>
{
    protected override DbSet<Customer> Table => DatabaseContext.Customers;
}