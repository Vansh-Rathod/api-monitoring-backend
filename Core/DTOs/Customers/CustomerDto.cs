namespace Core.DTOs.Customers;

public class CustomerDto
{
    public long CustomerId { get; set; }
    public long TenantId { get; set; }
    public string? ExternalCustomerRef { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public byte Status { get; set; }
    public DateTime OnboardedAtUtc { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
