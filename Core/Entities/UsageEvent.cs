namespace Core.Entities;

public class UsageEvent
{
    public long UsageEventId { get; set; }
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public long? MonitorId { get; set; }
    public DateTime RequestAtUtc { get; set; }
    public int StatusCode { get; set; }
    public int LatencyMs { get; set; }
    public bool IsSuccess { get; set; }
    public int RequestUnits { get; set; }
    public string? TraceId { get; set; }
}
