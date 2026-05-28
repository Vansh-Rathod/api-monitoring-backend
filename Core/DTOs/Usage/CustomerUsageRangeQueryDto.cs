namespace Core.DTOs.Usage;

public class CustomerUsageRangeQueryDto
{
    public long TenantId { get; set; }
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
    public long? CustomerId { get; set; }
}
