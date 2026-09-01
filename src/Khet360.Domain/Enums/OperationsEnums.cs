namespace Khet360.Domain.Enums;

public enum VehicleStatus
{
    Available,
    InUse,
    Maintenance,
    Retired,
    Moving,
    Idle,
    Parked
}

public enum MortuarySlotStatus
{
    Available,
    Occupied,
    Reserved
}

public enum WorkOrderStatus
{
    Open,
    InProgress,
    AwaitingParts,
    Completed,
    Cancelled
}

