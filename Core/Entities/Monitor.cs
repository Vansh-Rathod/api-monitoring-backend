namespace Core.Entities;

public class Monitor
{
    public long MonitorId { get; set; }
    public long TenantId { get; set; }
    public long? CustomerId { get; set; }
    public string MonitorName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; }
    public int TimeoutMs { get; set; }
    public int ExpectedStatusCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
