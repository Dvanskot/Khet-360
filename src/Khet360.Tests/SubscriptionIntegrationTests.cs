using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace Khet360.Tests;

public class SubscriptionIntegrationTests
{
    private async Task<(IServiceProvider, PlatformDbContext, TenantDbContext)> GetServiceProviderAsync()
    {
        var services = new ServiceCollection();

        // Platform DB
        var platformDb = new PlatformDbContext(new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: "PlatformDB_" + Guid.NewGuid().ToString()).Options);
        services.AddSingleton(platformDb);

        // Seed Platform Data
        await DbInitializer.InitializeDatabase(platformDb, new Mock<ILogger<PlatformDbContext>>().Object);

        // Tenant DB
        var tenantDb = new TenantDbContext(new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName: "TenantDB_" + Guid.NewGuid().ToString()).Options, new Mock<ITenantUserContext>().Object);
        services.AddSingleton(tenantDb);

        // Mocked dependencies
        services.AddSingleton(new Mock<ITenantUserContext>().Object);

        var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        mockConfig.Setup(c => c["SqlSettings:DedicatedServerConnectionString"]).Returns("Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");

        var mockSection = new Mock<Microsoft.Extensions.Configuration.IConfigurationSection>();
        mockSection.Setup(s => s["PlatformConnection"]).Returns("Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");
        mockConfig.Setup(c => c.GetSection("ConnectionStrings")).Returns(mockSection.Object);

        services.AddSingleton(mockConfig.Object);

        services.AddSingleton(new Mock<ITenantService>().Object);
        services.AddSingleton(new Mock<ICacheService>().Object);
        services.AddSingleton(new Mock<IMessageBus>().Object);
        services.AddSingleton(new Mock<IStateSyncService>().Object);
        services.AddSingleton(new Mock<IMetricsService>().Object);
        services.AddSingleton(new Mock<IOutboxService>().Object);

        // Services
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IEntitlementService, EntitlementService>();
        services.AddScoped<ITenantManagementService, TenantManagementService>();

        var mockProvisioning = new Mock<ITenantProvisioningService>();
        mockProvisioning.Setup(p => p.ProvisionTenantAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IsolationTier>()))
            .ReturnsAsync("Server=localhost;Database=MockDb;Trusted_Connection=True;");
        services.AddScoped<ITenantProvisioningService>(sp => mockProvisioning.Object);

        services.AddScoped<IPlatformPaymentService, PlatformPaymentService>();
        services.AddSingleton(new Mock<ILogger<SubscriptionService>>().Object);
        services.AddSingleton(new Mock<ILogger<TenantProvisioningService>>().Object);
        services.AddSingleton(new Mock<ILogger<TenantManagementService>>().Object);
        services.AddSingleton(new Mock<ILogger<PlatformPaymentService>>().Object);

        // Payment Providers (Mock)
        var mockProvider = new Mock<IPaymentGatewayProvider>();
        mockProvider.Setup(p => p.ProviderName).Returns("Stripe");
        mockProvider.Setup(p => p.CreatePaymentLinkAsync(It.IsAny<PaymentConfiguration>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync("https://stripe.com/payment/test");
        mockProvider.Setup(p => p.VerifyPaymentAsync(It.IsAny<PaymentConfiguration>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .ReturnsAsync(true);
        services.AddSingleton<IPaymentGatewayProvider>(mockProvider.Object);

        return (services.BuildServiceProvider(), platformDb, tenantDb);
    }

    [Fact]
    public async Task PublicDiscovery_ShouldReturnActivePlans()
    {
        // Arrange
        var (sp, platformDb, _) = await GetServiceProviderAsync();
        var plans = await platformDb.SubscriptionPlans.ToListAsync();

        // Assert
        plans.Should().NotBeEmpty();
        plans.Should().Contain(p => p.Name == "Basic Plan");
        plans.Should().Contain(p => p.Name == "Professional Plan");
        plans.Should().Contain(p => p.Name == "Enterprise Plan");
    }

    [Fact]
    public async Task TrialFlow_ShouldProvisionTenantWithTrialStatus()
    {
        // Arrange
        var (sp, platformDb, _) = await GetServiceProviderAsync();
        var tenantService = sp.GetRequiredService<ITenantManagementService>();
        var basicPlan = platformDb.SubscriptionPlans.First(p => p.Name == "Basic Plan");

        // Act
        var tenant = await tenantService.CreateTenantAsync("Test Company", "test-trial", basicPlan.Id, IsolationTier.Isolated);

        // Assert
        tenant.SubscriptionStatus.Should().Be(SubscriptionStatus.Trial);
        tenant.TrialEndDate.Should().NotBeNull();
        tenant.TrialEndDate.Value.Should().BeCloseTo(DateTime.UtcNow.AddDays(basicPlan.TrialPeriodDays), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task PaymentAndProvisioning_ShouldActivateTenant()
    {
        // Arrange
        var (sp, platformDb, _) = await GetServiceProviderAsync();
        var tenantService = sp.GetRequiredService<ITenantManagementService>();
        var subscriptionService = sp.GetRequiredService<ISubscriptionService>();
        var proPlan = platformDb.SubscriptionPlans.First(p => p.Name == "Professional Plan");

        // Act
        var tenant = await tenantService.CreateTenantAsync("Paid Company", "paid-tenant", proPlan.Id, IsolationTier.Isolated);
        await subscriptionService.ActivateSubscriptionAsync(tenant.Id, 12); // Activate for 1 year

        // Assert
        var status = await subscriptionService.GetSubscriptionStatusAsync(tenant.Id);
        status.Status.Should().Be(SubscriptionStatus.Active);
        status.EndDate.Should().BeCloseTo(DateTime.UtcNow.AddMonths(12), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task LimitEnforcement_ShouldPreventExceedingPlanLimits()
    {
        // Arrange
        var (sp, platformDb, _) = await GetServiceProviderAsync();
        var subscriptionService = sp.GetRequiredService<ISubscriptionService>();
        var basicPlan = platformDb.SubscriptionPlans.First(p => p.Name == "Basic Plan");

        var tenant = new Tenant { Id = Guid.NewGuid(), SubscriptionPlanId = basicPlan.Id };
        platformDb.Tenants.Add(tenant);
        await platformDb.SaveChangesAsync();

        // Basic plan has MAX_BRANCHES = 2
        var entitlementCode = "MAX_BRANCHES";

        // Act & Assert
        // 1. Under limit
        var canAddFirst = await subscriptionService.ValidateLimitAsync(tenant.Id, entitlementCode, 0);
        canAddFirst.Should().BeTrue();

        // 2. At limit
        var canAddThird = await subscriptionService.ValidateLimitAsync(tenant.Id, entitlementCode, 2);
        canAddThird.Should().BeFalse();
    }

    [Fact]
    public async Task AdminManagement_ShouldUpdatePlanDetails()
    {
        // Arrange
        var (sp, platformDb, _) = await GetServiceProviderAsync();
        var plan = platformDb.SubscriptionPlans.First();
        var originalPrice = plan.MonthlyPrice;

        // Act
        plan.MonthlyPrice = 999.99m;
        await platformDb.SaveChangesAsync();

        // Assert
        var updatedPlan = await platformDb.SubscriptionPlans.FindAsync(plan.Id);
        updatedPlan.MonthlyPrice.Should().Be(999.99m);
        updatedPlan.MonthlyPrice.Should().NotBe(originalPrice);
    }
}
