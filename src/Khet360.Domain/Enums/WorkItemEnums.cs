namespace Khet360.Domain.Enums;

public enum WorkItemPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum WorkItemStatus
{
    Pending = 0,
    InProgress = 1,
    Blocked = 2,
    Completed = 3,
    Cancelled = 4
}

public enum SlaStatus
{
    OnTrack = 0,
    Warning = 1,
    Breached = 2
}
