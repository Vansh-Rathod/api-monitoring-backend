namespace Core.DTOs.Customers;

public class CustomerListQueryDto
{
    public long TenantId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
