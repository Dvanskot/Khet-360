using System;
using Khet360.Domain.Entities.Platform;

namespace Khet360.Application.Dtos;

public record TenantDetailDto(
    Guid Id,
    string Name,
    string Slug,
    SubscriptionStatus SubscriptionStatus,
    PlanCategory PlanCategory,
    string PlanName,
    IsolationTier Tier,
    bool IsActive,
    DateTime SubscriptionStartDate,
    DateTime? SubscriptionEndDate,
    DateTime? TrialEndDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record TenantListDto(
    Guid Id,
    string Name,
    string Slug,
    SubscriptionStatus SubscriptionStatus,
    string PlanName,
    IsolationTier Tier,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateTenantDto(
    string Name,
    string Slug,
    Guid SubscriptionPlanId,
    IsolationTier Tier,
    string? ContactEmail
);

public record UpdateTenantDto(
    string Name,
    bool IsActive,
    Guid? SubscriptionPlanId
);

public record SuspendTenantDto(string Reason);

public record CancelTenantDto(string Reason);

public record ReactivateTenantDto(string Reason);

public record TenantSearchFilter(
    string? SearchTerm,
    SubscriptionStatus? Status,
    IsolationTier? Tier,
    PlanCategory? PlanCategory,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20
);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
