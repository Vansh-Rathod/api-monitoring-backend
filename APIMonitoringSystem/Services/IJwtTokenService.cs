using Core.DTOs.Auth;

namespace APIMonitoringSystem.Services;

public interface IJwtTokenService
{
    (string Token, string JwtId, DateTime ExpiresAtUtc) GenerateAccessToken(AuthUserDto user);
    (string Token, DateTime ExpiresAtUtc) GenerateRefreshToken();
    string ComputeRefreshTokenHash(string refreshToken);
}
