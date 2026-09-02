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
    private TenantDbContext GetDbContext(ITenantUserContext userContext)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new TenantDbContext(options, userContext);
    }

    private (IServiceProvider, TenantDbContext, List<Guid>) GetServiceProvider()
    {
        var mockUserContext = new Mock<ITenantUserContext>();
        mockUserContext.Setup(uc => uc.IsAuthenticated).Returns(true);
        mockUserContext.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        var assignedBranches = new List<Guid>();
        mockUserContext.Setup(uc => uc.AssignedBranchIds).Returns(assignedBranches);

        var db = GetDbContext(mockUserContext.Object);

        var services = new ServiceCollection();
        services.AddSingleton(db);
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
        services.AddSingleton(new Mock<ILogger<InventoryService>>().Object);
        services.AddSingleton(new Mock<ILogger<POSService>>().Object);
        services.AddSingleton(new Mock<ILogger<NotificationService>>().Object);
        services.AddSingleton(new Mock<ILogger<ClaimService>>().Object);
        services.AddSingleton(new Mock<ILogger<VendorService>>().Object);

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
        services.AddScoped<IRoutingService, RoutingService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IPOSService, POSService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IFinancialService, FinancialService>();

        return (services.BuildServiceProvider(), db, assignedBranches);
    }

    [Fact]
    public async Task Ultimate_Funeral_Lifecycle_GoldenPath_Should_Complete_Successfully()
    {
        // Arrange
        var (sp, db, assignedBranches) = GetServiceProvider();
        var leadService = sp.GetRequiredService<ILeadService>();
        var caseService = sp.GetRequiredService<IFuneralCaseService>();
        var arrangementService = sp.GetRequiredService<IServiceArrangementService>();
        var prodService = sp.GetRequiredService<IProductionService>();
        var financeService = sp.GetRequiredService<IFinanceVerificationService>();
        var paymentService = sp.GetRequiredService<IPaymentService>();

        var branchId = Guid.NewGuid();
        // Add branch to mock user context to bypass global filter
        assignedBranches.Add(branchId);
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });
        await db.SaveChangesAsync();

        // 1. Lead Intake
        var lead = new Lead {
            Id = Guid.NewGuid(),
            FirstName = "Ultimate",
            LastName = "Test",
            Email = "ultimate@test.com",
            Phone = "123-456-7890",
            Status = LeadStatus.New,
            BranchId = branchId
        };
        db.Leads.Add(lead);
        await db.SaveChangesAsync();

        // 2. Convert Lead to Customer & Opportunity
        var conversionDto = new LeadConversionDto(true, true, "Individual", 10000, "Premium Package");
        var customerId = await leadService.ConvertLeadAsync(lead.Id, conversionDto);

        // 3. Open Funeral Case
        var caseId = await caseService.OpenCaseAsync(customerId, null, branchId);

        // 4. Create Arrangement
        var arrangementDto = new ServiceArrangementCreateDto(
            ArrangementName: "Standard Arrangement",
            ScheduledDate: DateTime.UtcNow.AddDays(7),
            Location: "City Cemetery",
            Type: ArrangementType.Burial,
            Description: "Standard package arrangement",
            HasCatering: true,
            ExpectedGuestCount: 100,
            CateringNotes: "Standard catering",
            FuneralCaseId: caseId,
            Items: new List<ArrangementItemCreateDto> {
                new("Casket", "Premium Wood", 2000m, 1, false)
            }
        );
        var arrangementId = await arrangementService.CreateArrangementAsync(arrangementDto, branchId);

        // 5. Production (Memorial)
        var memorialId = Guid.NewGuid();
        var orderId = await prodService.CreateProductionOrderAsync(memorialId);

        var order = await db.ProductionOrders.FindAsync(orderId);
        order!.CurrentStage = ProductionStage.QualityCheck;
        await db.SaveChangesAsync();

        var inspectorId = Guid.NewGuid();
        db.Employees.Add(new Employee { Id = inspectorId, FirstName = "Inspector", LastName = "One", BranchId = branchId, DepartmentId = Guid.NewGuid(), PositionId = Guid.NewGuid() });
        await db.SaveChangesAsync();

        await prodService.PerformQualityCheckAsync(orderId, inspectorId, passed: true, comments: "Passed");

        // 6. Financials
        var txId = Guid.NewGuid();
        var transaction = new FinancialTransaction {
            Id = txId,
            Description = "Final Payment",
            Entries = new List<FinancialEntry> {
                new() { Id = Guid.NewGuid(), AccountCode = "1001", Debit = 5000m, Credit = 0 },
                new() { Id = Guid.NewGuid(), AccountCode = "4001", Debit = 0, Credit = 5000m }
            }
        };
        db.FinancialTransactions.Add(transaction);
        await db.SaveChangesAsync();

        // 7. Verify Invariants
        var verification = await financeService.VerifyInvariantsAsync();

        // Assert
        verification.IsBalanced.Should().BeTrue();

        var finalOrder = await db.ProductionOrders.FindAsync(orderId);
        finalOrder!.Status.Should().Be(ProductionStatus.Completed);

        var createdCase = await db.FuneralCases.FindAsync(caseId);
        createdCase.Should().NotBeNull();
    }

    [Fact]
    public async Task InsuranceClaim_GoldenPath_Should_Complete_Successfully()
    {
        // Arrange
        var (sp, db, assignedBranches) = GetServiceProvider();
        var policyService = sp.GetRequiredService<IPolicyService>();
        var claimService = sp.GetRequiredService<IClaimService>();
        var branchId = Guid.NewGuid();

        // Add branch to mock user context to bypass global filter
        assignedBranches.Add(branchId);
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });
        await db.SaveChangesAsync();

        // Setup Burial Plan and Products
        var product = new FuneralProduct { Id = Guid.NewGuid(), Name = "Premium Casket", DefaultPrice = 2000m, Description = "High-end wood casket" };
        db.FuneralProducts.Add(product);

        var plan = new InsurancePolicyPlan {
            Id = Guid.NewGuid(),
            Name = "Premium Burial Plan",
            PremiumAmount = 100m,
            WaitingPeriodMonths = 1,
            CoverType = InsuranceCoverType.Burial
        };
        db.InsurancePolicyPlans.Add(plan);

        var benefit = new InsurancePolicyPlanBenefit {
            Id = Guid.NewGuid(),
            PolicyPlanId = plan.Id,
            Role = MemberRole.Main,
            IsFixed = true,
            CoverAmount = 15000m
        };
        db.InsurancePolicyPlanBenefits.Add(benefit);

        db.InsurancePolicyPlanBenefitItems.Add(new InsurancePolicyPlanBenefitItem {
            BenefitId = benefit.Id,
            FuneralProductId = product.Id,
            Quantity = 1
        });

        await db.SaveChangesAsync();

        var customer = new IndividualCustomer {
            Id = Guid.NewGuid(),
            FirstName = "Insured",
            LastName = "User",
            BranchId = branchId,
            IdentityNumber = "ID12345"
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var funeralCase = new FuneralCase {
            Id = Guid.NewGuid(),
            CaseNumber = "CLAIM-CASE-123",
            CustomerId = customer.Id,
            DeceasedCustomerId = customer.Id,
            BranchId = branchId
        };
        db.FuneralCases.Add(funeralCase);
        await db.SaveChangesAsync();

        // 1. Link Customer to Policy
        var policyDto = new PolicyCreateDto(
            PolicyNumber: "POL-123",
            ProviderName: "SafeLife Insurance",
            CoverageAmount: 15000m,
            StartDate: DateTime.UtcNow.AddMonths(-1),
            EndDate: null,
            PolicyPlanId: plan.Id,
            Members: new List<PolicyMemberCreateDto>
            {
                new PolicyMemberCreateDto(customer.Id, MemberRole.Main)
            }
        );
        var policyId = await policyService.CreatePolicyAsync(policyDto, branchId);

        // 2. Submit Claim
        var claimDto = new ClaimCreateDto(
            ClaimNumber: "CLM-999",
            ClaimAmount: 5000m,
            PolicyId: policyId,
            FuneralCaseId: funeralCase.Id,
            Notes: "Standard claim for funeral expenses"
        );
        var claimId = await claimService.CreateClaimAsync(claimDto, branchId);

        // 3. Process Claim (Update Status to Approved)
        await claimService.UpdateClaimStatusAsync(claimId, new ClaimUpdateDto(
            Status: ClaimStatus.UnderReview,
            ProcessedAt: DateTime.UtcNow,
            Notes: "Reviewing documents"));

        await claimService.UpdateClaimStatusAsync(claimId, new ClaimUpdateDto(
            Status: ClaimStatus.Approved,
            ProcessedAt: DateTime.UtcNow,
            Notes: "Documents verified"));

        // 4. Execute Payout
        await claimService.ExecutePayoutAsync(claimId);

        // Assert
        var finalClaim = await db.InsuranceClaims.FindAsync(claimId);
        finalClaim!.Status.Should().Be(ClaimStatus.Paid);

        // Verify Burial Items were added to the case
        var arrangement = await db.ServiceArrangements
            .FirstOrDefaultAsync(a => a.FuneralCaseId == funeralCase.Id);
        arrangement.Should().NotBeNull();

        var arrangementItems = await db.ArrangementItems
            .Where(ai => ai.ServiceArrangementId == arrangement!.Id)
            .ToListAsync();
        arrangementItems.Should().Contain(ai => ai.ItemName == "Premium Casket");
    }

    [Fact]
    public async Task OperationalExcellence_GoldenPath_Should_Complete_Successfully()
    {
        // Arrange
        var (sp, db, assignedBranches) = GetServiceProvider();
        var workItemService = sp.GetRequiredService<IWorkItemService>();
        var branchId = Guid.NewGuid();

        // Add branch to mock user context to bypass global filter
        assignedBranches.Add(branchId);
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });
        await db.SaveChangesAsync();

        // 1. Work Item Trigger (Simulate a case milestone creating a task)
        var entityId = Guid.NewGuid();
        var workItemId = await workItemService.CreateWorkItemAsync(
            entityType: "FuneralCase",
            entityId: entityId,
            nextAction: "Verify Death Certificate",
            priority: WorkItemPriority.High,
            dueDate: DateTime.UtcNow.AddHours(2), // Set to be close to SLA breach
            branchId: branchId
        );

        // 2. Assignment
        var userId = Guid.NewGuid();
        await workItemService.AssignWorkItemAsync(workItemId, userId);

        // 3. Resolution
        await workItemService.CompleteWorkItemAsync(workItemId, "Certificate verified and uploaded");

        // Assert
        var workItem = await db.WorkItems.FindAsync(workItemId);
        workItem.Should().NotBeNull();
        workItem!.Status.Should().Be(WorkItemStatus.Completed);
    }

    [Fact]
    public async Task LeadToCase_GoldenPath_Should_Complete_Successfully()
    {
        // Arrange
        var (sp, db, assignedBranches) = GetServiceProvider();
        var leadService = sp.GetRequiredService<ILeadService>();
        var caseService = sp.GetRequiredService<IFuneralCaseService>();
        var branchId = Guid.NewGuid();

        // Add branch to mock user context to bypass global filter
        assignedBranches.Add(branchId);
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
        var (sp, db, assignedBranches) = GetServiceProvider();
        var prodService = sp.GetRequiredService<IProductionService>();
        var financeService = sp.GetRequiredService<IFinanceVerificationService>();
        var branchId = Guid.NewGuid();

        // Add branch to mock user context to bypass global filter
        assignedBranches.Add(branchId);
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });
        await db.SaveChangesAsync();

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
        var (sp, db, assignedBranches) = GetServiceProvider();
        var empService = sp.GetRequiredService<IEmployeeService>();
        var payrollService = sp.GetRequiredService<IPayrollService>();
        var branchId = Guid.NewGuid();
        var deptId = Guid.NewGuid();
        var posId = Guid.NewGuid();

        // Add branch to mock user context to bypass global filter
        assignedBranches.Add(branchId);
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

    [Fact]
    public async Task InsuranceClaim_CashPayout_Success()
    {
        // Arrange
        var (sp, db, assignedBranches) = GetServiceProvider();
        var policyService = sp.GetRequiredService<IPolicyService>();
        var claimService = sp.GetRequiredService<IClaimService>();
        var branchId = Guid.NewGuid();

        // Add branch to mock user context to bypass global filter
        assignedBranches.Add(branchId);
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });
        await db.SaveChangesAsync();

        var plan = new InsurancePolicyPlan {
            Id = Guid.NewGuid(),
            Name = "Cash Payout Plan",
            PremiumAmount = 50m,
            WaitingPeriodMonths = 1,
            CoverType = InsuranceCoverType.Cash
        };
        db.InsurancePolicyPlans.Add(plan);

        var benefit = new InsurancePolicyPlanBenefit {
            Id = Guid.NewGuid(),
            PolicyPlanId = plan.Id,
            Role = MemberRole.Main,
            IsFixed = true,
            CoverAmount = 10000m
        };
        db.InsurancePolicyPlanBenefits.Add(benefit);

        await db.SaveChangesAsync();

        var customer = new IndividualCustomer {
            Id = Guid.NewGuid(),
            FirstName = "Cash",
            LastName = "User",
            BranchId = branchId,
            IdentityNumber = "ID54321"
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var funeralCase = new FuneralCase {
            Id = Guid.NewGuid(),
            CaseNumber = "CASH-CASE-123",
            CustomerId = customer.Id,
            DeceasedCustomerId = customer.Id,
            BranchId = branchId
        };
        db.FuneralCases.Add(funeralCase);
        await db.SaveChangesAsync();

        var policyDto = new PolicyCreateDto(
            PolicyNumber: "POL-CASH-123",
            ProviderName: "CashLife Insurance",
            CoverageAmount: 10000m,
            StartDate: DateTime.UtcNow.AddMonths(-1),
            EndDate: null,
            PolicyPlanId: plan.Id,
            Members: new List<PolicyMemberCreateDto>
            {
                new PolicyMemberCreateDto(customer.Id, MemberRole.Main)
            }
        );
        var policyId = await policyService.CreatePolicyAsync(policyDto, branchId);

        var claimDto = new ClaimCreateDto(
            ClaimNumber: "CLM-CASH-999",
            ClaimAmount: 5000m,
            PolicyId: policyId,
            FuneralCaseId: funeralCase.Id,
            Notes: "Cash payout claim"
        );
        var claimId = await claimService.CreateClaimAsync(claimDto, branchId);

        await claimService.UpdateClaimStatusAsync(claimId, new ClaimUpdateDto(
            Status: ClaimStatus.UnderReview,
            ProcessedAt: DateTime.UtcNow,
            Notes: "Reviewing documents"));

        await claimService.UpdateClaimStatusAsync(claimId, new ClaimUpdateDto(
            Status: ClaimStatus.Approved,
            ProcessedAt: DateTime.UtcNow,
            Notes: "Verified"));

        // Act
        await claimService.ExecutePayoutAsync(claimId);

        // Assert
        var finalClaim = await db.InsuranceClaims.FindAsync(claimId);
        finalClaim!.Status.Should().Be(ClaimStatus.Paid);

        var transaction = await db.FinancialTransactions
            .FirstOrDefaultAsync(t => t.SourceEntityId == claimId && t.SourceEntityType == "InsuranceClaim");
        transaction.Should().NotBeNull();

        var entries = await db.FinancialEntries
            .Where(e => e.FinancialTransactionId == transaction!.Id)
            .ToListAsync();
        entries.Count.Should().Be(2);
        entries.Should().Contain(e => e.AccountCode == "INS-EXP" && e.Debit == 10000m);
        entries.Should().Contain(e => e.AccountCode == "CASH-BANK" && e.Credit == 10000m);
    }

    [Fact]
    public async Task POS_QuickSale_Should_CreateInvoiceAndDecrementStock()
    {
        // Arrange
        var (sp, db, assignedBranches) = GetServiceProvider();
        var posService = sp.GetRequiredService<IPOSService>();
        var inventoryService = sp.GetRequiredService<IInventoryService>();
        var branchId = Guid.NewGuid();
        assignedBranches.Add(branchId);
        db.Branches.Add(new Branch { Id = branchId, Name = "Retail Branch" });

        var customer = new IndividualCustomer { Id = Guid.NewGuid(), FirstName = "Retail", LastName = "Customer", BranchId = branchId, IdentityNumber = "ID-POS-001" };
        db.Customers.Add(customer);

        var product = new FuneralProduct { Id = Guid.NewGuid(), Name = "Standard Casket", DefaultPrice = 1000m };
        db.FuneralProducts.Add(product);

        await inventoryService.InitializeStockAsync(product.Id, branchId, 10, Guid.NewGuid());
        await db.SaveChangesAsync();

        var request = new POSSaleRequest(
            CustomerId: customer.Id,
            BranchId: branchId,
            Items: new List<POSSaleItemRequest> { new(product.Id, 2) },
            PaymentAmount: 2000m,
            PaymentReference: "POS-REF-001"
        );

        // Act
        var invoiceId = await posService.CreateQuickSaleAsync(request);

        // Assert
        var invoice = await db.Invoices.FindAsync(invoiceId);
        invoice.Should().NotBeNull();

        var payment = await db.Payments.FirstOrDefaultAsync(p => p.InvoiceId == invoiceId);
        payment.Should().NotBeNull();
        payment!.Amount.Should().Be(2000m);

        var transaction = await db.FinancialTransactions.FirstOrDefaultAsync(t => t.SourceEntityId == invoiceId);
        transaction.Should().NotBeNull();

        var entries = await db.FinancialEntries.Where(e => e.FinancialTransactionId == transaction!.Id).ToListAsync();
        entries.Should().Contain(e => e.AccountCode == "CASH-BANK" && e.Debit == 2000m);
        entries.Should().Contain(e => e.AccountCode == "SALES-REVENUE" && e.Credit == 2000m);

        var stock = await inventoryService.GetStockLevelAsync(product.Id, branchId);
        stock.Should().Be(8); // 10 - 2
    }

    [Fact]
    public async Task Inventory_LowStockAlert_Should_ReturnCorrectItems()
    {
        // Arrange
        var (sp, db, assignedBranches) = GetServiceProvider();
        var inventoryService = sp.GetRequiredService<IInventoryService>();
        var branchId = Guid.NewGuid();
        assignedBranches.Add(branchId);
        db.Branches.Add(new Branch { Id = branchId, Name = "Main Branch" });

        var p1 = new FuneralProduct { Id = Guid.NewGuid(), Name = "Item 1" };
        var p2 = new FuneralProduct { Id = Guid.NewGuid(), Name = "Item 2" };
        db.FuneralProducts.AddRange(p1, p2);

        // p1: low stock (3 < 5), p2: healthy stock (10 > 5)
        await inventoryService.InitializeStockAsync(p1.Id, branchId, 3, Guid.NewGuid());
        await inventoryService.InitializeStockAsync(p2.Id, branchId, 10, Guid.NewGuid());
        await db.SaveChangesAsync();

        // Act
        var lowStockItems = await inventoryService.GetLowStockItemsAsync(branchId);

        // Assert
        lowStockItems.Should().ContainSingle();
        lowStockItems.First().ProductId.Should().Be(p1.Id);
    }
}
