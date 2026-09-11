using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace Khet360.Tests;

public class AuthFoundationTests
{
    [Fact]
    public async Task PlatformAuth_LoginRefreshLogout_UsesPersistedCredentials()
    {
        var db = CreatePlatformDbContext();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correct-password");
        db.PlatformUsers.Add(new PlatformUser
        {
            Id = Guid.NewGuid(),
            Username = "platform-admin",
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            Role = "PlatformAdmin",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new PlatformAuthService(db, CreateConfiguration());
        var login = await service.LoginAsync("platform-admin", "correct-password");

        login.Should().NotBeNull();
        var loginToken = new JwtSecurityTokenHandler().ReadJwtToken(login!.Token);
        loginToken.Claims.Should().Contain(claim => claim.Type == "platform" && claim.Value == "true");

        var refreshed = await service.RefreshTokenAsync(login.Token, login.RefreshToken);
        refreshed.Should().NotBeNull();
        refreshed!.RefreshToken.Should().NotBe(login.RefreshToken);

        (await service.LogoutAsync(refreshed.Token, refreshed.RefreshToken)).Should().BeTrue();
        (await service.RefreshTokenAsync(refreshed.Token, refreshed.RefreshToken)).Should().BeNull();
    }

    [Fact]
    public async Task PlatformAuth_RejectsUnknownUserAndPlainTextPassword()
    {
        var db = CreatePlatformDbContext();
        db.PlatformUsers.Add(new PlatformUser
        {
            Id = Guid.NewGuid(),
            Username = "platform-admin",
            Email = "admin@example.com",
            PasswordHash = "plain-text",
            Role = "PlatformAdmin",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new PlatformAuthService(db, CreateConfiguration());

        (await service.LoginAsync("missing", "correct-password")).Should().BeNull();
        (await service.LoginAsync("platform-admin", "plain-text")).Should().BeNull();
    }

    [Fact]
    public async Task TenantAuth_IssuesTenantBoundTokenAndRotatesRefreshToken()
    {
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var (db, tenantService) = await CreateTenantDbContextAsync(tenantId, branchId, roleId, userId);
        var service = new TenantAuthService(db, CreateConfiguration(), tenantService.Object);

        var login = await service.LoginAsync("tenant-user", "correct-password", tenantId);

        login.Should().NotBeNull();
        var token = new JwtSecurityTokenHandler().ReadJwtToken(login!.Token);
        token.Claims.Should().Contain(claim => claim.Type == "tenant_id" && claim.Value == tenantId.ToString());
        token.Claims.Should().Contain(claim => claim.Type == "branch_id" && claim.Value == branchId.ToString());

        var refreshedToken = await service.RefreshTokenAsync(login.Token, login.RefreshToken);
        refreshedToken.Should().NotBe(login.Token);
        await Assert.ThrowsAsync<SecurityTokenException>(() => service.RefreshTokenAsync(login.Token, login.RefreshToken));
    }

    [Fact]
    public async Task TenantAuth_RejectsPlainTextPasswordFallback()
    {
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var (db, tenantService) = await CreateTenantDbContextAsync(tenantId, branchId, roleId, userId);
        var user = await db.Users.SingleAsync();
        user.PasswordHash = "plain-text";
        await db.SaveChangesAsync();
        var service = new TenantAuthService(db, CreateConfiguration(), tenantService.Object);

        (await service.LoginAsync("tenant-user", "plain-text", tenantId)).Should().BeNull();
    }

    private static PlatformDbContext CreatePlatformDbContext()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PlatformDbContext(options);
    }

    private static async Task<(TenantDbContext DbContext, Mock<ITenantService> TenantService)> CreateTenantDbContextAsync(
        Guid tenantId,
        Guid branchId,
        Guid roleId,
        Guid userId,
        string password = "correct-password")
    {
        var userContext = new Mock<ITenantUserContext>();
        userContext.Setup(context => context.AssignedBranchIds).Returns(Array.Empty<Guid>());
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new TenantDbContext(options, userContext.Object);
        var tenantService = new Mock<ITenantService>();
        tenantService.Setup(service => service.CurrentTenant).Returns(new Tenant { Id = tenantId, Slug = "tenant-a" });

        db.Roles.Add(new Role { Id = roleId, Name = "Administrator" });
        db.Branches.Add(new Branch { Id = branchId, Name = "Main" });
        db.Users.Add(new User
        {
            Id = userId,
            Username = "tenant-user",
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = true
        });
        await db.SaveChangesAsync();

        db.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        db.UserBranches.Add(new UserBranch { UserId = userId, BranchId = branchId });
        await db.SaveChangesAsync();

        return (db, tenantService);
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "khet360",
            ["Jwt:Audience"] = "khet360",
            ["Jwt:PlatformKey"] = "PlatformSigningKey_1234567890abcdef",
            ["Jwt:TenantKey"] = "TenantSigningKey_1234567890abcdef",
            ["Jwt:PlatformExpirationMinutes"] = "480",
            ["Jwt:TenantExpirationMinutes"] = "480",
            ["Jwt:RefreshTokenExpirationDays"] = "30"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }
}
