using Core.DTOs.Usage;
using Core.Interfaces.Repositories;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Data;
using System.Data;
using AppEnum = Core.Enums.Enum;

namespace Infrastructure.Repositories;

public class UsageQueryRepository : IUsageQueryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILoggingService _loggingService;

    public UsageQueryRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
    {
        _connectionFactory = connectionFactory;
        _loggingService = loggingService;
    }

    public async Task<DashboardSummaryDto?> GetDashboardSummaryAsync(DashboardSummaryQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<DashboardSummaryDto>(
                "dbo.usp_Dashboard_GetSummary",
                new { query.TenantId, query.FromUtc, query.ToUtc },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch dashboard summary.",
                AppEnum.LogLevel.Error,
                nameof(UsageQueryRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = query.TenantId,
                    ["FromUtc"] = query.FromUtc,
                    ["ToUtc"] = query.ToUtc
                });
            throw;
        }
    }

    public async Task<List<CustomerUsageDto>> GetCustomerUsageRangeAsync(CustomerUsageRangeQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<CustomerUsageDto>(
                "dbo.usp_CustomerUsage_GetRange",
                new { query.TenantId, query.FromUtc, query.ToUtc, query.CustomerId },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch customer usage range.",
                AppEnum.LogLevel.Error,
                nameof(UsageQueryRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = query.TenantId,
                    ["CustomerId"] = query.CustomerId ?? 0,
                    ["FromUtc"] = query.FromUtc,
                    ["ToUtc"] = query.ToUtc
                });
            throw;
        }
    }

    public async Task<List<CustomerUsageDailyTrendDto>> GetCustomerUsageDailyTrendAsync(CustomerUsageDailyTrendQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<CustomerUsageDailyTrendDto>(
                "dbo.usp_CustomerUsage_GetDailyTrend",
                new
                {
                    query.TenantId,
                    query.CustomerId,
                    FromDate = query.FromDate.ToDateTime(TimeOnly.MinValue),
                    ToDate = query.ToDate.ToDateTime(TimeOnly.MinValue)
                },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch customer usage daily trend.",
                AppEnum.LogLevel.Error,
                nameof(UsageQueryRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = query.TenantId,
                    ["CustomerId"] = query.CustomerId,
                    ["FromDate"] = query.FromDate.ToString("yyyy-MM-dd"),
                    ["ToDate"] = query.ToDate.ToString("yyyy-MM-dd")
                });
            throw;
        }
    }
}
