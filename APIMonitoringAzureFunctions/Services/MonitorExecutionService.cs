using APIMonitoringAzureFunctions.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace APIMonitoringAzureFunctions.Services;

public class MonitorExecutionService : IMonitorExecutionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MonitorExecutionService> _logger;

    public MonitorExecutionService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MonitorExecutionService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> ExecuteActiveMonitorsAsync(CancellationToken cancellationToken)
    {
        var monitors = await GetActiveMonitorsAsync();
        if (monitors.Count == 0)
        {
            _logger.LogInformation("No active monitors found.");
            return 0;
        }

        var executedCount = 0;
        foreach (var monitor in monitors)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await ExecuteSingleMonitorAsync(monitor, cancellationToken);
                executedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Monitor execution failed. MonitorId={MonitorId}, TenantId={TenantId}", monitor.MonitorId, monitor.TenantId);
            }
        }

        return executedCount;
    }

    private async Task<List<ActiveMonitorDto>> GetActiveMonitorsAsync()
    {
        await using var connection = new SqlConnection(GetConnectionString());
        var monitors = await connection.QueryAsync<ActiveMonitorDto>(
            "dbo.usp_Monitor_ListActive",
            commandType: CommandType.StoredProcedure);
        return monitors.ToList();
    }

    private async Task ExecuteSingleMonitorAsync(ActiveMonitorDto monitor, CancellationToken cancellationToken)
    {
        var requestUri = BuildMonitorUri(monitor.BaseUrl, monitor.Path);
        using var request = new HttpRequestMessage(new HttpMethod(monitor.HttpMethod), requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var client = _httpClientFactory.CreateClient("monitor-executor");
        client.Timeout = TimeSpan.FromMilliseconds(Math.Max(500, monitor.TimeoutMs));

        var stopwatch = Stopwatch.StartNew();
        int? statusCode = null;
        string? errorMessage = null;
        byte resultCode;
        bool isSuccess;

        try
        {
            using var response = await client.SendAsync(request, cancellationToken);
            stopwatch.Stop();
            statusCode = (int)response.StatusCode;
            isSuccess = statusCode == monitor.ExpectedStatusCode;
            resultCode = isSuccess ? (byte)1 : (byte)2;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            isSuccess = false;
            resultCode = 3;
            errorMessage = $"Timeout: {ex.Message}";
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            isSuccess = false;
            resultCode = 4;
            errorMessage = ex.Message;
        }

        var checkedAtUtc = DateTime.UtcNow;
        var latencyMs = (int)stopwatch.ElapsedMilliseconds;

        await InsertMonitorCheckRunAsync(
            monitor.TenantId,
            monitor.MonitorId,
            checkedAtUtc,
            statusCode,
            latencyMs,
            resultCode,
            errorMessage);

        if (monitor.CustomerId.HasValue)
        {
            await InsertUsageEventAsync(
                monitor.TenantId,
                monitor.CustomerId.Value,
                monitor.MonitorId,
                checkedAtUtc,
                statusCode ?? 0,
                latencyMs,
                isSuccess,
                $"monitor-{monitor.MonitorId}-{Guid.NewGuid():N}");
        }
    }

    private async Task InsertMonitorCheckRunAsync(
        long tenantId,
        long monitorId,
        DateTime checkedAtUtc,
        int? statusCode,
        int latencyMs,
        byte result,
        string? errorMessage)
    {
        await using var connection = new SqlConnection(GetConnectionString());
        await connection.ExecuteAsync(
            "dbo.usp_MonitorCheckRun_Insert",
            new
            {
                TenantId = tenantId,
                MonitorId = monitorId,
                CheckedAtUtc = checkedAtUtc,
                StatusCode = statusCode,
                LatencyMs = latencyMs,
                Result = result,
                ErrorMessage = errorMessage
            },
            commandType: CommandType.StoredProcedure);
    }

    private async Task InsertUsageEventAsync(
        long tenantId,
        long customerId,
        long monitorId,
        DateTime requestAtUtc,
        int statusCode,
        int latencyMs,
        bool isSuccess,
        string traceId)
    {
        var eventsTable = new DataTable();
        eventsTable.Columns.Add("CustomerId", typeof(long));
        eventsTable.Columns.Add("MonitorId", typeof(long));
        eventsTable.Columns.Add("RequestAtUtc", typeof(DateTime));
        eventsTable.Columns.Add("StatusCode", typeof(int));
        eventsTable.Columns.Add("LatencyMs", typeof(int));
        eventsTable.Columns.Add("IsSuccess", typeof(bool));
        eventsTable.Columns.Add("RequestUnits", typeof(int));
        eventsTable.Columns.Add("TraceId", typeof(string));
        eventsTable.Rows.Add(customerId, monitorId, requestAtUtc, statusCode, latencyMs, isSuccess, 1, traceId);

        await using var connection = new SqlConnection(GetConnectionString());
        var parameters = new DynamicParameters();
        parameters.Add("@TenantId", tenantId);
        parameters.Add("@Events", eventsTable.AsTableValuedParameter("dbo.UsageEventTvp"));

        await connection.ExecuteAsync(
            "dbo.usp_UsageEvent_InsertBulk",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    private string BuildMonitorUri(string baseUrl, string path)
    {
        var trimmedBase = baseUrl.TrimEnd('/');
        var trimmedPath = path.StartsWith('/') ? path : "/" + path;
        return trimmedBase + trimmedPath;
    }

    private string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection")
            ?? _configuration["Sql:ConnectionString"]
            ?? throw new InvalidOperationException("DefaultConnection connection string is missing.");
    }
}
