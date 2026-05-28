namespace Core.DTOs.Customers;

public class UpdateCustomerStatusRequestDto
{
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public byte Status { get; set; }
}
