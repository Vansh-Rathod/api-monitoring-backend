namespace Core.DTOs.Customers;

public class UpdateCustomerRequestDto
{
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public string? ExternalCustomerRef { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
