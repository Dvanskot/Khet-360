using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Application.Validators;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace Khet360.Tests;

public class PlatformManagementServiceTests
{
    private async Task<(IServiceProvider, PlatformDbContext)> GetPlatformServiceProviderAsync()
    {
        var services = new ServiceCollection();

        var platformDb = new PlatformDbContext(new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: "PlatformDB_" + Guid.NewGuid().ToString()).Options);
        services.AddSingleton(platformDb);

        await DbInitializer.InitializeDatabase(platformDb, new Mock<ILogger<PlatformDbContext>>().Object);

        services.AddSingleton(new Mock<ILogger<PlatformTenantService>>().Object);
        services.AddSingleton(new Mock<ILogger<PlatformAnalyticsService>>().Object);
        services.AddSingleton(new Mock<ILogger<PlatformFeatureService>>().Object);
        services.AddSingleton(new Mock<ILogger<PlatformAuditService>>().Object);
        services.AddSingleton(new Mock<ILogger<PlatformAnnouncementService>>().Object);

        var mockCache = new Mock<IPlatformCacheService>();
        mockCache.Setup(c => c.InvalidateTenantsAsync()).Returns(Task.CompletedTask);
        mockCache.Setup(c => c.InvalidateAllAsync()).Returns(Task.CompletedTask);
        mockCache.Setup(c => c.InvalidateSubscriptionPlansAsync()).Returns(Task.CompletedTask);
        services.AddSingleton<IPlatformCacheService>(mockCache.Object);

        services.AddScoped<IPlatformTenantService, PlatformTenantService>();
        services.AddScoped<IPlatformAnalyticsService, PlatformAnalyticsService>();
        services.AddScoped<IPlatformFeatureService, PlatformFeatureService>();
        services.AddScoped<IPlatformAuditService, PlatformAuditService>();
        services.AddScoped<IPlatformAnnouncementService, PlatformAnnouncementService>();

        var sp = services.BuildServiceProvider();

