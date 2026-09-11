using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.BackgroundServices;

public class LowStockAlertWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LowStockAlertWorker> _logger;
    private readonly IPlatformCacheService _cache;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public LowStockAlertWorker(IServiceProvider serviceProvider, ILogger<LowStockAlertWorker> logger, IPlatformCacheService cache)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LowStockAlertWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tenants = await _cache.GetTenantsAsync();

                foreach (var tenant in tenants)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    using var tenantScope = _serviceProvider.CreateScope();
                    var tenantService = tenantScope.ServiceProvider.GetRequiredService<ITenantService>();
                    tenantService.SetTenant(tenant);

                    var db = tenantScope.ServiceProvider.GetRequiredService<TenantDbContext>();
                    var inventoryService = tenantScope.ServiceProvider.GetRequiredService<IInventoryService>();
                    var notificationService = tenantScope.ServiceProvider.GetRequiredService<INotificationService>();

                    var branches = await db.Branches
                        .Where(b => b.IsActive)
                        .ToListAsync(stoppingToken);

                    foreach (var branch in branches)
                    {
                        var lowStockItems = await inventoryService.GetLowStockItemsAsync(branch.Id);

                        foreach (var stock in lowStockItems)
                        {
                            var branchUserIds = await db.UserBranches
                                .Where(ub => ub.BranchId == branch.Id)
                                .Select(ub => ub.UserId)
                                .Distinct()
                                .ToListAsync(stoppingToken);

                            foreach (var userId in branchUserIds)
                            {
                                await notificationService.SendNotificationAsync(
                                    userId,
                                    "Low Stock Alert",
                                    $"Product {stock.ProductId} at branch {branch.Name} is low on stock. Current quantity: {stock.QuantityOnHand}, Reorder level: {stock.ReorderLevel}",
                                    NotificationPriority.High);
                            }
                        }
                    }
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
