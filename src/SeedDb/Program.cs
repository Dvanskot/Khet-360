using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

// Build configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Set up services
var services = new ServiceCollection();
services.AddDbContext<PlatformDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("PlatformConnection")));

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var serviceProvider = services.BuildServiceProvider();

// Get the DbContext and logger
using var scope = serviceProvider.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

// Ensure the database is created based on the model
dbContext.Database.EnsureCreated();

// Clear all data using sp_MSforeachtable
logger.LogInformation("Clearing all data from tables...");
try
{
    // Disable constraints
    await dbContext.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
    // Delete all data
    await dbContext.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'DELETE FROM ?'");
    // Re-enable constraints
    await dbContext.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
    logger.LogInformation("All data cleared.");
}
catch (Exception ex)
{
    logger.LogWarning(ex, "Warning: Could not clear data using sp_MSforeachtable. Falling back to individual deletes.");
    // Fallback: delete tables in order
    try
    {
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Tenants];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Entitlements];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [SubscriptionPlans];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [TaxBrackets];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [TaxRebates];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [StatutoryRates];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [TaxYears];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [LeaveTypes];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [PlatformFeatures];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [PlatformPaymentConfigs];");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Positions];");
        logger.LogInformation("Existing data cleared via individual deletes.");
    }
    catch (Exception ex2)
    {
        logger.LogError(ex2, "Error: Failed to clear data.");
    }
}

// Seed the database
logger.LogInformation("Starting database seeding...");
await DbInitializer.InitializeDatabase(dbContext, logger);
logger.LogInformation("Database seeding completed.");