using APIMonitoringSystem.Services;
using Core.CommonModels;
using Core.DTOs.Auth;
using Core.Interfaces.Repositories;
using GenericServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppEnum = Core.Enums.Enum;

namespace APIMonitoringSystem.Controllers;

[Route("api/v1/auth")]
public class AuthController : BaseApiController
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILoggingService _loggingService;

    public AuthController(
        IAuthRepository authRepository,
        IJwtTokenService jwtTokenService,
        ILoggingService loggingService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
        _loggingService = loggingService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(APIResponse<AuthTokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<AuthTokenResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<APIResponse<AuthTokenResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var user = await _authRepository.GetUserForLoginAsync(request.Email, request.TenantId);
            if (user is null || !user.IsActive)
            {
                return Unauthorized(FailResponse<AuthTokenResponseDto>("Invalid credentials.", "User not found or inactive."));
            }

            // TODO: replace with secure password hashing verification (BCrypt/PBKDF2).
            if (!string.Equals(user.PasswordHash, request.Password, StringComparison.Ordinal))
            {
                return Unauthorized(FailResponse<AuthTokenResponseDto>("Invalid credentials.", "Email or password is incorrect."));
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenHash = _jwtTokenService.ComputeRefreshTokenHash(refreshToken.Token);

            await _authRepository.SaveRefreshTokenAsync(new SaveRefreshTokenRequestDto
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                TokenHash = refreshTokenHash,
                JwtId = accessToken.JwtId,
                ExpiresAtUtc = refreshToken.ExpiresAtUtc,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            var response = new AuthTokenResponseDto
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc
            };

            return Ok(OkResponse(response, "Login successful."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Login failed due to server error.",
                AppEnum.LogLevel.Error,
                nameof(AuthController),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["Email"] = request.Email,
                    ["TenantId"] = request.TenantId
                });

            return StatusCode(StatusCodes.Status500InternalServerError,
                FailResponse<AuthTokenResponseDto>("An unexpected error occurred.", ex.Message));
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(APIResponse<AuthTokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<AuthTokenResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<APIResponse<AuthTokenResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var tokenHash = _jwtTokenService.ComputeRefreshTokenHash(request.RefreshToken);
            var tokenRecord = await _authRepository.GetRefreshTokenByHashAsync(tokenHash);
            if (tokenRecord is null || tokenRecord.IsRevoked || tokenRecord.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return Unauthorized(FailResponse<AuthTokenResponseDto>("Invalid refresh token.", "Token is expired or revoked."));
            }

            await _authRepository.RevokeRefreshTokenAsync(tokenHash, HttpContext.Connection.RemoteIpAddress?.ToString());

            var user = await _authRepository.GetUserByIdAsync(tokenRecord.UserId, tokenRecord.TenantId);
            if (user is null || !user.IsActive)
            {
                return Unauthorized(FailResponse<AuthTokenResponseDto>("Invalid user context.", "User was not found for this token."));
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var newRefreshHash = _jwtTokenService.ComputeRefreshTokenHash(refreshToken.Token);

            await _authRepository.SaveRefreshTokenAsync(new SaveRefreshTokenRequestDto
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                TokenHash = newRefreshHash,
                JwtId = accessToken.JwtId,
                ExpiresAtUtc = refreshToken.ExpiresAtUtc,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            var response = new AuthTokenResponseDto
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc
            };

            return Ok(OkResponse(response, "Token refreshed successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Refresh token flow failed.",
                AppEnum.LogLevel.Error,
                nameof(AuthController),
                ex.ToString());

            return StatusCode(StatusCodes.Status500InternalServerError,
                FailResponse<AuthTokenResponseDto>("An unexpected error occurred.", ex.Message));
        }
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<bool>>> Logout([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var tokenHash = _jwtTokenService.ComputeRefreshTokenHash(request.RefreshToken);
            var revoked = await _authRepository.RevokeRefreshTokenAsync(tokenHash, HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok(OkResponse(revoked, revoked ? "Logout successful." : "Refresh token not found."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Logout flow failed.",
                AppEnum.LogLevel.Error,
                nameof(AuthController),
                ex.ToString());

            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<bool>("An unexpected error occurred.", ex.Message));
        }
    }
}
