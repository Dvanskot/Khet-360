using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class DriverProfile : IBranchScoped
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiryDate { get; set; }
    public string ContactPhone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public double SafetyScore { get; set; } = 100.0;

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}

public class TripAssignment : IBranchScoped
{
    public Guid Id { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public string RouteDetails { get; set; } = string.Empty;
    public string? DigitalSignature { get; set; }
    public string? DropOffNotes { get; set; }
    public bool IsCompleted { get; set; }

    public Guid VehicleId { get; set; }
    public virtual FuneralVehicle Vehicle { get; set; } = null!;

    public Guid DriverId { get; set; }
    public virtual DriverProfile Driver { get; set; } = null!;

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}

public class VehicleDocument : IBranchScoped
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty; // e.g., "Registration", "Insurance"
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string FileKey { get; set; } = string.Empty; // MinIO key

    public Guid VehicleId { get; set; }
    public virtual FuneralVehicle Vehicle { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
