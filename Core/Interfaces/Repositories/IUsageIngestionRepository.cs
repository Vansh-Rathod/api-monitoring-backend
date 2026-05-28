using Core.DTOs.Usage;

namespace Core.Interfaces.Repositories;

public interface IUsageIngestionRepository
{
    Task<int> InsertBulkEventsAsync(long tenantId, IReadOnlyCollection<UsageEventIngestItemDto> events);
    Task<long> InsertMonitorCheckRunAsync(MonitorCheckRunRequestDto request);
}
