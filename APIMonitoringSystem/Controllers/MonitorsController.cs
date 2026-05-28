using Core.CommonModels;
using Core.DTOs.Monitors;
using Core.Interfaces.Repositories;
using GenericServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppEnum = Core.Enums.Enum;

namespace APIMonitoringSystem.Controllers;

[Authorize]
[Route("api/v1/monitors")]
public class MonitorsController : BaseApiController
{
    private readonly IMonitorRepository _monitorRepository;
    private readonly ILoggingService _loggingService;

    public MonitorsController(IMonitorRepository monitorRepository, ILoggingService loggingService)
    {
        _monitorRepository = monitorRepository;
        _loggingService = loggingService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<long>>> Create([FromBody] UpsertMonitorRequestDto request)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<long>("Unauthorized.", "Missing tenant claim."));
            }

            request.TenantId = tenantId.Value;
            request.MonitorId = null;

            var monitorId = await _monitorRepository.CreateOrUpdateAsync(request);
            return Ok(OkResponse(monitorId, "Monitor created successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to create monitor.", AppEnum.LogLevel.Error, nameof(MonitorsController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<long>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpPut("{monitorId:long}")]
    [ProducesResponseType(typeof(APIResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<long>>> Update(long monitorId, [FromBody] UpsertMonitorRequestDto request)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<long>("Unauthorized.", "Missing tenant claim."));
            }

            request.TenantId = tenantId.Value;
            request.MonitorId = monitorId;

            var updatedMonitorId = await _monitorRepository.CreateOrUpdateAsync(request);
            return Ok(OkResponse(updatedMonitorId, "Monitor updated successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to update monitor.", AppEnum.LogLevel.Error, nameof(MonitorsController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<long>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpPatch("{monitorId:long}/active")]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<bool>>> SetActive(long monitorId, [FromBody] SetMonitorActiveRequestDto request)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<bool>("Unauthorized.", "Missing tenant claim."));
            }

            request.TenantId = tenantId.Value;
            request.MonitorId = monitorId;

            var updated = await _monitorRepository.SetActiveAsync(request);
            return Ok(OkResponse(updated, updated ? "Monitor status updated successfully." : "Monitor not found."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to set monitor active state.", AppEnum.LogLevel.Error, nameof(MonitorsController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<bool>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpGet("health/latest")]
    [ProducesResponseType(typeof(APIResponse<List<MonitorHealthDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<MonitorHealthDto>>>> LatestHealth([FromQuery] long? customerId = null)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<List<MonitorHealthDto>>("Unauthorized.", "Missing tenant claim."));
            }

            var result = await _monitorRepository.GetLatestHealthAsync(tenantId.Value, customerId);
            return Ok(OkResponse(result));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to fetch latest monitor health.", AppEnum.LogLevel.Error, nameof(MonitorsController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<List<MonitorHealthDto>>("An unexpected error occurred.", ex.Message));
        }
    }
}
