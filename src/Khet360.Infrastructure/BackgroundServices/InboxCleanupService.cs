using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Khet360.Infrastructure.BackgroundServices;

public class InboxCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxCleanupService> _logger;

    public InboxCleanupService(IServiceProvider serviceProvider, ILogger<InboxCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Running Inbox cleanup job...");

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

                var cutoff = DateTime.UtcNow.AddDays(-30);
                var oldMessages = db.InboxMessages
                    .Where(m => m.ProcessedAt < cutoff);

                var count = await oldMessages.CountAsync();
                if (count > 0)
                {
                    db.InboxMessages.RemoveRange(oldMessages);
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Cleaned up {Count} old inbox messages.", count);
                }
                else
                {
                    _logger.LogInformation("No old inbox messages to clean up.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Inbox cleanup job.");
            }

            // Run every 24 hours
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
