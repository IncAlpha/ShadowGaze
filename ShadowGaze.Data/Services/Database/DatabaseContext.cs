using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Xray> Xrays => Set<Xray>();
    public DbSet<Endpoint> Endpoints => Set<Endpoint>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureEndpoints(modelBuilder);
        ConfigureCustomers(modelBuilder);
    }

    private void ConfigureEndpoints(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Endpoint>()
            .HasOne(e => e.Xray)
            .WithMany()
            .HasForeignKey(x => x.XrayId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Endpoint)
            .WithOne()
            .HasForeignKey<Customer>(x => x.EndpointId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}