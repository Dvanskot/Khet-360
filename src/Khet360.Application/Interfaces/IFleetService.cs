using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;

namespace Khet360.Application.Interfaces;

public record VehicleDto(
    Guid Id,
    string RegistrationNumber,
    string Model,
    int Capacity,
    VehicleStatus Status,
    DateTime? LastMaintenanceDate);

public record VehicleCreateDto(
    string RegistrationNumber,
    string Model,
    int Capacity);

public record VehicleUpdateDto(
    VehicleStatus Status,
    DateTime? LastMaintenanceDate);

public interface IFleetService
{
    Task<Guid> RegisterVehicleAsync(VehicleCreateDto dto, Guid branchId);
    Task<VehicleDto?> GetVehicleAsync(Guid id);
    Task UpdateVehicleStatusAsync(Guid id, VehicleUpdateDto dto);
    Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync(Guid branchId);
    Task<double> CalculateFuelEfficiencyAsync(Guid vehicleId);
    Task<List<Guid>> GetVehiclesRequiringMaintenanceAsync(Guid branchId);
    Task AssignTripAsync(Guid vehicleId, Guid driverId, Guid caseId, string route);
}
