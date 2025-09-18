using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Models.Database.Instructions;
using ShadowGaze.Data.Models.Database.TelegramFiles;
using ShadowGaze.Data.Services.Database.Extensions;

namespace ShadowGaze.Data.Services.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Xray> Xrays => Set<Xray>();
    public DbSet<Endpoint> Endpoints => Set<Endpoint>();
    public DbSet<PlatformInstruction> PlatformInstructions => Set<PlatformInstruction>();
    public DbSet<TelegramFile> TelegramFiles => Set<TelegramFile>();
    public DbSet<BotSection> BotSections => Set<BotSection>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PromotionalCode> PromotionalCodes => Set<PromotionalCode>();
    public DbSet<PromotionalCodeUsage> PromotionalCodeUsages =>Set<PromotionalCodeUsage>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureEndpoints(modelBuilder);
        ConfigureCustomers(modelBuilder);
        ConfigurePayments(modelBuilder);
        ConfigurePromotionalCodes(modelBuilder);
        ConfigurePromotionalCodeUsages(modelBuilder);
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
            .HasColumnType("timestamp without time zone")
            .HasUtcConversion();

        modelBuilder.Entity<Endpoint>()
            .Property(e => e.ExpiryDate)
            .HasColumnType("timestamp without time zone")
            .HasUtcConversion();
    }

    private void ConfigureCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Endpoint)
            .WithOne()
            .HasForeignKey<Customer>(x => x.EndpointId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigurePayments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<Payment>()
            .Property(p => p.PaymentDate)
            .HasColumnType("timestamp without time zone")
            .HasUtcConversion();
    }

    private void ConfigurePromotionalCodes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PromotionalCode>()
            .Property(pc => pc.StartDate)
            .HasColumnType("timestamp without time zone")
            .HasUtcConversion();
        
        modelBuilder.Entity<PromotionalCode>()
            .Property(pc => pc.EndDate)
            .HasColumnType("timestamp without time zone")
            .HasUtcConversion();
    }
    
    private void ConfigurePromotionalCodeUsages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PromotionalCodeUsage>()
            .HasKey(e => new { e.CustomerId, e.PromotionalCodeId });

        modelBuilder.Entity<PromotionalCodeUsage>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(e => e.CustomerId);

        modelBuilder.Entity<PromotionalCodeUsage>()
            .HasOne<PromotionalCode>()
            .WithMany()
            .HasForeignKey(e => e.PromotionalCodeId);
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