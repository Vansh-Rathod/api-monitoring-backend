namespace Core.DTOs.Monitors;

public class UpsertMonitorRequestDto
{
    public long TenantId { get; set; }
    public long? MonitorId { get; set; }
    public long? CustomerId { get; set; }
    public string MonitorName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; }
    public int TimeoutMs { get; set; }
    public int ExpectedStatusCode { get; set; }
}
