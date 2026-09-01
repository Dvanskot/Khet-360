namespace Khet360.Domain.Enums;

public enum LeadStatus
{
    New = 0,
    Contacted = 1,
    Qualified = 2,
    Converted = 3,
    Lost = 4,
    Disqualified = 5
}

public enum OpportunityStage
{
    Qualification = 0,
    Discovery = 1,
    Proposal = 2,
    Negotiation = 3,
    ClosedWon = 4,
    ClosedLost = 5
}

public enum ActivityType
{
    Call = 0,
    Email = 1,
    Visit = 2,
    Meeting = 3,
    Note = 4
}

public enum ActivityStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}
