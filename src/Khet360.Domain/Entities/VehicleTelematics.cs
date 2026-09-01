using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class VehicleTelematics : IBranchScoped
{
    public Guid Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double FuelLevel { get; set; }
    public double EngineTemperature { get; set; }
    public DateTime TimestampUtc { get; set; }

    public Guid VehicleId { get; set; }
    public virtual FuneralVehicle Vehicle { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
