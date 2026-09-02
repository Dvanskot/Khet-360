using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Khet360.Domain.Enums;
using Khet360.Domain.Entities;
using System.Text.Json;

namespace Khet360.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public DashboardService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<OperationalDashboardDto> GetOperationalOverviewAsync()
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        // 1. SLA Overview
        var workItems = await _db.WorkItems
            .Where(wi => wi.Status != WorkItemStatus.Completed)
            .ToListAsync();

        var warningItems = workItems.Count(wi => wi.SlaStatus == SlaStatus.Warning);
        var breachedItems = workItems.Count(wi => wi.SlaStatus == SlaStatus.Breached);

        var criticalAlerts = workItems
            .Where(wi => wi.SlaStatus == SlaStatus.Breached)
            .Select(wi => new SlaAlertDto(
                wi.Id,
                "WorkItem",
                wi.NextAction ?? "No next action defined",
                wi.DueDate,
                "Critical"))
            .Take(5)
            .ToList();

        var slaOverview = new SlaOverviewDto(
            workItems.Count,
            warningItems,
            breachedItems,
            criticalAlerts);

        // 2. Fleet Overview
        var vehicles = await _db.FuneralVehicles
            .ToListAsync();

        var activeVehicles = await _db.TripAssignments
            .CountAsync(ta => !ta.IsCompleted);

        var maintenanceDue = await _db.MaintenanceSchedules
            .CountAsync(ms => ms.NextDueDate <= DateTime.UtcNow);

        var fuelLogs = await _db.FuelLogs
            .ToListAsync();

        decimal avgFuelEfficiency = 0; // Simplified for now as it requires multi-log calculation

        var fleetOverview = new FleetOverviewDto(
            vehicles.Count,
            activeVehicles,
            vehicles.Count - activeVehicles,
            maintenanceDue,
            avgFuelEfficiency);

        // 3. Vendor Overview
        var pendingOrders = await _db.VendorOrders
            .Where(vo => vo.Status != VendorOrderStatus.Delivered)
            .ToListAsync();

        var overdueOrders = pendingOrders.Count(vo => vo.OrderedAt < DateTime.UtcNow.AddDays(-7));
        var totalPendingValue = pendingOrders.Sum(vo => vo.TotalAmount);

        var vendorOverview = new VendorOverviewDto(
            pendingOrders.Count,
            overdueOrders,
            totalPendingValue);

        // 4. CRM Overview
        var newLeads = await _db.Leads
            .CountAsync(l => l.Status == LeadStatus.New);

        var openOpportunities = await _db.Opportunities
            .CountAsync(o => o.Stage != OpportunityStage.ClosedWon && o.Stage != OpportunityStage.ClosedLost);

        var pipelineValue = await _db.Opportunities
            .Where(o => o.Stage != OpportunityStage.ClosedWon && o.Stage != OpportunityStage.ClosedLost)
            .SumAsync(o => o.EstimatedValue);

        var totalLeads = await _db.Leads.CountAsync();
        var convertedLeads = await _db.Leads.CountAsync(l => l.Status == LeadStatus.Converted);
        double conversionRate = totalLeads == 0 ? 0 : (double)convertedLeads / totalLeads * 100;

        var crmOverview = new CrmOverviewDto(
            newLeads,
            openOpportunities,
            pipelineValue,
            conversionRate);

        return new OperationalDashboardDto(
            slaOverview,
            fleetOverview,
            vendorOverview,
            crmOverview);
    }

    public async Task<UserDashboardLayoutDto> GetUserLayoutAsync(Guid userId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var config = await _db.UserDashboardConfigs
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (config == null)
        {
            return GetDefaultLayout(userId);
        }

        var widgets = JsonSerializer.Deserialize<List<DashboardWidgetDto>>(config.ConfigJson)
            ?? new List<DashboardWidgetDto>();

        return new UserDashboardLayoutDto(userId, widgets);
    }

    public async Task SaveUserLayoutAsync(UserDashboardLayoutDto layout)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var config = await _db.UserDashboardConfigs
            .FirstOrDefaultAsync(c => c.UserId == layout.UserId);

        if (config == null)
        {
            config = new UserDashboardConfig
            {
                Id = Guid.NewGuid(),
                UserId = layout.UserId
            };
            _db.UserDashboardConfigs.Add(config);
        }

        config.ConfigJson = JsonSerializer.Serialize(layout.Widgets);
        config.LastUpdatedUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    private UserDashboardLayoutDto GetDefaultLayout(Guid userId)
    {
        var defaultWidgets = new List<DashboardWidgetDto>
        {
            new("sla-overview", "SLA Overview", 4, 2, 0, 0, true),
            new("fleet-overview", "Fleet Status", 4, 2, 4, 0, true),
            new("vendor-overview", "Vendor Pending", 4, 2, 0, 2, true),
            new("crm-overview", "CRM Pipeline", 4, 2, 4, 2, true),
            new("productivity-scorecard", "Productivity Scorecard", 8, 2, 0, 4, true)
        };

        return new UserDashboardLayoutDto(userId, defaultWidgets);
    }
}
