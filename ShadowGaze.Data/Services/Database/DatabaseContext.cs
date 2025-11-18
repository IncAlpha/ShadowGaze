using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Migrations;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Models.Database.Instructions;
using ShadowGaze.Data.Models.Database.TelegramFiles;
using ShadowGaze.Data.Services.Database.Extensions;
using PromotionalCode = ShadowGaze.Data.Models.Database.PromotionalCode;

namespace ShadowGaze.Data.Services.Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PlatformInstruction> PlatformInstructions => Set<PlatformInstruction>();
    public DbSet<TelegramFile> TelegramFiles => Set<TelegramFile>();
    public DbSet<BotSection> BotSections => Set<BotSection>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PromotionalCode> PromotionalCodes => Set<PromotionalCode>();
    public DbSet<PromotionalCodeUsage> PromotionalCodeUsages =>Set<PromotionalCodeUsage>();
    public DbSet<Connection> Connections => Set<Connection>();
    public DbSet<ConnectionConfiguration> ConnectionConfigurations => Set<ConnectionConfiguration>();
    public DbSet<XrayApi> XrayApis => Set<XrayApi>();
    public DbSet<ConnectionButton> ConnectionButtons => Set<ConnectionButton>();
        
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureCustomers(modelBuilder);
        ConfigurePayments(modelBuilder);
        ConfigurePromotionalCodes(modelBuilder);
        ConfigurePromotionalCodeUsages(modelBuilder);
        ConfigureConnection(modelBuilder);
        ConfigureConnectionButtons(modelBuilder);
    }

    private void ConfigureCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasUtcConversion();

        modelBuilder.Entity<Customer>()
            .Property(e => e.ExpiryDate)
            .HasColumnType("timestamp without time zone")
            .HasUtcConversion();
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

    private void ConfigureConnection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Connection>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(connection => connection.CustomerId);
        
        modelBuilder.Entity<Connection>()
            .HasOne<ConnectionConfiguration>()
            .WithOne()
            .HasForeignKey<Connection>(connection => connection.VlessInboundId);
    }

    private void ConfigureConnectionButtons(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConnectionButton>()
            .HasOne<ConnectionConfiguration>()
            .WithOne()
            .HasForeignKey<ConnectionButton>(cb => cb.ConnectionConfigurationId);
    }
}