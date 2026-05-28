using Core.CommonFunctions;
using Core.DTOs.Usage;
using Core.Interfaces.Repositories;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Data;
using System.Data;
using AppEnum = Core.Enums.Enum;

namespace Infrastructure.Repositories;

public class UsageIngestionRepository : IUsageIngestionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILoggingService _loggingService;

    public UsageIngestionRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
    {
        _connectionFactory = connectionFactory;
        _loggingService = loggingService;
    }

    public async Task<int> InsertBulkEventsAsync(long tenantId, IReadOnlyCollection<UsageEventIngestItemDto> events)
    {
        try
        {
            if (events.Count == 0)
            {
                return 0;
            }

            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@TenantId", tenantId);
            parameters.Add("@Events", CommonFunctions.CreateUsageEventDataTable(events).AsTableValuedParameter("dbo.UsageEventTvp"));

            return await connection.QuerySingleAsync<int>(
                "dbo.usp_UsageEvent_InsertBulk",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to insert usage events in bulk.",
                AppEnum.LogLevel.Error,
                nameof(UsageIngestionRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = tenantId,
                    ["EventCount"] = events.Count
                });
            throw;
        }
    }

    public async Task<long> InsertMonitorCheckRunAsync(MonitorCheckRunRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<long>(
                "dbo.usp_MonitorCheckRun_Insert",
                new
                {
                    request.TenantId,
                    request.MonitorId,
                    request.CheckedAtUtc,
                    request.StatusCode,
                    request.LatencyMs,
                    request.Result,
                    request.ErrorMessage
                },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to insert monitor check run.",
                AppEnum.LogLevel.Error,
                nameof(UsageIngestionRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = request.TenantId,
                    ["MonitorId"] = request.MonitorId
                });
            throw;
        }
    }
}
