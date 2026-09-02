using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Khet360.Tests;

public class AdversarialSecurityTests
{
    private DbContextOptions<TenantDbContext> CreateOptions(string dbName)
    {
        return new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Fact]
    public async Task TenantA_Should_Not_Be_Able_To_Access_TenantB_Data()
    {
        // Arrange
        var optionsA = CreateOptions($"TenantA_{Guid.NewGuid()}");
        var optionsB = CreateOptions($"TenantB_{Guid.NewGuid()}");

        var mockUserContextA = new Mock<ITenantUserContext>();
        mockUserContextA.Setup(uc => uc.IsAuthenticated).Returns(true);
        mockUserContextA.Setup(uc => uc.UserId).Returns(Guid.NewGuid());

        using var contextA = new TenantDbContext(optionsA, mockUserContextA.Object);
        using var contextB = new TenantDbContext(optionsB, mockUserContextA.Object);

        var secretDataB = new User { Id = Guid.NewGuid(), Username = "SecretUserB", Email = "secret@tenantB.com" };
        contextB.Users.Add(secretDataB);
        await contextB.SaveChangesAsync();

        // Act
        // Attempt to find Tenant B's user in Tenant A's context
        var userInA = await contextA.Users.FindAsync(secretDataB.Id);

        // Assert
        userInA.Should().BeNull("because Tenant A's database should be physically isolated from Tenant B's database");
    }

    [Fact]
    public async Task User_Should_Not_Be_Able_To_Access_Data_Outside_Their_Assigned_Branch()
    {
        // Arrange
        var options = CreateOptions($"Security_{Guid.NewGuid()}");
        var mockUserContext = new Mock<ITenantUserContext>();
        mockUserContext.Setup(uc => uc.IsAuthenticated).Returns(true);
        var userId = Guid.NewGuid();
        mockUserContext.Setup(uc => uc.UserId).Returns(userId);

        var context = new TenantDbContext(options, mockUserContext.Object);

        var branchA = Guid.NewGuid();
        var branchB = Guid.NewGuid();

        // Setup User's assigned branches in the context
        mockUserContext.Setup(uc => uc.AssignedBranchIds).Returns(new List<Guid> { branchA });

        context.Branches.AddRange(
            new Branch { Id = branchA, Name = "Branch A" },
            new Branch { Id = branchB, Name = "Branch B" }
        );

        var userBranch = new UserBranch { UserId = userId, BranchId = branchA };
        context.UserBranches.Add(userBranch);

        var leadA = new Lead {
            Id = Guid.NewGuid(),
            FirstName = "LeadA",
            LastName = "Doe A",
            Email = "a@test.com",
            Phone = "123",
            BranchId = branchA
        };
        var leadB = new Lead {
            Id = Guid.NewGuid(),
            FirstName = "LeadB",
            LastName = "Doe B",
            Email = "b@test.com",
            Phone = "456",
            BranchId = branchB
        };
        context.Leads.AddRange(leadA, leadB);
        await context.SaveChangesAsync();

        // Act
        // The Global Query Filter should now automatically filter by User's assigned branches
        var accessibleLeads = await context.Leads.ToListAsync();

        // Assert
        accessibleLeads.Should().Contain(leadA);
        accessibleLeads.Should().NotContain(leadB);
    }
}
