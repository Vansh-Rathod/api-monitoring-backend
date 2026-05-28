using Core.DTOs.Monitors;

namespace Core.Interfaces.Repositories;

public interface IMonitorRepository
{
    Task<long> CreateOrUpdateAsync(UpsertMonitorRequestDto request);
    Task<bool> SetActiveAsync(SetMonitorActiveRequestDto request);
    Task<List<MonitorHealthDto>> GetLatestHealthAsync(long tenantId, long? customerId = null);
}
