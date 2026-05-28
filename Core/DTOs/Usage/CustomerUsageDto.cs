namespace Core.DTOs.Usage;

public class CustomerUsageDto
{
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public long TotalRequests { get; set; }
    public long SuccessRequests { get; set; }
    public long ErrorRequests { get; set; }
    public int AvgLatencyMs { get; set; }
}
