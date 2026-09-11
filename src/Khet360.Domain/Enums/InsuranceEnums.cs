namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum PolicyStatus
{
    Active,
    Lapsed,
    Cancelled
}

public enum InsuranceCoverType
{
    Burial,
    Cash
}

public enum ClaimStatus
{
    Submitted,
    UnderReview,
    Approved,
    Paid,
    Rejected
}

public enum MemberRole
{
    Main,
    Spouse,
    Child,
    Parent,
    Extended
}
