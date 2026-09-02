using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services.WidgetProviders;

public class FleetStatusProvider : IWidgetProvider
{
    private readonly TenantDbContext _db;

    public string WidgetId => "fleet-overview";

    public FleetStatusProvider(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetDataAsync(Guid tenantId, Guid userId)
    {
        var vehicles = await _db.FuneralVehicles.ToListAsync();
        var activeVehicles = await _db.TripAssignments.CountAsync(ta => !ta.IsCompleted);
        var maintenanceDue = await _db.MaintenanceSchedules.CountAsync(ms => ms.NextDueDate <= DateTime.UtcNow);

        return new {
            TotalVehicles = vehicles.Count,
            ActiveVehicles = activeVehicles,
            IdleVehicles = vehicles.Count - activeVehicles,
            MaintenanceDue = maintenanceDue,
            AvgFuelEfficiency = 0.0 // Placeholder for complex calculation
        };
    }
}
