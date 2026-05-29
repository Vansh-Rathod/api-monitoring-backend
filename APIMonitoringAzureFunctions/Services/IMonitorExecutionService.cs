namespace APIMonitoringAzureFunctions.Services;

public interface IMonitorExecutionService
{
    Task<int> ExecuteActiveMonitorsAsync(CancellationToken cancellationToken);
}
