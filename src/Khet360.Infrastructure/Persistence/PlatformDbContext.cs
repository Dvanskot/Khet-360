using Microsoft.EntityFrameworkCore;
using Khet360.Domain.Entities;

namespace Khet360.Infrastructure.Persistence;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
        });
    }
}
