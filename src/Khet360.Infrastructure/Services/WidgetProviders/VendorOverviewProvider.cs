using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services.WidgetProviders;

public class VendorOverviewProvider : IWidgetProvider
{
    private readonly TenantDbContext _db;

    public string WidgetId => "vendor-overview";

    public VendorOverviewProvider(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetDataAsync(Guid tenantId, Guid userId)
    {
        var pendingOrders = await _db.VendorOrders
            .Where(vo => vo.Status != VendorOrderStatus.Delivered)
            .ToListAsync();

        var overdueOrders = pendingOrders.Count(vo => vo.OrderedAt < DateTime.UtcNow.AddDays(-7));
        var totalPendingValue = pendingOrders.Sum(vo => vo.TotalAmount);

        return new {
            PendingCount = pendingOrders.Count,
            OverdueCount = overdueOrders,
            TotalPendingValue = totalPendingValue
        };
    }
}
