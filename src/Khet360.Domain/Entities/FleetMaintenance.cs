using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class MaintenanceSchedule : IBranchScoped
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MileageInterval { get; set; }
    public int DaysInterval { get; set; }
    public DateTime LastPerformed { get; set; }
    public DateTime NextDueDate { get; set; }

    public Guid VehicleId { get; set; }
    public virtual FuneralVehicle Vehicle { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}

public class WorkOrder : IBranchScoped
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public WorkOrderStatus Status { get; set; }
    public string? Diagnosis { get; set; }

    public Guid VehicleId { get; set; }
    public virtual FuneralVehicle Vehicle { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}

public class FuelLog : IBranchScoped
{
    public Guid Id { get; set; }
    public DateTime PurchaseDate { get; set; }
    public double Volume { get; set; }
    public decimal Cost { get; set; }
    public double MileageAtPurchase { get; set; }
    public string? FuelCardNumber { get; set; }
    public string? ReceiptImageUrl { get; set; }

    public Guid VehicleId { get; set; }
    public virtual FuneralVehicle Vehicle { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
