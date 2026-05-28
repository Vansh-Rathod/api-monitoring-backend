using Core.DTOs.Usage;

namespace Core.Interfaces.Repositories;

public interface IUsageQueryRepository
{
    Task<DashboardSummaryDto?> GetDashboardSummaryAsync(DashboardSummaryQueryDto query);
    Task<List<CustomerUsageDto>> GetCustomerUsageRangeAsync(CustomerUsageRangeQueryDto query);
    Task<List<CustomerUsageDailyTrendDto>> GetCustomerUsageDailyTrendAsync(CustomerUsageDailyTrendQueryDto query);
}
