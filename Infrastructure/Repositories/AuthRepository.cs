using Core.DTOs.Auth;
using Core.Interfaces.Repositories;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Data;
using AppEnum = Core.Enums.Enum;
using System.Data;

namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILoggingService _loggingService;

    public AuthRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
    {
        _connectionFactory = connectionFactory;
        _loggingService = loggingService;
    }

    public async Task<AuthUserDto?> GetUserForLoginAsync(string email, long tenantId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AuthUserDto>(
                "dbo.usp_Auth_GetUserForLogin",
                new { Email = email, TenantId = tenantId },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch auth user for login.",
                AppEnum.LogLevel.Error,
                nameof(AuthRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["Email"] = email,
                    ["TenantId"] = tenantId
                });
            throw;
        }
    }

    public async Task<AuthUserDto?> GetUserByIdAsync(long userId, long tenantId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AuthUserDto>(
                "dbo.usp_Auth_GetUserById",
                new { UserId = userId, TenantId = tenantId },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch auth user by id.",
                AppEnum.LogLevel.Error,
                nameof(AuthRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["UserId"] = userId,
                    ["TenantId"] = tenantId
                });
            throw;
        }
    }

    public async Task<long> SaveRefreshTokenAsync(SaveRefreshTokenRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<long>(
                "dbo.usp_Auth_SaveRefreshToken",
                new
                {
                    request.UserId,
                    request.TenantId,
                    request.TokenHash,
                    request.JwtId,
                    request.ExpiresAtUtc,
                    request.CreatedByIp
                },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to save refresh token.",
                AppEnum.LogLevel.Error,
                nameof(AuthRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["UserId"] = request.UserId,
                    ["TenantId"] = request.TenantId
                });
            throw;
        }
    }

    public async Task<RefreshTokenRecordDto?> GetRefreshTokenByHashAsync(string tokenHash)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<RefreshTokenRecordDto>(
                "dbo.usp_Auth_GetRefreshTokenByHash",
                new { TokenHash = tokenHash },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch refresh token by hash.",
                AppEnum.LogLevel.Error,
                nameof(AuthRepository),
                ex.ToString());
            throw;
        }
    }

    public async Task<bool> RevokeRefreshTokenAsync(string tokenHash, string? revokedByIp)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.QuerySingleAsync<int>(
                "dbo.usp_Auth_RevokeRefreshToken",
                new { TokenHash = tokenHash, RevokedByIp = revokedByIp },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to revoke refresh token.",
                AppEnum.LogLevel.Error,
                nameof(AuthRepository),
                ex.ToString());
            throw;
        }
    }
}
