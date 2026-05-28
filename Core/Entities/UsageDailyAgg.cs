namespace Core.Entities;

public class UsageDailyAgg
{
    public long UsageDailyAggId { get; set; }
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public long? MonitorId { get; set; }
    public DateOnly UsageDate { get; set; }
    public long TotalRequests { get; set; }
    public long SuccessRequests { get; set; }
    public long ErrorRequests { get; set; }
    public int AvgLatencyMs { get; set; }
}
