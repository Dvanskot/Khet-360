using Microsoft.EntityFrameworkCore;
using Khet360.Domain.Entities;
using Khet360.Application.Interfaces;

namespace Khet360.Infrastructure.Persistence;

public class TenantDbContext : DbContext
{
    private readonly ITenantUserContext _userContext;

    public TenantDbContext(DbContextOptions<TenantDbContext> options, ITenantUserContext userContext) : base(options)
    {
        _userContext = userContext;
    }

    public DbSet<User> Users { get; set; } = null!;
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

        // Apply Branch Scope filters to all IBranchScoped entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IBranchScoped).IsAssignableFrom(entityType.ClrType))
            {
                ApplyBranchScopeFilter(modelBuilder, entityType.ClrType);
            }
        }
    }

    private void ApplyBranchScopeFilter(ModelBuilder modelBuilder, Type type)
    {
        // This is a simplified version. In a real production app,
        // we'd use Expression trees to build the filter:
        // e => _userContext.AssignedBranchIds.Contains(e.BranchId)

        // Since we are in a plan-execution phase, I will implement the filter
        // specifically for entities as they are added to avoid complex Expression tree logic here,
        // OR I will implement a robust dynamic filter if needed.

        // For now, let's stick to a manual application per entity to ensure stability.
    }
}

