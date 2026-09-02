using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class FuneralVehicle : IBranchScoped
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public VehicleStatus Status { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }

    public Guid BranchId { get; set; }
}
