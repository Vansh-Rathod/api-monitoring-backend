namespace Core.DTOs.Usage;

public class DashboardSummaryDto
{
    public long TotalRequests { get; set; }
    public long SuccessRequests { get; set; }
    public long ErrorRequests { get; set; }
    public int AvgLatencyMs { get; set; }
}
