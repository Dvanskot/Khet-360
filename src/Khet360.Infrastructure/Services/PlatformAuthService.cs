using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Khet360.Infrastructure.Services;

public class PlatformAuthService : IPlatformAuthService
{
    private readonly PlatformDbContext _platformDb;
    private readonly IConfiguration _configuration;

    public PlatformAuthService(PlatformDbContext platformDb, IConfiguration configuration)
    {
        _platformDb = platformDb;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await _platformDb.PlatformUsers
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        var refreshToken = CreateRefreshToken();
        var now = DateTime.UtcNow;
        user.RefreshTokenHash = HashRefreshToken(refreshToken);
        user.RefreshTokenExpiresAtUtc = now.AddDays(GetRefreshTokenExpirationDays());
        user.RefreshTokenRevokedAtUtc = null;
        user.UpdatedAtUtc = now;
        await _platformDb.SaveChangesAsync();

        return new AuthResponse(
            GenerateToken(user),
            refreshToken,
            user.Username,
            user.Email,
            new[] { user.Role });
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = ValidateAccessToken(token);
        if (principal == null)
        {
            return null;
        }

        var userId = GetUserId(principal);
        if (userId == null)
        {
            return null;
        }

        var user = await _platformDb.PlatformUsers
            .FirstOrDefaultAsync(u => u.Id == userId.Value && u.IsActive);

        if (user == null || !VerifyStoredRefreshToken(refreshToken, user))
        {
            return null;
        }

        var newRefreshToken = CreateRefreshToken();
        var now = DateTime.UtcNow;
        user.RefreshTokenHash = HashRefreshToken(newRefreshToken);
        user.RefreshTokenExpiresAtUtc = now.AddDays(GetRefreshTokenExpirationDays());
        user.RefreshTokenRevokedAtUtc = null;
        user.UpdatedAtUtc = now;
        await _platformDb.SaveChangesAsync();

        return new AuthResponse(
            GenerateToken(user),
            newRefreshToken,
            user.Username,
            user.Email,
            new[] { user.Role });
    }

    public async Task<bool> LogoutAsync(string token, string refreshToken)
    {
        var principal = ValidateAccessToken(token);
        if (principal == null)
        {
            return false;
        }

        var userId = GetUserId(principal);
        if (userId == null)
        {
            return false;
        }

        var user = await _platformDb.PlatformUsers
            .FirstOrDefaultAsync(u => u.Id == userId.Value && u.IsActive);

        if (user == null || !VerifyStoredRefreshToken(refreshToken, user))
        {
            return false;
        }

        user.RefreshTokenRevokedAtUtc = DateTime.UtcNow;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await _platformDb.SaveChangesAsync();
        return true;
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        return Task.FromResult(ValidateAccessToken(token) != null);
    }

    private ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            var keyString = _configuration["Jwt:PlatformKey"];
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
                ClockSkew = TimeSpan.Zero
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    private string GenerateToken(PlatformUser user)
    {
        var keyString = _configuration["Jwt:PlatformKey"];
        if (string.IsNullOrWhiteSpace(keyString))
        {
            throw new InvalidOperationException("JWT PlatformKey is not configured.");
        }

        var now = DateTime.UtcNow;
        var expirationMinutes = _configuration.GetValue("Jwt:PlatformExpirationMinutes", 480);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("platform", "true"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: now.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private Guid? GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    private bool VerifyStoredRefreshToken(string refreshToken, PlatformUser user)
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

    private string CreateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    private string HashRefreshToken(string refreshToken)
    {
        return BCrypt.Net.BCrypt.HashPassword(refreshToken);
    }

    private int GetRefreshTokenExpirationDays()
    {
        return _configuration.GetValue("Jwt:RefreshTokenExpirationDays", 30);
    }
}
