using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.BackgroundServices;

public class LowStockAlertWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LowStockAlertWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public LowStockAlertWorker(IServiceProvider serviceProvider, ILogger<LowStockAlertWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LowStockAlertWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    var branchService = scope.ServiceProvider.GetRequiredService<ITenantService>(); // To get branches

                    // In a real system, we would iterate over all active branches for the tenant
                    // For now, we'll assume the context is handled or we fetch branches.
                    // Since we are in a background service, we need to handle multi-tenancy carefully.
                    // In Khet-360, background tasks often operate on the Platform plane or iterate tenants.

                    _logger.LogInformation("Checking for low stock items...");

                    // Note: Simplified for demonstration.
                    // Ideally, this would iterate all branches in the database.
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for low stock.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
