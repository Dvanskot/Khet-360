using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantUserContext _userContext;
    private readonly ILogger<TenantProvisioningService> _logger;

    public TenantProvisioningService(PlatformDbContext platformDb, ITenantUserContext userContext, ILogger<TenantProvisioningService> logger)
    {
        _platformDb = platformDb;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task ProvisionTenantAsync(Guid tenantId, string slug, string connectionString)
    {
        _logger.LogInformation("Provisioning tenant {Slug} ({TenantId})", slug, tenantId);

        try
        {
            // 1. Create the Database
            // We use the platform connection string to execute the CREATE DATABASE command
            // Note: This assumes the platform connection has sufficient privileges
            var platformConnectionString = _platformDb.Database.GetDbConnection().ConnectionString;
            using var connection = new SqlConnection(platformConnectionString);
            await connection.OpenAsync();

            var dbName = $"KhetLinQ_{slug}";
            var createDbSql = $"CREATE DATABASE [{dbName}]";

            using var command = new SqlCommand(createDbSql, connection);
            await command.ExecuteNonQueryAsync();
            _logger.LogInformation("Database {DbName} created successfully.", dbName);

            // 2. Apply Migrations to the new Tenant Database
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var tenantContext = new TenantDbContext(optionsBuilder.Options, _userContext);
            await tenantContext.Database.MigrateAsync();
            _logger.LogInformation("Migrations applied to database {DbName} successfully.", dbName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to provision tenant {Slug}", slug);
            throw;
        }
    }
}
