using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public class CustomersRepository(DatabaseContext context) : BaseModelRepository<Customer>(context)
{
    protected override DbSet<Customer> Table => DatabaseContext.Customers;

    public async Task<Customer> GetOrCreateAsync(long id, string username)
    {
        var customer = await DatabaseContext.Customers.FirstOrDefaultAsync(c => c.TelegramId == id);
        if (customer == null)
        {
            customer = new Customer
            {
                Id = 0,
                TelegramId = id,
                TelegramName = username,
                CreatedAt = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(21).Date,
                ClientId = Guid.NewGuid()
            };
            await DatabaseContext.Customers.AddAsync(customer);
            await DatabaseContext.SaveChangesAsync();
        }
        return customer;
    }
    
    public async Task<Customer> GetByTelegramIdAsync(long id)
    {
        return await DatabaseContext.Customers.FirstOrDefaultAsync(c => c.TelegramId == id);   
    }
}