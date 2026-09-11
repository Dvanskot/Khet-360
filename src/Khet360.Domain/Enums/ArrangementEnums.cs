namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum ArrangementType
{
    Burial,
    Cremation,
    MemorialService,
    Wake,
    Viewing,
    Other
}

public enum CateringStatus
{
    Pending,
    Confirmed,
    Cancelled
}
