using Microsoft.EntityFrameworkCore;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

namespace Khet360.Infrastructure.Persistence;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
    public DbSet<Entitlement> Entitlements { get; set; } = null!;
    public DbSet<PlatformPaymentConfig> PlatformPaymentConfigs { get; set; } = null!;
    public DbSet<PlatformPaymentReceipt> PlatformPaymentReceipts { get; set; } = null!;
    public DbSet<PlatformBackupJob> BackupJobs { get; set; } = null!;
    public DbSet<PlatformRestoreJob> RestoreJobs { get; set; } = null!;
    public DbSet<PlatformMigrationJob> MigrationJobs { get; set; } = null!;
    public DbSet<TaxYear> TaxYears { get; set; } = null!;
    public DbSet<TaxBracket> TaxBrackets { get; set; } = null!;
    public DbSet<TaxRebate> TaxRebates { get; set; } = null!;
    public DbSet<StatutoryRate> StatutoryRates { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<LeaveType> LeaveTypes { get; set; } = null!;
    public DbSet<PlatformFeature> PlatformFeatures { get; set; } = null!;
    public DbSet<TenantFeatureOverride> TenantFeatureOverrides { get; set; } = null!;
    public DbSet<TenantAuditLog> TenantAuditLogs { get; set; } = null!;
    public DbSet<TenantUsageMetric> TenantUsageMetrics { get; set; } = null!;
    public DbSet<PlatformAnnouncement> PlatformAnnouncements { get; set; } = null!;
    public DbSet<PlatformUser> PlatformUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ConnectionString).IsRequired();
            entity.HasOne(t => t.SubscriptionPlan)
                .WithMany(p => p.Tenants)
                .HasForeignKey(t => t.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
        });

        modelBuilder.Entity<Entitlement>(entity =>
                 {
                     entity.HasKey(e => e.Id);
                     entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                     entity.Property(e => e.Description).IsRequired();
                     entity.HasIndex(e => new { e.Code, e.SubscriptionPlanId }).IsUnique();
                     entity.HasOne(e => e.SubscriptionPlan)
                         .WithMany(p => p.Entitlements)
                         .HasForeignKey(e => e.SubscriptionPlanId)
                         .OnDelete(DeleteBehavior.Cascade);
                 });

        modelBuilder.Entity<PlatformPaymentConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProviderName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ApiKey).IsRequired();
            entity.Property(e => e.SecretKey).IsRequired();
            entity.Property(e => e.WebhookSecret).IsRequired();
        });

        modelBuilder.Entity<PlatformPaymentReceipt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionReference).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.TransactionReference).IsUnique();
            entity.HasIndex(e => e.TenantId);
        });

        modelBuilder.Entity<PlatformBackupJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.RequestedAtUtc });
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<PlatformRestoreJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => e.BackupJobId);
        });

        modelBuilder.Entity<PlatformMigrationJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => e.BackupJobId);
        });

        modelBuilder.Entity<TaxYear>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.YearLabel).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.YearLabel).IsUnique();
        });

        modelBuilder.Entity<TaxBracket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.TaxYear)
                .WithMany()
                .HasForeignKey(e => e.TaxYearId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaxRebate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.TaxYear)
                .WithMany()
                .HasForeignKey(e => e.TaxYearId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StatutoryRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.TaxYear)
                .WithMany()
                .HasForeignKey(e => e.TaxYearId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<PlatformFeature>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasMany(e => e.TenantOverrides)
                .WithOne(e => e.Feature)
                .HasForeignKey(e => e.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TenantFeatureOverride>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.FeatureId }).IsUnique();
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TenantAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.PerformedAtUtc });
            entity.HasIndex(e => e.EntityType);
            entity.Property(e => e.Reason).HasMaxLength(1000);
        });

        modelBuilder.Entity<TenantUsageMetric>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.RecordedAtUtc });
            entity.HasIndex(e => e.MetricCode);
        });

        modelBuilder.Entity<PlatformAnnouncement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedByUsername).IsRequired();
            entity.HasIndex(e => new { e.IsActive, e.StartDate, e.EndDate });
        });

        modelBuilder.Entity<PlatformUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(320);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
