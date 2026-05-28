namespace Core.DTOs.Usage;

public class DashboardSummaryQueryDto
{
    public long TenantId { get; set; }
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
}
