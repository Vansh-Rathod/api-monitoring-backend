namespace Core.DTOs.Usage;

public class UsageEventIngestItemDto
{
    public long CustomerId { get; set; }
    public long? MonitorId { get; set; }
    public DateTime RequestAtUtc { get; set; }
    public int StatusCode { get; set; }
    public int LatencyMs { get; set; }
    public bool IsSuccess { get; set; }
    public int RequestUnits { get; set; } = 1;
    public string? TraceId { get; set; }
}
