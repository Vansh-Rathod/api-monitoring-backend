using Core.DTOs.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APIMonitoringSystem.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, string JwtId, DateTime ExpiresAtUtc) GenerateAccessToken(AuthUserDto user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var keyValue = jwtSettings["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing in configuration.");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing in configuration.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing in configuration.");
        var accessTokenMinutes = int.TryParse(jwtSettings["AccessTokenExpireMinutes"], out var m)
            ? m
            : int.TryParse(jwtSettings["BearerTokenExpireMinutes"], out var legacyM) ? legacyM : 30;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
        var jwtId = Guid.NewGuid().ToString();
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(accessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("tenantId", user.TenantId.ToString()),
            new(JwtRegisteredClaimNames.Jti, jwtId)
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenValue, jwtId, expiresAtUtc);
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateRefreshToken()
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var refreshDays = int.TryParse(jwtSettings["RefreshTokenExpireDays"], out var d) ? d : 7;
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return (Convert.ToBase64String(randomBytes), DateTime.UtcNow.AddDays(refreshDays));
    }

    public string ComputeRefreshTokenHash(string refreshToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hash);
    }
}
