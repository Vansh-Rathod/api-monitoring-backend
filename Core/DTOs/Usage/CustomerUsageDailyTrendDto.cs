namespace Core.DTOs.Usage;

public class CustomerUsageDailyTrendDto
{
    public DateOnly UsageDate { get; set; }
    public long TotalRequests { get; set; }
    public long SuccessRequests { get; set; }
    public long ErrorRequests { get; set; }
    public int AvgLatencyMs { get; set; }
}
