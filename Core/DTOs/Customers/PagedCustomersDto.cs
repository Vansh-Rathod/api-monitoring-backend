namespace Core.DTOs.Customers;

public class PagedCustomersDto
{
    public List<CustomerDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}
