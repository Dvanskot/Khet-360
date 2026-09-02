using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services.WidgetProviders;

public class ProductivityScorecardProvider : IWidgetProvider
{
    private readonly TenantDbContext _db;

    public string WidgetId => "productivity-scorecard";

    public ProductivityScorecardProvider(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetDataAsync(Guid tenantId, Guid userId)
    {
        var totalItems = await _db.WorkItems.CountAsync();
        var completedItems = await _db.WorkItems.CountAsync(wi => wi.Status == Khet360.Domain.Enums.WorkItemStatus.Completed);

        double completionRate = totalItems == 0 ? 0 : (double)completedItems / totalItems * 100;

        return new {
            TotalItems = totalItems,
            CompletedItems = completedItems,
            CompletionRate = completionRate,
            EfficiencyScore = 88.2 // Mock score
        };
    }
}
