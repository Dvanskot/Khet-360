using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Khet360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Khet360.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantUserContext _userContext;
    private readonly ILogger<TenantProvisioningService> _logger;
    private readonly IConfiguration _configuration;

    public TenantProvisioningService(PlatformDbContext platformDb, ITenantUserContext userContext, ILogger<TenantProvisioningService> logger, IConfiguration configuration)
    {
        _platformDb = platformDb;
        _userContext = userContext;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> ProvisionTenantAsync(Guid tenantId, string slug, IsolationTier tier)
    {
        _logger.LogInformation("Provisioning tenant {Slug} ({TenantId}) with tier {Tier}", slug, tenantId, tier);

        try
        {
            // 1. Resolve the target server based on tier
            string serverConnectionString;
            if (tier == IsolationTier.Dedicated)
            {
                serverConnectionString = _configuration["SqlSettings:DedicatedServerConnectionString"]
                    ?? throw new InvalidOperationException("Dedicated server connection string is not configured.");
            }
            else
            {
                serverConnectionString = _configuration.GetConnectionString("PlatformConnection")
                    ?? throw new InvalidOperationException("Platform connection string is not configured.");
            }

            // 2. Create the Database
            using var connection = new SqlConnection(serverConnectionString);
            await connection.OpenAsync();

            var dbName = $"KhetLinQ_{slug}";
            var createDbSql = $"CREATE DATABASE [{dbName}]";

            using var command = new SqlCommand(createDbSql, connection);
            await command.ExecuteNonQueryAsync();
            _logger.LogInformation("Database {DbName} created successfully on {Tier} tier.", dbName, tier);

            // 3. Generate the final connection string for the tenant
            var tenantConnectionString = serverConnectionString.Replace("Initial Catalog=master", $"Initial Catalog={dbName}")
                                                             .Replace("Database=master", $"Database={dbName}");

            if (!tenantConnectionString.Contains("Initial Catalog=") && !tenantConnectionString.Contains("Database="))
            {
                tenantConnectionString += $";Initial Catalog={dbName}";
            }

            // 4. Apply Migrations to the new Tenant Database
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(tenantConnectionString);

            using var tenantContext = new TenantDbContext(optionsBuilder.Options, _userContext);
            await tenantContext.Database.MigrateAsync();
            _logger.LogInformation("Migrations applied to database {DbName} successfully.", dbName);

            return tenantConnectionString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to provision tenant {Slug}", slug);
            throw;
        }
    }
}
