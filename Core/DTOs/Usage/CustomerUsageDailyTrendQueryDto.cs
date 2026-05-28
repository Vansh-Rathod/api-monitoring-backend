namespace Core.DTOs.Usage;

public class CustomerUsageDailyTrendQueryDto
{
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
}
