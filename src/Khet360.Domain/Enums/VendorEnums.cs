using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

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
