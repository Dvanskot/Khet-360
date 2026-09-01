using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class FleetService : IFleetService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public FleetService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> RegisterVehicleAsync(VehicleCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var vehicle = new FuneralVehicle
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = dto.RegistrationNumber,
            Model = dto.Model,
            Capacity = dto.Capacity,
            Status = VehicleStatus.Available,
            TenantId = tenantId,
            BranchId = branchId
        };

        _db.FuneralVehicles.Add(vehicle);
        await _db.SaveChangesAsync();

        return vehicle.Id;
    }

    public async Task<VehicleDto?> GetVehicleAsync(Guid id)
    {
        var v = await _db.FuneralVehicles.FindAsync(id);
        if (v == null) return null;

        return new VehicleDto(v.Id, v.RegistrationNumber, v.Model, v.Capacity, v.Status, v.LastMaintenanceDate);
    }

    public async Task UpdateVehicleStatusAsync(Guid id, VehicleUpdateDto dto)
    {
        var v = await _db.FuneralVehicles.FindAsync(id);
        if (v == null) throw new KeyNotFoundException("Vehicle not found.");

        v.Status = dto.Status;
        v.LastMaintenanceDate = dto.LastMaintenanceDate;

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync(Guid branchId)
    {
        return await _db.FuneralVehicles
            .Where(v => v.BranchId == branchId && v.Status == VehicleStatus.Available)
            .Select(v => new VehicleDto(v.Id, v.RegistrationNumber, v.Model, v.Capacity, v.Status, v.LastMaintenanceDate))
            .ToListAsync();
    }
}
