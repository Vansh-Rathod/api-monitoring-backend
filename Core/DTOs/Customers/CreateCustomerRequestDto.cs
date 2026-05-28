namespace Core.DTOs.Customers;

public class CreateCustomerRequestDto
{
    public long TenantId { get; set; }
    public string? ExternalCustomerRef { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public long CreatedByUserId { get; set; }
}
