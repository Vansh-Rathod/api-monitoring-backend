namespace Core.Entities;

public class TenantUser
{
    public long TenantUserId { get; set; }
    public long TenantId { get; set; }
    public long UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsPrimaryOwner { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
