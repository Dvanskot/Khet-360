using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Khet360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;

namespace Khet360.Tests;

public class TenantIsolationTests
{
    private DbContextOptions<TenantDbContext> CreateOptions(string databaseName)
    {
        return new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }

    [Fact]
    public async Task Data_Is_Isolated_Between_Tenants()
    {
        // Arrange
        var tenantA_DbName = $"TenantA_{Guid.NewGuid()}";
        var tenantB_DbName = $"TenantB_{Guid.NewGuid()}";

        var optionsA = CreateOptions(tenantA_DbName);
        var optionsB = CreateOptions(tenantB_DbName);

        var mockUserContext = new Mock<ITenantUserContext>();

        // Act
        using (var contextA = new TenantDbContext(optionsA, mockUserContext.Object))
        {
            contextA.Users.Add(new User { Id = Guid.NewGuid(), Username = "UserA", Email = "a@tenant.com" });
            await contextA.SaveChangesAsync();
        }

        using (var contextB = new TenantDbContext(optionsB, mockUserContext.Object))
        {
            var usersInB = await contextB.Users.ToListAsync();

            // Assert
            usersInB.Should().BeEmpty("because Tenant B should not see Tenant A's data");
        }
    }
}
