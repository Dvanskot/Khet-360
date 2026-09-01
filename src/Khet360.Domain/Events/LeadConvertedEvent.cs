using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Events;

public record LeadConvertedEvent(
    Guid LeadId,
    Guid CustomerId,
    Guid? OpportunityId,
    DateTime ConvertedAt);

public record FuneralCaseOpenedEvent(
    Guid CaseId,
    Guid CustomerId,
    DateTime OpenedAt);

public record SlaBreachedEvent(
    Guid WorkItemId,
    SlaStatus Status,
    DateTime BreachedAt);
