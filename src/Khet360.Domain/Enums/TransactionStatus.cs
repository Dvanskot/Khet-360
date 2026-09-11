namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum TransactionStatus
{
    Pending,
    Success,
    Failed,
    Cancelled,
    Refunded
}
