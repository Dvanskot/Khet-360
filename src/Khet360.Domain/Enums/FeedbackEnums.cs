namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum FeedbackStatus
{
    Submitted,
    UnderReview,
    Resolved,
    Closed
}

public enum ResolutionHelpfulness
{
    Helpful,
    NotHelpful
}
