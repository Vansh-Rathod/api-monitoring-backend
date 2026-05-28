namespace Core.DTOs.Monitors;

public class SetMonitorActiveRequestDto
{
    public long TenantId { get; set; }
    public long MonitorId { get; set; }
    public bool IsActive { get; set; }
}
