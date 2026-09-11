using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public record SubscriptionStatusDto(
    Guid TenantId,
    string PlanName,
    SubscriptionStatus Status,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime? TrialEndDate
);

public interface ISubscriptionService
{
    Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid tenantId);
    Task ChangePlanAsync(Guid tenantId, Guid newPlanId);
    Task ActivateSubscriptionAsync(Guid tenantId, int durationMonths);
    Task<bool> ValidateLimitAsync(Guid tenantId, string entitlementCode, decimal currentUsage);
}
