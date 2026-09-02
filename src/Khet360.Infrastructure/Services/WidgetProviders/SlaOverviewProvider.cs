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

public class SlaOverviewProvider : IWidgetProvider
{
    private readonly TenantDbContext _db;

    public string WidgetId => "sla-overview";

    public SlaOverviewProvider(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetDataAsync(Guid tenantId, Guid userId)
    {
        var workItems = await _db.WorkItems
            .Where(wi => wi.Status != WorkItemStatus.Completed)
            .ToListAsync();

        var warningItems = workItems.Count(wi => wi.SlaStatus == SlaStatus.Warning);
        var breachedItems = workItems.Count(wi => wi.SlaStatus == SlaStatus.Breached);

        var criticalAlerts = workItems
            .Where(wi => wi.SlaStatus == SlaStatus.Breached)
            .Select(wi => new {
                wi.Id,
                Type = "WorkItem",
                wi.NextAction,
                wi.DueDate,
                Severity = "Critical"
            })
            .Take(5)
            .ToList();

        return new {
            Total = workItems.Count,
            Warnings = warningItems,
            Breaches = breachedItems,
            Alerts = criticalAlerts
        };
    }
}
