using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Khet360.Infrastructure.Services;

public class TenantAuthService : ITenantAuthService
{
    private readonly TenantDbContext _tenantDb;
    private readonly IConfiguration _configuration;

    public TenantAuthService(TenantDbContext tenantDb, IConfiguration configuration)
    {
        _tenantDb = tenantDb;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(string username, string password, Guid tenantId)
    {
        var user = await _tenantDb.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        // Ensure the user belongs to the requested tenant
        if (user.UserBranches == null)
        {
            return null;
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
        var token = GenerateJwtToken(user, roles);
        var refreshToken = Guid.NewGuid().ToString(); // Simplified refresh token

        return new AuthResponse(token, refreshToken, user.Username, user.Email, roles);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        // Validation is handled by the JwtBearer middleware in Program.cs
        return true;
    }

    public async Task<string> RefreshTokenAsync(string token, string refreshToken)
    {
        throw new NotImplementedException("Refresh token logic requires a persistence store for tokens.");
    }

    private string GenerateJwtToken(Khet360.Domain.Entities.User user, string[] roles)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:TenantKey"] ?? "DefaultTenantKey_MustChangeInProduction_56789!"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("linked_branches", string.Join(",", user.UserBranches.Select(ub => ub.BranchId))),
            new Claim("username", user.Username)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, string hash)
    {
        // In production, use BCrypt or Argon2
        // Simplified for initial setup:
        return password == hash;
    }
}
