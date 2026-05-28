namespace Core.DTOs.Auth;

public class RefreshTokenRecordDto
{
    public long RefreshTokenId { get; set; }
    public long UserId { get; set; }
    public long TenantId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string? JwtId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public bool IsRevoked { get; set; }
}
