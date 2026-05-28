using Core.DTOs.Monitors;
using Core.Interfaces.Repositories;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Data;
using System.Data;
using AppEnum = Core.Enums.Enum;

namespace Infrastructure.Repositories;

public class MonitorRepository : IMonitorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILoggingService _loggingService;

    public MonitorRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
    {
        _connectionFactory = connectionFactory;
        _loggingService = loggingService;
    }

    public async Task<long> CreateOrUpdateAsync(UpsertMonitorRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<long>(
                "dbo.usp_Monitor_CreateOrUpdate",
                new
                {
                    request.TenantId,
                    request.MonitorId,
                    request.CustomerId,
                    request.MonitorName,
                    request.BaseUrl,
                    request.Path,
                    request.HttpMethod,
                    request.CheckIntervalSeconds,
                    request.TimeoutMs,
                    request.ExpectedStatusCode
                },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to create/update monitor.",
                AppEnum.LogLevel.Error,
                nameof(MonitorRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = request.TenantId,
                    ["MonitorId"] = request.MonitorId ?? 0
                });
            throw;
        }
    }

    public async Task<bool> SetActiveAsync(SetMonitorActiveRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var affectedRows = await connection.QuerySingleAsync<int>(
                "dbo.usp_Monitor_SetActive",
                new { request.TenantId, request.MonitorId, request.IsActive },
                commandType: CommandType.StoredProcedure);

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to update monitor active status.",
                AppEnum.LogLevel.Error,
                nameof(MonitorRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = request.TenantId,
                    ["MonitorId"] = request.MonitorId,
                    ["IsActive"] = request.IsActive
                });
            throw;
        }
    }

    public async Task<List<MonitorHealthDto>> GetLatestHealthAsync(long tenantId, long? customerId = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<MonitorHealthDto>(
                "dbo.usp_MonitorHealth_GetLatest",
                new { TenantId = tenantId, CustomerId = customerId },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch monitor latest health.",
                AppEnum.LogLevel.Error,
                nameof(MonitorRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = tenantId,
                    ["CustomerId"] = customerId ?? 0
                });
            throw;
        }
    }
}
