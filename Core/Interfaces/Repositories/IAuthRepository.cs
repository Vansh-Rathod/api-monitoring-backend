using Core.DTOs.Auth;

namespace Core.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<AuthUserDto?> GetUserForLoginAsync(string email, long tenantId);
    Task<AuthUserDto?> GetUserByIdAsync(long userId, long tenantId);
    Task<long> SaveRefreshTokenAsync(SaveRefreshTokenRequestDto request);
    Task<RefreshTokenRecordDto?> GetRefreshTokenByHashAsync(string tokenHash);
    Task<bool> RevokeRefreshTokenAsync(string tokenHash, string? revokedByIp);
}
