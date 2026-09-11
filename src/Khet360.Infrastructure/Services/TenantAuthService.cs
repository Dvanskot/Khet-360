using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Tenant;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Khet360.Infrastructure.Services;

public class TenantAuthService : ITenantAuthService
{
    private readonly TenantDbContext _tenantDb;
    private readonly IConfiguration _configuration;
    private readonly ITenantService _tenantService;

    public TenantAuthService(TenantDbContext tenantDb, IConfiguration configuration, ITenantService tenantService)
    {
        _tenantDb = tenantDb;
        _configuration = configuration;
        _tenantService = tenantService;
    }

    public async Task<AuthResponse?> LoginAsync(string username, string password, Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        if (_tenantService.CurrentTenant == null || _tenantService.CurrentTenant.Id != tenantId)
        {
            return null;
        }

        var user = await _tenantDb.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.UserBranches)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        var roles = GetUserRoles(user);
        var branchIds = GetUserBranchIds(user);
        var refreshToken = CreateRefreshToken();
        SetRefreshToken(user, refreshToken);
        user.UpdatedAt = DateTime.UtcNow;
        await _tenantDb.SaveChangesAsync();

        return new AuthResponse(
            GenerateJwtToken(user, roles, tenantId, branchIds),
            refreshToken,
            user.Username,
            user.Email,
            roles);
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        return Task.FromResult(ValidateAccessToken(token) != null);
    }

    public async Task<string> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = ValidateAccessToken(token);
        if (principal == null)
        {
            throw new SecurityTokenException("Access token is invalid.");
        }

        var userId = GetUserId(principal);
        var tenantId = GetTenantId(principal);
        if (userId == null || tenantId == null)
        {
            throw new SecurityTokenException("Token claims are invalid.");
        }

        EnsureCurrentTenant(tenantId.Value);

        var user = await _tenantDb.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.UserBranches)
            .FirstOrDefaultAsync(u => u.Id == userId.Value && u.IsActive);

        if (user == null || !VerifyStoredRefreshToken(refreshToken, user))
        {
            throw new SecurityTokenException("Refresh token is invalid.");
        }

        var newRefreshToken = CreateRefreshToken();
        SetRefreshToken(user, newRefreshToken);
        user.UpdatedAt = DateTime.UtcNow;
        await _tenantDb.SaveChangesAsync();

        return GenerateJwtToken(
            user,
            GetUserRoles(user),
            tenantId.Value,
            GetUserBranchIds(user));
    }

    public async Task<bool> LogoutAsync(string token, string refreshToken)
    {
        var principal = ValidateAccessToken(token);
        if (principal == null)
        {
            return false;
        }

        var userId = GetUserId(principal);
        var tenantId = GetTenantId(principal);
        if (userId == null || tenantId == null)
        {
            return false;
        }

        if (_tenantService.CurrentTenant == null || _tenantService.CurrentTenant.Id != tenantId.Value)
        {
            return false;
        }

        var user = await _tenantDb.Users
            .FirstOrDefaultAsync(u => u.Id == userId.Value && u.IsActive);

        if (user == null || !VerifyStoredRefreshToken(refreshToken, user))
        {
            return false;
        }

        user.RefreshTokenRevokedAtUtc = DateTime.UtcNow;
        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAtUtc = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _tenantDb.SaveChangesAsync();
        return true;
    }

    private string GenerateJwtToken(User user, string[] roles, Guid tenantId, Guid[] branchIds)
    {
        var keyString = _configuration["Jwt:TenantKey"];
        if (string.IsNullOrWhiteSpace(keyString))
        {
            throw new InvalidOperationException("JWT TenantKey is not configured.");
        }

        var now = DateTime.UtcNow;
        var expirationMinutes = _configuration.GetValue("Jwt:TenantExpirationMinutes", 480);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("tenant_id", tenantId.ToString()),
            new("username", user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(branchIds.Select(branchId => new Claim("branch_id", branchId.ToString())));
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: now.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            var keyString = _configuration["Jwt:TenantKey"];
            if (string.IsNullOrWhiteSpace(keyString))
            {
                return null;
            }

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString)),
                NameClaimType = ClaimTypes.Name,
                ClockSkew = TimeSpan.Zero
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    private bool VerifyStoredRefreshToken(string refreshToken, User user)
    {
        if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(user.RefreshTokenHash))
        {
            return false;
        }

        if (user.RefreshTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            return false;
        }

        if (user.RefreshTokenRevokedAtUtc.HasValue)
        {
            return false;
        }

        return VerifyHash(refreshToken, user.RefreshTokenHash);
    }

    private bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    private bool VerifyHash(string value, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(value, hash);
        }
        catch
        {
            return false;
        }
    }

    private void SetRefreshToken(User user, string refreshToken)
    {
        user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
        user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());
        user.RefreshTokenRevokedAtUtc = null;
    }

    private string CreateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    private int GetRefreshTokenExpirationDays()
    {
        return _configuration.GetValue("Jwt:RefreshTokenExpirationDays", 30);
    }

    private Guid? GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    private Guid? GetTenantId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue("tenant_id");
        return Guid.TryParse(value, out var tenantId) ? tenantId : null;
    }

    private string[] GetUserRoles(User user)
    {
        return user.UserRoles
            .Select(ur => ur.Role?.Name)
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct()
            .ToArray()!;
    }

    private Guid[] GetUserBranchIds(User user)
    {
        return user.UserBranches
            .Select(ub => ub.BranchId)
            .Distinct()
            .ToArray();
    }

    private void EnsureCurrentTenant(Guid tenantId)
    {
        if (_tenantService.CurrentTenant == null || _tenantService.CurrentTenant.Id != tenantId)
        {
            throw new SecurityTokenException("Token tenant does not match the resolved tenant.");
        }
    }
}
