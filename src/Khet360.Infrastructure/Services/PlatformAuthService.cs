using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Khet360.Infrastructure.Services;

public class PlatformAuthService
{
    private readonly IConfiguration _configuration;

    public PlatformAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GeneratePlatformToken(string username, string role)
    {
        var keyString = _configuration["Jwt:PlatformKey"];
        if (string.IsNullOrEmpty(keyString))
        {
            throw new InvalidOperationException("JWT PlatformKey is not configured in appsettings.json. This is a critical security requirement.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
