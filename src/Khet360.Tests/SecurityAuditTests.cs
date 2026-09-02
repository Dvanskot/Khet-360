using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;

namespace Khet360.Tests;

public class SecurityAuditTests
{
    private readonly TenantDbContext _db;
    private readonly Mock<ITenantUserContext> _mockUserContext;
    private readonly AuthorizationService _authService;

    public SecurityAuditTests()
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockUserContext = new Mock<ITenantUserContext>();
        _db = new TenantDbContext(options, _mockUserContext.Object);
        _authService = new AuthorizationService(_mockUserContext.Object, _db);
    }

    [Fact]
    public async Task IsAuthorized_Should_Return_True_When_User_Has_Permission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var permissionCode = "EMPLOYEE_WRITE";

        _mockUserContext.Setup(uc => uc.IsAuthenticated).Returns(true);
        _mockUserContext.Setup(uc => uc.UserId).Returns(userId);

        var role = new Role { Id = roleId, Name = "HR_Manager" };
        var permission = new Permission { Code = permissionCode, Name = "Edit Employees", Description = "Edit Employees" };
        var rolePerm = new RolePermission { RoleId = roleId, PermissionCode = permissionCode };
        var userRole = new UserRole { UserId = userId, RoleId = roleId };

        _db.Roles.Add(role);
        _db.Permissions.Add(permission);
        _db.RolePermissions.Add(rolePerm);
        _db.UserRoles.Add(userRole);
        await _db.SaveChangesAsync();

        // Act
        var result = await _authService.HasPermissionAsync(permissionCode);

        // Assert
        result.Should().BeTrue("because the user is assigned a role that has the required permission");
    }

    [Fact]
    public async Task IsAuthorized_Should_Return_False_When_User_Has_No_Roles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserContext.Setup(uc => uc.IsAuthenticated).Returns(true);
        _mockUserContext.Setup(uc => uc.UserId).Returns(userId);

        // Act
        var result = _authService.HasPermission("SOME_PERMISSION");

        // Assert
        result.Should().BeFalse("because the user has no roles and therefore no permissions");
    }

    [Fact]
    public async Task IsAuthorized_Should_Return_False_When_Role_Lacks_Permission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        _mockUserContext.Setup(uc => uc.IsAuthenticated).Returns(true);
        _mockUserContext.Setup(uc => uc.UserId).Returns(userId);

        var role = new Role { Id = roleId, Name = "General_Staff" };
        var userRole = new UserRole { UserId = userId, RoleId = roleId };

        _db.Roles.Add(role);
        _db.UserRoles.Add(userRole);
        await _db.SaveChangesAsync();

        // Act
        var result = _authService.HasPermission("ADMIN_PERMISSION");

        // Assert
        result.Should().BeFalse("because the user's role does not contain the required permission");
    }

    [Fact]
    public async Task IsAuthorized_Should_Verify_Branch_Scope()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var permissionCode = "BRANCH_MANAGER_ACTION";

        _mockUserContext.Setup(uc => uc.IsAuthenticated).Returns(true);
        _mockUserContext.Setup(uc => uc.UserId).Returns(userId);

        var role = new Role { Id = roleId, Name = "Branch_Manager" };
        var permission = new Permission { Code = permissionCode, Name = "Manage Branch", Description = "Manage Branch" };
        var rolePerm = new RolePermission { RoleId = roleId, PermissionCode = permissionCode };
        var userRole = new UserRole { UserId = userId, RoleId = roleId };
        var userBranch = new UserBranch { UserId = userId, BranchId = branchId };

        _db.Roles.Add(role);
        _db.Permissions.Add(permission);
        _db.RolePermissions.Add(rolePerm);
        _db.UserRoles.Add(userRole);
        _db.UserBranches.Add(userBranch);
        await _db.SaveChangesAsync();

        // Act
        var result = await _authService.HasPermissionInBranchAsync(permissionCode, branchId);

        // Assert
        result.Should().BeTrue("because the user has the permission and is assigned to the current branch");
    }
}
