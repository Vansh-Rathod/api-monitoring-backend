namespace Core.DTOs.Auth;

public class SaveRefreshTokenRequestDto
{
    public long UserId { get; set; }
    public long TenantId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string? JwtId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string? CreatedByIp { get; set; }
}
