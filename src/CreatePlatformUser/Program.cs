using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

// Build configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get connection string from configuration
var connectionString = configuration.GetConnectionString("PlatformConnection");
// Fallback if not found in appsettings.json
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Server=localhost;Database=Khet360_Platform;Trusted_Connection=True;TrustServerCertificate=True;";
}

// Set up services
var services = new ServiceCollection();
services.AddDbContext<PlatformDbContext>(options =>
    options.UseSqlServer(connectionString));

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

// Check if any platform user exists
var existingUser = await dbContext.PlatformUsers.FirstOrDefaultAsync();
if (existingUser != null)
{
    logger.LogInformation("Platform user already exists. Skipping creation.");
    return;
}

// Hash password
string password = "admin123"; // You can change this password as needed
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

// Create user
var user = new PlatformUser
{
    Id = Guid.NewGuid(),
    Username = "admin",
    Email = "admin@example.com",
    PasswordHash = hashedPassword,
    Role = "Admin",
    IsActive = true,
    CreatedAtUtc = DateTime.UtcNow,
    UpdatedAtUtc = DateTime.UtcNow
};

dbContext.PlatformUsers.Add(user);
await dbContext.SaveChangesAsync();

logger.LogInformation("Created platform user with Username: {Username}", user.Username);