using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Persistence;

public class TenantDbInitializer
{
    public static async Task InitializeDatabase(TenantDbContext context, ILogger logger, Guid branchId)
    {
        try
        {
            logger.LogInformation("Initializing Tenant Database...");

            await context.Database.EnsureCreatedAsync();

            // 1. Seed Permissions
            var permissions = new List<Permission>
            {
                new() { Code = "User.Read", Name = "Read Users", Description = "Ability to view users" },
                new() { Code = "User.Write", Name = "Manage Users", Description = "Ability to create and edit users" },
                new() { Code = "Role.Read", Name = "Read Roles", Description = "Ability to view roles" },
                new() { Code = "Role.Write", Name = "Manage Roles", Description = "Ability to create and edit roles" },
                new() { Code = "FuneralCase.Read", Name = "Read Cases", Description = "Ability to view funeral cases" },
                new() { Code = "FuneralCase.Write", Name = "Manage Cases", Description = "Ability to create and edit funeral cases" },
                new() { Code = "Finance.Read", Name = "Read Finance", Description = "Ability to view financial transactions" },
                new() { Code = "Finance.Write", Name = "Manage Finance", Description = "Ability to create financial transactions" },
                new() { Code = "Payroll.Read", Name = "Read Payroll", Description = "Ability to view payroll" },
                new() { Code = "Payroll.Write", Name = "Manage Payroll", Description = "Ability to process payroll" },
                new() { Code = "Inventory.Read", Name = "Read Inventory", Description = "Ability to view stock" },
                new() { Code = "Inventory.Write", Name = "Manage Inventory", Description = "Ability to update stock" },
                new() { Code = "Policy.Read", Name = "Read Policies", Description = "Ability to view insurance policies" },
                new() { Code = "Policy.Write", Name = "Manage Policies", Description = "Ability to manage insurance policies" },
                new() { Code = "Claim.Read", Name = "Read Claims", Description = "Ability to view insurance claims" },
                new() { Code = "Claim.Write", Name = "Manage Claims", Description = "Ability to process insurance claims" }
            };

            foreach (var p in permissions)
            {
                if (!await context.Permissions.AnyAsync(x => x.Code == p.Code))
                {
                    context.Permissions.Add(p);
                }
            }

            // 2. Seed Roles
            var roles = new List<Role>
            {
                new() { Id = Guid.NewGuid(), Name = "Administrator", Description = "Full system access" },
                new() { Id = Guid.NewGuid(), Name = "Manager", Description = "Departmental management access" },
                new() { Id = Guid.NewGuid(), Name = "Staff", Description = "Standard operational access" }
            };

            foreach (var r in roles)
            {
                if (!await context.Roles.AnyAsync(x => x.Name == r.Name))
                {
                    context.Roles.Add(r);
                }
            }
            await context.SaveChangesAsync();

            // 3. Seed Role-Permission Mapping
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Administrator");
            var managerRole = await context.Roles.FirstAsync(r => r.Name == "Manager");
            var staffRole = await context.Roles.FirstAsync(r => r.Name == "Staff");

            var allPermissions = await context.Permissions.ToListAsync();

            // Administrator gets everything
            foreach (var p in allPermissions)
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionCode == p.Code))
                {
                    context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionCode = p.Code });
                }
            }

            // Manager gets read access to everything and write access to operational areas
            var managerPerms = new[] { "FuneralCase.Read", "FuneralCase.Write", "Finance.Read", "Payroll.Read", "Inventory.Read", "Inventory.Write", "Policy.Read", "Claim.Read" };
            foreach (var code in managerPerms)
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == managerRole.Id && rp.PermissionCode == code))
                {
                    context.RolePermissions.Add(new RolePermission { RoleId = managerRole.Id, PermissionCode = code });
                }
            }

            // Staff gets read access to operational areas
            var staffPerms = new[] { "FuneralCase.Read", "Inventory.Read", "Policy.Read" };
            foreach (var code in staffPerms)
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == staffRole.Id && rp.PermissionCode == code))
                {
                    context.RolePermissions.Add(new RolePermission { RoleId = staffRole.Id, PermissionCode = code });
                }
            }

            // 4. Seed Departments
            var departments = new List<Department>
            {
                new() { Id = Guid.NewGuid(), Name = "Administration", Description = "General administration and management" },
                new() { Id = Guid.NewGuid(), Name = "Operations", Description = "Funeral operations and services" },
                new() { Id = Guid.NewGuid(), Name = "Finance", Description = "Financial management and accounting" },
                new() { Id = Guid.NewGuid(), Name = "Human Resources", Description = "Employee management and payroll" }
            };

            foreach (var d in departments)
            {
                if (!await context.Departments.AnyAsync(x => x.Name == d.Name))
                {
                    context.Departments.Add(d);
                }
            }

            await context.SaveChangesAsync();

            logger.LogInformation("Successfully seeded tenant departments, roles, and permissions.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the Tenant Database.");
            throw;
        }
    }
}
