using Microsoft.Azure.Functions.Worker;
using APIMonitoringAzureFunctions.Services;
using Microsoft.Extensions.Logging;

namespace APIMonitoringAzureFunctions;

public class Function1
{
    private readonly IMonitorExecutionService _monitorExecutionService;
    private readonly ILogger<Function1> _logger;

    public Function1(IMonitorExecutionService monitorExecutionService, ILogger<Function1> logger)
    {
        _monitorExecutionService = monitorExecutionService;
        _logger = logger;
    }

    [Function("MonitorSchedulerFunction")]
    public async Task Run([TimerTrigger("%MonitorSchedulerCron%")] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        var startedAt = DateTime.UtcNow;
        _logger.LogInformation("Monitor scheduler started at: {StartedAtUtc}", startedAt);

        var executedCount = await _monitorExecutionService.ExecuteActiveMonitorsAsync(cancellationToken);
        _logger.LogInformation("Monitor scheduler completed. ExecutedMonitors={ExecutedCount}", executedCount);

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}