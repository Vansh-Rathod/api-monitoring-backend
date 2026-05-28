using Core.CommonModels;
using Core.DTOs.Usage;
using Core.Interfaces.Repositories;
using GenericServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppEnum = Core.Enums.Enum;

namespace APIMonitoringSystem.Controllers;

[Authorize]
[Route("api/v1/usage")]
public class UsageController : BaseApiController
{
    private readonly IUsageIngestionRepository _usageIngestionRepository;
    private readonly ILoggingService _loggingService;

    public UsageController(IUsageIngestionRepository usageIngestionRepository, ILoggingService loggingService)
    {
        _usageIngestionRepository = usageIngestionRepository;
        _loggingService = loggingService;
    }

    [HttpPost("events/bulk")]
    [ProducesResponseType(typeof(APIResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<int>>> InsertBulkEvents([FromBody] List<UsageEventIngestItemDto> events)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<int>("Unauthorized.", "Missing tenant claim."));
            }

            var inserted = await _usageIngestionRepository.InsertBulkEventsAsync(tenantId.Value, events);
            return Ok(OkResponse(inserted, "Usage events inserted successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to ingest usage events.", AppEnum.LogLevel.Error, nameof(UsageController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<int>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpPost("monitor-check-runs")]
    [ProducesResponseType(typeof(APIResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<long>>> InsertMonitorCheckRun([FromBody] MonitorCheckRunRequestDto request)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<long>("Unauthorized.", "Missing tenant claim."));
            }

            request.TenantId = tenantId.Value;
            var checkRunId = await _usageIngestionRepository.InsertMonitorCheckRunAsync(request);
            return Ok(OkResponse(checkRunId, "Monitor check run inserted successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to insert monitor check run.", AppEnum.LogLevel.Error, nameof(UsageController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<long>("An unexpected error occurred.", ex.Message));
        }
    }
}
