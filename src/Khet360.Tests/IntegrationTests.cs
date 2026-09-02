using System;
using System.Collections.Generic;
using System.Linq;
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

public class IntegrationTests
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

    private IServiceProvider GetServiceProvider(TenantDbContext db)
    {
        var services = new ServiceCollection();
        services.AddSingleton(db);

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
        services.AddSingleton(new Mock<IWorkItemService>().Object);
        services.AddSingleton(new Mock<ILogger<LeadService>>().Object);
        services.AddSingleton(new Mock<ILogger<OpportunityService>>().Object);
        services.AddSingleton(new Mock<ILogger<CustomerService>>().Object);
        services.AddSingleton(new Mock<ILogger<FuneralCaseService>>().Object);
        services.AddSingleton(new Mock<ILogger<ProductionService>>().Object);
        services.AddSingleton(new Mock<ILogger<FinanceVerificationService>>().Object);
        services.AddSingleton(new Mock<ILogger<EmployeeService>>().Object);
        services.AddSingleton(new Mock<ILogger<PayrollService>>().Object);

        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<IOpportunityService, OpportunityService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IFuneralCaseService, FuneralCaseService>();
        services.AddScoped<IProductionService, ProductionService>();
        services.AddScoped<IFinanceVerificationService, FinanceVerificationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPayrollService, PayrollService>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task LeadToCase_GoldenPath_Should_Complete_Successfully()
    {
        // Arrange
        var db = GetDbContext();
        var sp = GetServiceProvider(db);
        var leadService = sp.GetRequiredService<ILeadService>();
        var caseService = sp.GetRequiredService<IFuneralCaseService>();
        var branchId = Guid.NewGuid();

        // Setup branch in DB
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });
        await db.SaveChangesAsync();

        // 1. Lead Intake
        var lead = new Lead {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "123-456-7890",
            Status = LeadStatus.New,
            BranchId = branchId
        };
        db.Leads.Add(lead);
        await db.SaveChangesAsync();

        // 2. Convert Lead to Customer and Opportunity
        var conversionDto = new LeadConversionDto(
            CreateOpportunity: true,
            CreateCustomer: true,
            CustomerType: "Individual",
            EstimatedValue: 5000,
            OpportunityName: "Standard Funeral Package"
        );

        var customerId = await leadService.ConvertLeadAsync(lead.Id, conversionDto);

        // 3. Open Funeral Case for Customer
        var caseId = await caseService.OpenCaseAsync(customerId, null, branchId);

        // Assert
        var createdCase = await db.FuneralCases.FindAsync(caseId);
        createdCase.Should().NotBeNull();
        createdCase.CustomerId.Should().Be(customerId);
        createdCase.BranchId.Should().Be(branchId);

        var customer = await db.Customers.FindAsync(customerId);
        customer.Should().NotBeNull();
        customer.FullName.Should().Contain("John");
    }

    [Fact]
    public async Task ProductionToFinance_GoldenPath_Should_Complete_Successfully()
    {
        // Arrange
        var db = GetDbContext();
        var sp = GetServiceProvider(db);
        var prodService = sp.GetRequiredService<IProductionService>();
        var financeService = sp.GetRequiredService<IFinanceVerificationService>();

        var memorialId = Guid.NewGuid();
        var artisanId = Guid.NewGuid();
        var inspectorId = Guid.NewGuid();

        // Setup Artisan and Inspector as Employees
        db.Employees.Add(new Employee { Id = artisanId, FirstName = "Artisan", LastName = "One", BranchId = Guid.NewGuid(), DepartmentId = Guid.NewGuid(), PositionId = Guid.NewGuid() });
        db.Employees.Add(new Employee { Id = inspectorId, FirstName = "Inspector", LastName = "One", BranchId = Guid.NewGuid(), DepartmentId = Guid.NewGuid(), PositionId = Guid.NewGuid() });
        await db.SaveChangesAsync();

        // 1. Create Production Order
        var orderId = await prodService.CreateProductionOrderAsync(memorialId);

        // 2. Advance through stages to Quality Check
        var order = await db.ProductionOrders.FindAsync(orderId);
        order.Should().NotBeNull();
        order!.CurrentStage = ProductionStage.QualityCheck;
        await db.SaveChangesAsync();

        // 3. Perform Quality Check (Pass)
        await prodService.PerformQualityCheckAsync(orderId, inspectorId, passed: true, comments: "Perfectly crafted");

        // Assert Production
        var finalOrder = await db.ProductionOrders.FindAsync(orderId);
        finalOrder.Should().NotBeNull();
        finalOrder!.Status.Should().Be(ProductionStatus.Completed);
        finalOrder.CurrentStage.Should().Be(ProductionStage.ReadyForDelivery);

        // 4. Simulate Financial Entry for the order
        var txId = Guid.NewGuid();
        var transaction = new FinancialTransaction {
            Id = txId,
            Description = $"Payment for Memorial {memorialId}",
            Entries = new List<FinancialEntry> {
                new() { Id = Guid.NewGuid(), AccountCode = "1001", Debit = 1000m, Credit = 0 },
                new() { Id = Guid.NewGuid(), AccountCode = "4001", Debit = 0, Credit = 1000m }
            }
        };
        db.FinancialTransactions.Add(transaction);
        await db.SaveChangesAsync();

        // 5. Verify Financial Invariants
        var verification = await financeService.VerifyInvariantsAsync();
        verification.IsBalanced.Should().BeTrue();
    }

    [Fact]
    public async Task PeopleToPayroll_GoldenPath_Should_Complete_Successfully()
    {
        // Arrange
        var db = GetDbContext();
        var sp = GetServiceProvider(db);
        var empService = sp.GetRequiredService<IEmployeeService>();
        var payrollService = sp.GetRequiredService<IPayrollService>();
        var branchId = Guid.NewGuid();
        var deptId = Guid.NewGuid();
        var posId = Guid.NewGuid();

        // Setup prerequisites
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });
        db.Departments.Add(new Department { Id = deptId, Name = "HR", BranchId = branchId });
        db.Positions.Add(new Position { Id = posId, Title = "HR Manager" });
        await db.SaveChangesAsync();

        // 1. Hire Employee
        var empDto = new EmployeeCreateDto(
            UserId: Guid.NewGuid(),
            FirstName: "Jane",
            LastName: "Smith",
            Email: "jane.smith@example.com",
            PhoneNumber: "555-0123",
            EmployeeCode: "EMP001",
            DepartmentId: deptId,
            PositionId: posId,
            BranchId: branchId,
            ManagerId: null,
            HireDate: DateTime.UtcNow,
            EmergencyContactName: "Relative",
            EmergencyContactPhone: "555-9999",
            Qualifications: "BSc HR",
            Salary: 50000m,
            ContractType: "FullTime"
        );
        var employeeId = await empService.CreateEmployeeAsync(empDto);

        // 2. Create Pay Profile
        var payProfileDto = new PayProfileCreateDto(
            EmployeeId: employeeId,
            BankName: "Standard Bank",
            AccountNumber: "123456789",
            BranchCode: "123456",
            TaxNumber: "TAX123",
            TaxBracket: "Standard"
        );
        await payrollService.CreatePayProfileAsync(payProfileDto);

        // Setup Basic Pay Item
        db.PayItems.Add(new PayItem {
            Id = Guid.NewGuid(),
            Name = "Basic Salary",
            Code = "BASIC",
            Type = PayItemType.Earning,
            IsStatutory = false
        });
        await db.SaveChangesAsync();

        // 3. Execute Payroll Run
        var runDto = new PayrollRunCreateDto(
            PeriodName: "September 2026 Run",
            StartDate: new DateTime(2026, 9, 1),
            EndDate: new DateTime(2026, 9, 30)
        );
        var runId = await payrollService.CreatePayrollRunAsync(runDto);
        await payrollService.CalculatePayrollAsync(runId);
        await payrollService.FinalizePayrollRunAsync(runId, Guid.NewGuid());

        // Assert
        var payslip = await payrollService.GetPayslipAsync(employeeId, runId);
        payslip.Should().NotBeNull();
        payslip.EmployeeId.Should().Be(employeeId);
        payslip.PayrollRunId.Should().Be(runId);
        payslip.NetPay.Should().BeGreaterThan(0);
    }
}
