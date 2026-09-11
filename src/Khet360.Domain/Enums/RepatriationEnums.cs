namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum RepatriationStatus
{
    Requested,
    InTransit,
    ClearedCustoms,
    Arrived,
    Cancelled
}

public enum TransportMethod
{
    Air,
    Road,
    Rail,
    Sea
}
