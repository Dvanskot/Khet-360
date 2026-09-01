using System;

namespace Khet360.Domain.Enums;

public enum VendorStatus
{
    Pending,
    Approved,
    Suspended,
    Blacklisted
}

public enum VendorOrderStatus
{
    Requested,
    Confirmed,
    InProduction,
    ReadyForDelivery,
    Delivered,
    Cancelled
}
