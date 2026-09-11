namespace Khet360.Application.Dtos;

public record LoginRequest(string Username, string Password);

public record RefreshTokenRequest(string Token, string RefreshToken);

public record LogoutRequest(string RefreshToken);
