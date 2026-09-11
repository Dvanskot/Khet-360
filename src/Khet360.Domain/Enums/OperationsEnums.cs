namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

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

