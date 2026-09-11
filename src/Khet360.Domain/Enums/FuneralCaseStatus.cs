namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum FuneralCaseStatus
{
    Enquiry = 0,
    Opened = 1,
    Verification = 2,
    Arrangement = 3,
    PolicyCheck = 4,
    RepatriationMortuary = 5,
    ServicePlanning = 6,
    ResourceAssignment = 7,
    ServiceDelivery = 8,
    BurialCremation = 9,
    Settlement = 10,
    MemorialFollowUp = 11,
    Closed = 12
}
