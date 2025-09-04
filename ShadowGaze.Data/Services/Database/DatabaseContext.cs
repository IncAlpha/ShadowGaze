using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Models.Database.Instructions;
using ShadowGaze.Data.Models.Database.TelegramFiles;

namespace ShadowGaze.Data.Services.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Xray> Xrays => Set<Xray>();
    public DbSet<Endpoint> Endpoints => Set<Endpoint>();
    public DbSet<PlatformInstruction> PlatformInstructions => Set<PlatformInstruction>();
    public DbSet<TelegramFile> TelegramFiles => Set<TelegramFile>();

    public DbSet<BotSection> BotSections => Set<BotSection>();

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

        modelBuilder.Entity<Endpoint>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone");

        modelBuilder.Entity<Endpoint>()
            .Property(e => e.ExpiryDate)
            .HasColumnType("timestamp without time zone");
    }

    private void ConfigureCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Endpoint)
            .WithOne()
            .HasForeignKey<Customer>(x => x.EndpointId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigurePlatformInstructions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlatformInstruction>()
            .HasOne(model => model.TelegramFile)
            .WithMany()
            .HasForeignKey(instruction => instruction.TelegramFileId);
    }

    private void ConfigureMenuSections(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BotSection>()
            .HasOne(model => model.TelegramFile)
            .WithMany()
            .HasForeignKey(section => section.TelegramFileId);
    }
}