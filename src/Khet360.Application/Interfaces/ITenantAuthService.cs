using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public record AuthResponse(string Token, string RefreshToken, string Username, string Email, string[] Roles);

public interface ITenantAuthService
{
    Task<AuthResponse?> LoginAsync(string username, string password, Guid tenantId);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> RefreshTokenAsync(string token, string refreshToken);
}