        return (sp, platformDb);
    }

    [Fact]
    public async Task PlatformTenantService_Should_Get_Tenant_Details()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var tenantService = sp.GetRequiredService<IPlatformTenantService>();
        var basicPlan = platformDb.SubscriptionPlans.First(p => p.Name == "Basic Plan");
        var tenant = await platformDb.Tenants.FirstAsync(t => t.SubscriptionPlanId == basicPlan.Id);

        var details = await tenantService.GetTenantDetailsAsync(tenant.Id);

        details.Should().NotBeNull();
        details.Name.Should().Be(tenant.Name);
        details.Slug.Should().Be(tenant.Slug);
        details.PlanName.Should().Be(basicPlan.Name);
    }

    [Fact]
    public async Task PlatformTenantService_Should_Suspend_Tenant()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var tenantService = sp.GetRequiredService<IPlatformTenantService>();
        var tenant = await platformDb.Tenants.FirstAsync(t => t.IsActive);

        var result = await tenantService.SuspendTenantAsync(tenant.Id, "Test suspend", "admin");

        result.Should().BeTrue();
        var updated = await platformDb.Tenants.FindAsync(tenant.Id);
        updated.Should().NotBeNull();
        updated!.SubscriptionStatus.Should().Be(SubscriptionStatus.Suspended);
        updated.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task PlatformTenantService_Should_Cancel_Tenant()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var tenantService = sp.GetRequiredService<IPlatformTenantService>();
        var tenant = await platformDb.Tenants.FirstAsync(t => t.IsActive);

        var result = await tenantService.CancelTenantAsync(tenant.Id, "Test cancel", "admin");

        result.Should().BeTrue();
        var updated = await platformDb.Tenants.FindAsync(tenant.Id);
        updated.Should().NotBeNull();
        updated!.SubscriptionStatus.Should().Be(SubscriptionStatus.Canceled);
        updated.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task PlatformTenantService_Should_Reactivate_Tenant()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var tenantService = sp.GetRequiredService<IPlatformTenantService>();
        var tenant = await platformDb.Tenants.FirstAsync(t => t.IsActive);

        await tenantService.SuspendTenantAsync(tenant.Id, "Test suspend", "admin");
        var result = await tenantService.ReactivateTenantAsync(tenant.Id, "Test reactivate", "admin");

        result.Should().BeTrue();
        var updated = await platformDb.Tenants.FindAsync(tenant.Id);
        updated.Should().NotBeNull();
        updated!.SubscriptionStatus.Should().Be(SubscriptionStatus.Active);
        updated.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task PlatformTenantService_Should_Search_Tenants_With_Filter()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var tenantService = sp.GetRequiredService<IPlatformTenantService>();

        var result = await tenantService.SearchTenantsAsync(new TenantSearchFilter(
            SearchTerm: "Alpha",
            Status: null,
            Tier: null,
            PlanCategory: null,
            IsActive: null,
            Page: 1,
            PageSize: 10
        ));

        result.Items.Should().NotBeEmpty();
        result.Items.Should().Contain(t => t.Name.Contains("Alpha"));
    }

    [Fact]
    public async Task PlatformAnalyticsService_Should_Return_Health_Metrics()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var analyticsService = sp.GetRequiredService<IPlatformAnalyticsService>();

        var health = await analyticsService.GetPlatformHealthAsync();

        health.Should().NotBeNull();
        health.TotalActiveTenants.Should().BeGreaterThanOrEqualTo(0);
        health.MeasuredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task PlatformAnalyticsService_Should_Return_Revenue_Analytics()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var analyticsService = sp.GetRequiredService<IPlatformAnalyticsService>();

        var revenue = await analyticsService.GetRevenueAnalyticsAsync();

        revenue.Should().NotBeNull();
        revenue.MonthlyRevenue.Should().BeGreaterThanOrEqualTo(0);
        revenue.RevenueByPlan.Should().NotBeNull();
    }

    [Fact]
    public async Task PlatformAnalyticsService_Should_Return_Growth_Metrics()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var analyticsService = sp.GetRequiredService<IPlatformAnalyticsService>();

        var growth = await analyticsService.GetTenantGrowthAsync();

        growth.Should().NotBeNull();
        growth.NewTenantsThisMonth.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task PlatformAnalyticsService_Should_Return_Subscription_Distribution()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var analyticsService = sp.GetRequiredService<IPlatformAnalyticsService>();

        var distribution = await analyticsService.GetSubscriptionDistributionAsync();

        distribution.Should().NotBeNull();
        distribution.BasicCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task PlatformFeatureService_Should_Get_All_Features()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var featureService = sp.GetRequiredService<IPlatformFeatureService>();

        var features = await featureService.GetAllFeatureFlagsAsync();

        features.Should().NotBeEmpty();
        features.Should().Contain(f => f.Code == "API_ACCESS");
    }

    [Fact]
    public async Task PlatformFeatureService_Should_Toggle_Global_Feature()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var featureService = sp.GetRequiredService<IPlatformFeatureService>();
        var feature = platformDb.PlatformFeatures.First();
        var originalValue = feature.IsEnabledGlobally;

        var result = await featureService.UpdateGlobalFeatureAsync(feature.Id, !originalValue, "admin");

        result.Should().BeTrue();
        var updated = await platformDb.PlatformFeatures.FindAsync(feature.Id);
        updated.Should().NotBeNull();
        updated!.IsEnabledGlobally.Should().Be(!originalValue);
    }

    [Fact]
    public async Task PlatformFeatureService_Should_Set_Tenant_Override()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var featureService = sp.GetRequiredService<IPlatformFeatureService>();
        var tenant = await platformDb.Tenants.FirstAsync();
        var feature = platformDb.PlatformFeatures.First();

        var result = await featureService.SetTenantFeatureOverrideAsync(tenant.Id, feature.Id, false, "admin");

        result.Should().BeTrue();
        var isEnabled = await featureService.IsFeatureEnabledForTenantAsync(tenant.Id, feature.Code);
        isEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task PlatformAuditService_Should_Log_And_Retrieve_Audit()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var auditService = sp.GetRequiredService<IPlatformAuditService>();
        var tenant = await platformDb.Tenants.FirstAsync();

        await auditService.LogAuditAsync(tenant.Id, tenant.Slug, AuditAction.Created, "Tenant", tenant.Id.ToString(), null, "created", "admin", true);

        var logs = await auditService.GetTenantAuditLogAsync(tenant.Id);

        logs.Should().NotBeEmpty();
        logs.Should().Contain(l => l.Action == AuditAction.Created && l.PerformedByUsername == "admin");
    }

    [Fact]
    public async Task PlatformAnnouncementService_Should_Create_And_Get_Announcements()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var announcementService = sp.GetRequiredService<IPlatformAnnouncementService>();

        var announcement = await announcementService.CreateAnnouncementAsync(
            "Test Announcement",
            "Content",
            AnnouncementPriority.Normal,
            AnnouncementScope.All,
            null,
            null,
            Guid.NewGuid(),
            "admin"
        );

        announcement.Should().NotBeNull();
        announcement.Title.Should().Be("Test Announcement");

        var announcements = await announcementService.GetActiveAnnouncementsAsync();
        announcements.Should().Contain(a => a.Title == "Test Announcement");
    }

    [Fact]
    public async Task PlatformAnnouncementService_Should_Deactivate_Announcement()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var announcementService = sp.GetRequiredService<IPlatformAnnouncementService>();

        var announcement = await announcementService.CreateAnnouncementAsync("To Deactivate", "Content", AnnouncementPriority.Low, AnnouncementScope.All, null, null, Guid.NewGuid(), "admin");
        var result = await announcementService.DeactivateAnnouncementAsync(announcement.Id);

        result.Should().BeTrue();
        var activeAnnouncements = await announcementService.GetActiveAnnouncementsAsync();
        activeAnnouncements.Should().NotContain(a => a.Id == announcement.Id);
    }

    [Fact]
    public async Task PlatformFeatureService_Should_Remove_Tenant_Override()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var featureService = sp.GetRequiredService<IPlatformFeatureService>();
        var tenant = await platformDb.Tenants.FirstAsync();
        var feature = platformDb.PlatformFeatures.First(f => f.IsEnabledGlobally);

        await featureService.SetTenantFeatureOverrideAsync(tenant.Id, feature.Id, false, "admin");
        var result = await featureService.RemoveTenantFeatureOverrideAsync(tenant.Id, feature.Id, "admin");

        result.Should().BeTrue();
        var isEnabled = await featureService.IsFeatureEnabledForTenantAsync(tenant.Id, feature.Code);
        isEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task PlatformTenantService_Should_Update_Tenant()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var tenantService = sp.GetRequiredService<IPlatformTenantService>();
        var tenant = await platformDb.Tenants.FirstAsync();

        var dto = new UpdateTenantDto("Updated Name", false, null);
        var result = await tenantService.UpdateTenantAsync(tenant.Id, dto, "admin");

        result.Should().BeTrue();
        var updated = await platformDb.Tenants.FindAsync(tenant.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Name");
        updated.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task PlatformAuditService_Should_Get_Platform_Audit_Log()
    {
        var (sp, platformDb) = await GetPlatformServiceProviderAsync();
        var auditService = sp.GetRequiredService<IPlatformAuditService>();
        var tenant = await platformDb.Tenants.FirstAsync();

        await auditService.LogAuditAsync(tenant.Id, tenant.Slug, AuditAction.Updated, "Tenant", tenant.Id.ToString(), null, "updated", "admin", true);

        var logs = await auditService.GetPlatformAuditLogAsync();

        logs.Should().NotBeEmpty();
        logs.Should().Contain(l => l.Action == AuditAction.Updated);
    }

    [Fact]
    public async Task Validators_Should_Validate_Correctly()
    {
        var createValidator = new CreateTenantDtoValidator();
        var validCreate = new CreateTenantDto("Valid Name", "valid-slug", Guid.NewGuid(), IsolationTier.Isolated, "test@example.com");
        var result = await createValidator.ValidateAsync(validCreate);
        result.IsValid.Should().BeTrue();

        var invalidCreate = new CreateTenantDto("", "", Guid.Empty, IsolationTier.Isolated, "invalid-email");
        result = await createValidator.ValidateAsync(invalidCreate);
        result.IsValid.Should().BeFalse();
    }
}
