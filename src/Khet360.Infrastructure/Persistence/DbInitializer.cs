using Khet360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Persistence;

public class DbInitializer
{
    public static async Task InitializeDatabase(PlatformDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Initializing Platform Database...");

            // Ensure database is created (standard for dev, migrations are better for prod)
            await context.Database.EnsureCreatedAsync();

            if (!await context.Tenants.AnyAsync())
            {
                logger.LogInformation("Seeding initial tenants...");

                var seedTenants = new List<Tenant>
                {
                    new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant Alpha",
                        Slug = "tenanta",
                        ConnectionString = "Server=localhost;Database=Khet360_TenantA;Trusted_Connection=True; TrustServerCertificate=True;",
                        IsActive = true,
                        SubscriptionPlanId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant Beta",
                        Slug = "tenantb",
                        ConnectionString = "Server=localhost;Database=Khet360_TenantB;Trusted_Connection=True; TrustServerCertificate=True;",
                        IsActive = true,
                        SubscriptionPlanId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Tenants.AddRangeAsync(seedTenants);
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully seeded {Count} tenants.", seedTenants.Count);
            }
            else
            {
                logger.LogInformation("Platform database already contains tenants. Skipping seed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the Platform Database.");
            throw;
        }
    }
}
