namespace Core.Entities;

public class CustomerApiCredential
{
    public long CredentialId { get; set; }
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public string CredentialType { get; set; } = string.Empty;
    public byte[] CredentialHash { get; set; } = [];
    public string? Last4 { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RotatedAtUtc { get; set; }
}
