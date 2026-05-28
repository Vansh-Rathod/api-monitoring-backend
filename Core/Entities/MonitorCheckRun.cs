namespace Core.Entities;

public class MonitorCheckRun
{
    public long CheckRunId { get; set; }
    public long TenantId { get; set; }
    public long MonitorId { get; set; }
    public DateTime CheckedAtUtc { get; set; }
    public int? StatusCode { get; set; }
    public int? LatencyMs { get; set; }
    public byte Result { get; set; }
    public string? ErrorMessage { get; set; }
}
