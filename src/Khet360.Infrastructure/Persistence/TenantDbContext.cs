using Microsoft.EntityFrameworkCore;
using Khet360.Domain.Entities;
using Khet360.Application.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Khet360.Infrastructure.Persistence;

public class TenantDbContext : DbContext
{
    private readonly ITenantUserContext _userContext;

    public TenantDbContext(DbContextOptions<TenantDbContext> options, ITenantUserContext userContext) : base(options)
    {
        _userContext = userContext;
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<FinancialTransaction> FinancialTransactions { get; set; } = null!;
    public DbSet<FinancialEntry> FinancialEntries { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<UserBranch> UserBranches { get; set; } = null!;
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<OrganisationConfig> OrganisationConfigs { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;
    public DbSet<CustomerContact> CustomerContacts { get; set; } = null!;
    public DbSet<FamilyRelationship> FamilyRelationships { get; set; } = null!;
    public DbSet<WorkItem> WorkItems { get; set; } = null!;
    public DbSet<WorkItemHistory> WorkItemHistories { get; set; } = null!;
    public DbSet<Lead> Leads { get; set; } = null!;
    public DbSet<Opportunity> Opportunities { get; set; } = null!;
    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<FuneralCase> FuneralCases { get; set; } = null!;
    public DbSet<FuneralCaseMilestone> FuneralCaseMilestones { get; set; } = null!;
    public DbSet<RoutingRule> RoutingRules { get; set; } = null!;
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; } = null!;
    public DbSet<InsuranceClaim> InsuranceClaims { get; set; } = null!;
    public DbSet<ClaimPayment> ClaimPayments { get; set; } = null!;
    public DbSet<ServiceArrangement> ServiceArrangements { get; set; } = null!;
    public DbSet<ArrangementItem> ArrangementItems { get; set; } = null!;
    public DbSet<FuneralVehicle> FuneralVehicles { get; set; } = null!;
    public DbSet<MortuarySlot> MortuarySlots { get; set; } = null!;
    public DbSet<Repatriation> Repatriations { get; set; } = null!;
    public DbSet<CaseAccessToken> CaseAccessTokens { get; set; } = null!;
    public DbSet<DocumentRequest> DocumentRequests { get; set; } = null!;
    public DbSet<VehicleTelematics> VehicleTelematics { get; set; } = null!;
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; } = null!;
    public DbSet<WorkOrder> WorkOrders { get; set; } = null!;
    public DbSet<FuelLog> FuelLogs { get; set; } = null!;
    public DbSet<DriverProfile> DriverProfiles { get; set; } = null!;
    public DbSet<TripAssignment> TripAssignments { get; set; } = null!;
    public DbSet<VehicleDocument> VehicleDocuments { get; set; } = null!;
    public DbSet<Vendor> Vendors { get; set; } = null!;
    public DbSet<VendorOrder> VendorOrders { get; set; } = null!;
    public DbSet<VendorOrderItem> VendorOrderItems { get; set; } = null!;
    public DbSet<Memorial> Memorials { get; set; } = null!;
    public DbSet<Obituary> Obituaries { get; set; } = null!;
    public DbSet<MemorialTribute> MemorialTributes { get; set; } = null!;
    public DbSet<UserDashboardConfig> UserDashboardConfigs { get; set; } = null!;
    public DbSet<Feedback> Feedbacks { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<EmploymentContract> EmploymentContracts { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<LeaveType> LeaveTypes { get; set; } = null!;
    public DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;
    public DbSet<LeaveApplication> LeaveApplications { get; set; } = null!;
    public DbSet<PayProfile> PayProfiles { get; set; } = null!;
    public DbSet<PayItem> PayItems { get; set; } = null!;
    public DbSet<PayrollRun> PayrollRuns { get; set; } = null!;
    public DbSet<PayrollEntry> PayrollEntries { get; set; } = null!;
    public DbSet<Payslip> Payslips { get; set; } = null!;
    public DbSet<ProductionOrder> ProductionOrders { get; set; } = null!;
    public DbSet<ProductionLog> ProductionLogs { get; set; } = null!;
    public DbSet<QualityCheck> QualityChecks { get; set; } = null!;
    public DbSet<InstallationJob> InstallationJobs { get; set; } = null!;
    public DbSet<InstallationChecklist> InstallationChecklists { get; set; } = null!;
    public DbSet<InstallationSignOff> InstallationSignOffs { get; set; } = null!;
    public DbSet<PaymentConfiguration> PaymentConfigurations { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<ArrangementWizardState> ArrangementWizardStates { get; set; } = null!;



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionCode });

            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            entity.HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionCode);
        });

        modelBuilder.Entity<UserBranch>(entity =>
        {
            entity.HasKey(ub => new { ub.UserId, ub.BranchId });

            entity.HasOne(ub => ub.User)
                .WithMany(u => u.UserBranches)
                .HasForeignKey(ub => ub.UserId);

            entity.HasOne(ub => ub.Branch)
                .WithMany()
                .HasForeignKey(ub => ub.BranchId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
        });

        // Customer Hierarchy - TPH (Table-Per-Hierarchy)
        modelBuilder.Entity<Customer>()
            .HasDiscriminator<string>("CustomerType")
            .HasValue<IndividualCustomer>("Individual")
            .HasValue<OrganisationCustomer>("Organisation");

        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId);
        });

        modelBuilder.Entity<CustomerContact>(entity =>
        {
            entity.HasOne(c => c.Customer)
                .WithMany(cust => cust.Contacts)
                .HasForeignKey(c => c.CustomerId);
        });

        modelBuilder.Entity<FamilyRelationship>(entity =>
        {
            entity.HasOne(fr => fr.FromCustomer)
                .WithMany()
                .HasForeignKey(fr => fr.FromCustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(fr => fr.ToCustomer)
                .WithMany()
                .HasForeignKey(fr => fr.ToCustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkItemHistory>(entity =>
        {
            entity.HasOne(wh => wh.WorkItem)
                .WithMany(wi => wi.History)
                .HasForeignKey(wh => wh.WorkItemId);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasOne(a => a.Lead)
                .WithMany(l => l.Activities)
                .HasForeignKey(a => a.LeadId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Opportunity)
                .WithMany(o => o.Activities)
                .HasForeignKey(a => a.OpportunityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.WorkItem)
                .WithMany()
                .HasForeignKey(a => a.WorkItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.HasOne(c => c.Policy)
                .WithMany(p => p.Claims)
                .HasForeignKey(c => c.PolicyId);

            entity.HasOne(c => c.FuneralCase)
                .WithMany()
                .HasForeignKey(c => c.FuneralCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClaimPayment>(entity =>
        {
            entity.HasOne(p => p.Claim)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.ClaimId);
        });

        modelBuilder.Entity<ServiceArrangement>(entity =>
        {
            entity.HasOne(s => s.FuneralCase)
                .WithMany()
                .HasForeignKey(s => s.FuneralCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ArrangementItem>(entity =>
        {
            entity.HasOne(i => i.ServiceArrangement)
                .WithMany(s => s.Items)
                .HasForeignKey(i => i.ServiceArrangementId);
        });

        modelBuilder.Entity<VendorOrderItem>(entity =>
        {
            entity.HasOne(i => i.VendorOrder)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.VendorOrderId);
        });

        modelBuilder.Entity<MortuarySlot>(entity =>
        {
            entity.HasOne(s => s.FuneralCase)
                .WithMany()
                .HasForeignKey(s => s.FuneralCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Memorial>(entity =>
        {
            entity.HasOne(m => m.FuneralCase)
                .WithMany()
                .HasForeignKey(m => m.FuneralCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Obituary>(entity =>
        {
            entity.HasOne(o => o.Memorial)
                .WithOne(m => m.Obituary)
                .HasForeignKey<Obituary>(o => o.MemorialId);
        });

        modelBuilder.Entity<MemorialTribute>(entity =>
        {
            entity.HasOne(t => t.Memorial)
                .WithMany(m => m.Tributes)
                .HasForeignKey(t => t.MemorialId);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasOne(i => i.FuneralCase)
                .WithMany()
                .HasForeignKey(i => i.FuneralCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId);
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasOne(pt => pt.Invoice)
                .WithMany()
                .HasForeignKey(pt => pt.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            entity.HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId);

            entity.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId);

            entity.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Contract)
                .WithOne()
                .HasForeignKey<EmploymentContract>(c => c.EmployeeId);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasOne(d => d.Branch)
                .WithMany()
                .HasForeignKey(d => d.BranchId);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.EmployeeCode).IsUnique();
        });

        modelBuilder.Entity<PayProfile>(entity =>
        {
            entity.HasOne(pp => pp.Employee)
                .WithOne()
                .HasForeignKey<PayProfile>(pp => pp.EmployeeId);
        });

        modelBuilder.Entity<InstallationJob>(entity =>
        {
            entity.HasOne(ij => ij.Memorial)
                .WithMany()
                .HasForeignKey(ij => ij.MemorialId);

            entity.HasOne(ij => ij.Branch)
                .WithMany()
                .HasForeignKey(ij => ij.BranchId);

            entity.HasOne(ij => ij.Vehicle)
                .WithMany()
                .HasForeignKey(ij => ij.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ij => ij.LeadArtisan)
                .WithMany()
                .HasForeignKey(ij => ij.LeadArtisanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ij => ij.SignOff)
                .WithOne()
                .HasForeignKey<InstallationSignOff>(isoff => isoff.InstallationJobId);
        });

        modelBuilder.Entity<InstallationChecklist>(entity =>
        {
            entity.HasOne(ic => ic.InstallationJob)
                .WithMany(ij => ij.Checklist)
                .HasForeignKey(ic => ic.InstallationJobId);
        });

        modelBuilder.Entity<InstallationSignOff>(entity =>
        {
            entity.HasOne(isoff => isoff.InstallationJob)
                .WithOne()
                .HasForeignKey<InstallationSignOff>(isoff => isoff.InstallationJobId);
        });

        modelBuilder.Entity<PayrollEntry>(entity =>
        {
            entity.HasOne(pe => pe.PayrollRun)
                .WithMany(pr => pr.Entries)
                .HasForeignKey(pe => pe.PayrollRunId);

            entity.HasOne(pe => pe.Employee)
                .WithMany()
                .HasForeignKey(pe => pe.EmployeeId);

            entity.HasOne(pe => pe.PayItem)
                .WithMany()
                .HasForeignKey(pe => pe.PayItemId);
        });

        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.HasOne(ps => ps.Employee)
                .WithMany()
                .HasForeignKey(ps => ps.EmployeeId);

            entity.HasOne(ps => ps.PayrollRun)
                .WithMany()
                .HasForeignKey(ps => ps.PayrollRunId);
        });

        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.HasOne(lb => lb.Employee)
                .WithMany()
                .HasForeignKey(lb => lb.EmployeeId);

            entity.HasOne(lb => lb.LeaveType)
                .WithMany()
                .HasForeignKey(lb => lb.LeaveTypeId);
        });

        modelBuilder.Entity<LeaveApplication>(entity =>
        {
            entity.HasOne(la => la.Employee)
                .WithMany()
                .HasForeignKey(la => la.EmployeeId);

            entity.HasOne(la => la.LeaveType)
                .WithMany()
                .HasForeignKey(la => la.LeaveTypeId);

            entity.HasOne(la => la.Approver)
                .WithMany()
                .HasForeignKey(la => la.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Apply Branch Scope filters to all IBranchScoped entities
        var processedRootTypes = new HashSet<Type>();
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IBranchScoped).IsAssignableFrom(entityType.ClrType))
            {
                var currentType = entityType;
                while (currentType.BaseType != null)
                {
                    currentType = currentType.BaseType;
                }
                var rootType = currentType.ClrType;

                if (processedRootTypes.Add(rootType))
                {
                    ApplyBranchScopeFilter(modelBuilder, rootType);
                }
            }
        }
    }

    private void ApplyBranchScopeFilter(ModelBuilder modelBuilder, Type type)
    {
        // Create expression: e => _userContext.AssignedBranchIds.Contains(e.BranchId)

        // Parameter 'e' of type 'type'
        var parameter = Expression.Parameter(type, "e");

        // Property 'BranchId' on 'e'
        var branchIdProperty = type.GetProperty("BranchId");
        if (branchIdProperty == null) return;
        var branchIdExpression = Expression.Property(parameter, branchIdProperty);

        // Field '_userContext' on the DbContext
        var userContextField = typeof(TenantDbContext).GetField("_userContext", BindingFlags.NonPublic | BindingFlags.Instance);
        if (userContextField == null) return;
        var userContextExpression = Expression.Field(Expression.Constant(this), userContextField);

        // Property 'AssignedBranchIds' on '_userContext'
        var assignedBranchesProperty = typeof(ITenantUserContext).GetProperty("AssignedBranchIds");
        if (assignedBranchesProperty == null) return;
        var assignedBranchesExpression = Expression.Property(userContextExpression, assignedBranchesProperty);

        // Method 'Contains' on IReadOnlyList<Guid> or Enumerable.Contains
        var containsMethod = typeof(IReadOnlyList<Guid>).GetMethod("Contains");
        bool isStatic = false;

        if (containsMethod == null)
        {
            isStatic = true;
            containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Guid));
        }

        Expression containsExpression;
        if (isStatic)
        {
            containsExpression = Expression.Call(null, containsMethod, assignedBranchesExpression, branchIdExpression);
        }
        else
        {
            containsExpression = Expression.Call(assignedBranchesExpression, containsMethod, branchIdExpression);
        }

        var lambda = Expression.Lambda(containsExpression, parameter);

        modelBuilder.Entity(type).HasQueryFilter(lambda);
    }
}

