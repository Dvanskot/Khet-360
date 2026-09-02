namespace Khet360.Domain.Enums;

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
