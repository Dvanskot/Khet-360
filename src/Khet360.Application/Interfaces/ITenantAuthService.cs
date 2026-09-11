using System.Threading.Tasks;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Interfaces;

public record AuthResponse(string Token, string RefreshToken, string Username, string Email, string[] Roles);

public interface ITenantAuthService
{
    Task<AuthResponse?> LoginAsync(string username, string password, Guid tenantId);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> LogoutAsync(string token, string refreshToken);
}
