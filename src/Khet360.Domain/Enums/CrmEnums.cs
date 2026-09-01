using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Enums;

public enum LeadStatus
{
    New = 0,
    Contacted = 1,
    Qualified = 2,
    Disqualified = 3
}

public enum OpportunityStage
{
    Discovery = 0,
    Proposal = 1,
    Negotiation = 2,
    Won = 3,
    Lost = 4
}

public enum ActivityType
{
    Call = 0,
    Email = 1,
    Visit = 2,
    Meeting = 3,
    Note = 4
}
