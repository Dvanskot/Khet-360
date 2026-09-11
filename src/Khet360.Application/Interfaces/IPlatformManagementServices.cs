using System;
using System.Threading.Tasks;
using Khet360.Domain.Entities.Platform;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IPlatformTenantService
{
    Task<TenantDetailDto> GetTenantDetailsAsync(Guid tenantId);
    Task<PagedResult<TenantListDto>> SearchTenantsAsync(TenantSearchFilter filter);
    Task<bool> SuspendTenantAsync(Guid tenantId, string reason, string performedBy);
    Task<bool> CancelTenantAsync(Guid tenantId, string reason, string performedBy);
    Task<bool> ReactivateTenantAsync(Guid tenantId, string reason, string performedBy);
    Task<bool> UpdateTenantAsync(Guid tenantId, UpdateTenantDto dto, string performedBy);
}

public interface IPlatformAnalyticsService
{
    Task<PlatformOverviewDto> GetPlatformHealthAsync();
    Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync();
    Task<TenantGrowthAnalyticsDto> GetTenantGrowthAsync();
    Task<SubscriptionDistributionDto> GetSubscriptionDistributionAsync();
}

public interface IPlatformFeatureService
{
    Task<List<FeatureFlagDto>> GetAllFeatureFlagsAsync(Guid? tenantId = null);
    Task<bool> UpdateGlobalFeatureAsync(Guid featureId, bool isEnabled, string performedBy);
    Task<bool> SetTenantFeatureOverrideAsync(Guid tenantId, Guid featureId, bool isEnabled, string performedBy);
    Task<bool> RemoveTenantFeatureOverrideAsync(Guid tenantId, Guid featureId, string performedBy);
    Task<bool> IsFeatureEnabledForTenantAsync(Guid tenantId, string featureCode);
}

public interface IPlatformAuditService
{
    Task LogAuditAsync(Guid tenantId, string tenantSlug, AuditAction action, string entityType, string entityId, string? oldValues, string? newValues, string performedBy, bool isSuccess, string? errorMessage = null, string? reason = null);
    Task<List<AuditLogDto>> GetTenantAuditLogAsync(Guid tenantId, int page = 1, int pageSize = 50);
    Task<List<AuditLogDto>> GetPlatformAuditLogAsync(int page = 1, int pageSize = 50);
}

public interface IPlatformAnnouncementService
{
    Task<AnnouncementDto> CreateAnnouncementAsync(string title, string content, AnnouncementPriority priority, AnnouncementScope scope, Guid? specificTenantId, string targetPlanCategories, Guid createdByUserId, string createdByUsername);
    Task<List<AnnouncementDto>> GetActiveAnnouncementsAsync(Guid? tenantId = null);
    Task<bool> DeactivateAnnouncementAsync(Guid announcementId);
}

public interface IPlatformAuthService
{
    Task<AuthResponse?> LoginAsync(string username, string password);
    Task<AuthResponse?> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> LogoutAsync(string token, string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
}
