namespace Core.DTOs.Monitors;

public class MonitorHealthDto
{
    public long MonitorId { get; set; }
    public string MonitorName { get; set; } = string.Empty;
    public long? CustomerId { get; set; }
    public DateTime? CheckedAtUtc { get; set; }
    public int? StatusCode { get; set; }
    public int? LatencyMs { get; set; }
    public byte? Result { get; set; }
    public string? ErrorMessage { get; set; }
}
