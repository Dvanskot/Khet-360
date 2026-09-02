using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Khet360.Domain.Entities;

namespace Khet360.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;
    private readonly IEnumerable<IWidgetProvider> _widgetProviders;

    public DashboardService(TenantDbContext db, ITenantService tenantService, IEnumerable<IWidgetProvider> widgetProviders)
    {
        _db = db;
        _tenantService = tenantService;
        _widgetProviders = widgetProviders;
    }

    public async Task<Dictionary<string, object>> GetDashboardDataAsync(Guid userId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var layout = await GetUserLayoutAsync(userId);
        var activeWidgetIds = layout.Widgets.Select(w => w.WidgetId).ToList();

        var providers = _widgetProviders
            .Where(p => activeWidgetIds.Contains(p.WidgetId))
            .ToList();

        var tasks = providers.Select(async p =>
        {
            var data = await p.GetDataAsync(tenantId, userId);
            return new { p.WidgetId, Data = data };
        });

        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(r => r.WidgetId, r => r.Data);
    }

    public async Task<UserDashboardLayoutDto> GetUserLayoutAsync(Guid userId)
    {
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
