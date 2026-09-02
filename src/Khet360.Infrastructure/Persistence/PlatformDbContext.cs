using Microsoft.EntityFrameworkCore;
using Khet360.Domain.Entities;

namespace Khet360.Infrastructure.Persistence;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
    public DbSet<Entitlement> Entitlements { get; set; } = null!;
    public DbSet<PlatformBackupJob> BackupJobs { get; set; } = null!;
    public DbSet<PlatformMigrationJob> MigrationJobs { get; set; } = null!;
    public DbSet<TaxYear> TaxYears { get; set; } = null!;
    public DbSet<TaxBracket> TaxBrackets { get; set; } = null!;
    public DbSet<TaxRebate> TaxRebates { get; set; } = null!;
    public DbSet<StatutoryRate> StatutoryRates { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<LeaveType> LeaveTypes { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);

            entity.HasOne(t => t.SubscriptionPlan)
                .WithMany(p => p.Tenants)
                .HasForeignKey(t => t.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Entitlement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.SubscriptionPlan)
                .WithMany(p => p.Entitlements)
                .HasForeignKey(e => e.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
