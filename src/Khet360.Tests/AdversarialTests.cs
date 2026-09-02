using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Khet360.Tests;

public class AdversarialTests
{
    private TenantDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        var mockUserContext = new Mock<ITenantUserContext>();
        return new TenantDbContext(options, mockUserContext.Object);
    }

    private async Task<IServiceProvider> GetServiceProviderAsync(TenantDbContext db)
    {
        var services = new ServiceCollection();
        services.AddSingleton(db);

        // Platform DB for Tax Service
        var platformOptions = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: "PlatformDB_" + Guid.NewGuid().ToString())
            .Options;
        var platformDb = new PlatformDbContext(platformOptions);
        await TestDataSeeder.SeedPlatformTaxData(platformDb);
        services.AddSingleton(platformDb);

        var mockUserContext = new Mock<ITenantUserContext>();
        mockUserContext.Setup(uc => uc.IsAuthenticated).Returns(true);
        mockUserContext.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        services.AddSingleton(mockUserContext.Object);

        var mockTenantService = new Mock<ITenantService>();
        var mockTenant = new Tenant { Id = Guid.NewGuid(), Slug = "test-tenant" };
        mockTenantService.Setup(ts => ts.CurrentTenant).Returns(mockTenant);
        services.AddSingleton(mockTenantService.Object);

        services.AddSingleton(new Mock<ICacheService>().Object);
        services.AddSingleton(new Mock<IMessageBus>().Object);
        services.AddSingleton(new Mock<IStateSyncService>().Object);
        services.AddSingleton(new Mock<IMetricsService>().Object);
        services.AddSingleton(new Mock<IOutboxService>().Object);
        services.AddScoped<IWorkItemService, WorkItemService>();
        services.AddScoped<IRoutingService, RoutingService>();
        services.AddSingleton(new Mock<ILogger<LeadService>>().Object);
        services.AddSingleton(new Mock<ILogger<OpportunityService>>().Object);
        services.AddSingleton(new Mock<ILogger<CustomerService>>().Object);
        services.AddSingleton(new Mock<ILogger<FuneralCaseService>>().Object);
        services.AddSingleton(new Mock<ILogger<ProductionService>>().Object);
        services.AddSingleton(new Mock<ILogger<FinanceVerificationService>>().Object);
        services.AddSingleton(new Mock<ILogger<EmployeeService>>().Object);
        services.AddSingleton(new Mock<ILogger<PayrollService>>().Object);
        services.AddSingleton(new Mock<ILogger<ServiceArrangementService>>().Object);
        services.AddSingleton(new Mock<ILogger<PaymentService>>().Object);

        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<IOpportunityService, OpportunityService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IFuneralCaseService, FuneralCaseService>();
        services.AddScoped<IProductionService, ProductionService>();
        services.AddScoped<IFinanceVerificationService, FinanceVerificationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IServiceArrangementService, ServiceArrangementService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPolicyService, PolicyService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<ITaxService, TaxService>();
        services.AddScoped<IFinancialService, FinancialService>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Payroll_Should_Fail_When_Employee_Has_No_PayProfile()
    {
        // Arrange
        var db = GetDbContext();
        var sp = await GetServiceProviderAsync(db);
        var payrollService = sp.GetRequiredService<IPayrollService>();

        // Setup BASIC pay item
        db.PayItems.Add(new PayItem { Id = Guid.NewGuid(), Name = "Basic Salary", Code = "BASIC", Type = PayItemType.Earning, IsStatutory = false });

        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            FirstName = "No",
            LastName = "Profile",
            EmployeeCode = "EMP-NO-PROFILE",
            DepartmentId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            BranchId = Guid.NewGuid()
        };
        db.Employees.Add(employee);

        db.EmploymentContracts.Add(new EmploymentContract
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Salary = 5000m
        });

        await db.SaveChangesAsync();

        var runDto = new PayrollRunCreateDto(
            PeriodName: "Adversarial Run",
            StartDate: new DateTime(2026, 9, 1),
            EndDate: new DateTime(2026, 9, 30)
        );
        var runId = await payrollService.CreatePayrollRunAsync(runDto);

        // Act
        Func<Task> act = async () => await payrollService.CalculatePayrollAsync(runId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*pay profile*");
    }

    [Fact]
    public async Task Financials_Should_Fail_When_Transaction_Is_Unbalanced()
    {
        // Arrange
        var db = GetDbContext();
        var sp = await GetServiceProviderAsync(db);
        var financeService = sp.GetRequiredService<IFinanceVerificationService>();

        var transaction = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            Description = "Unbalanced TX",
            Entries = new List<FinancialEntry> {
                new() { Id = Guid.NewGuid(), AccountCode = "1001", Debit = 1000m, Credit = 0 },
                new() { Id = Guid.NewGuid(), AccountCode = "4001", Debit = 0, Credit = 500m } // 1000 != 500
            }
        };
        db.FinancialTransactions.Add(transaction);
        await db.SaveChangesAsync();

        // Act
        var verification = await financeService.VerifyInvariantsAsync();

        // Assert
        verification.IsBalanced.Should().BeFalse();
    }
}
