using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;

namespace Khet360.Domain.Enums;

public enum MemorialType
{
    DigitalOnly,
    PhysicalPlaque,
    CemeteryMarker,
    InteractiveWall
}

public enum ObituaryStatus
{
    Draft,
    UnderReview,
    Published,
    Archived
}
