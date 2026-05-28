using Core.CommonModels;
using Core.DTOs.Usage;
using Core.Interfaces.Repositories;
using GenericServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppEnum = Core.Enums.Enum;

namespace APIMonitoringSystem.Controllers;

[Authorize]
[Route("api/v1/dashboard")]
public class DashboardController : BaseApiController
{
    private readonly IUsageQueryRepository _usageQueryRepository;
    private readonly ILoggingService _loggingService;

    public DashboardController(IUsageQueryRepository usageQueryRepository, ILoggingService loggingService)
    {
        _usageQueryRepository = usageQueryRepository;
        _loggingService = loggingService;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(APIResponse<DashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<DashboardSummaryDto>>> GetSummary([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<DashboardSummaryDto>("Unauthorized.", "Missing tenant claim."));
            }

            var result = await _usageQueryRepository.GetDashboardSummaryAsync(new DashboardSummaryQueryDto
            {
                TenantId = tenantId.Value,
                FromUtc = fromUtc,
                ToUtc = toUtc
            });

            if (result is null)
            {
                return Ok(OkResponse(new DashboardSummaryDto(), "No usage data found."));
            }

            return Ok(OkResponse(result));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to fetch dashboard summary.", AppEnum.LogLevel.Error, nameof(DashboardController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<DashboardSummaryDto>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpGet("customer-usage")]
    [ProducesResponseType(typeof(APIResponse<List<CustomerUsageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<CustomerUsageDto>>>> GetCustomerUsage([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, [FromQuery] long? customerId = null)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<List<CustomerUsageDto>>("Unauthorized.", "Missing tenant claim."));
            }

            var result = await _usageQueryRepository.GetCustomerUsageRangeAsync(new CustomerUsageRangeQueryDto
            {
                TenantId = tenantId.Value,
                FromUtc = fromUtc,
                ToUtc = toUtc,
                CustomerId = customerId
            });

            return Ok(OkResponse(result));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to fetch customer usage range.", AppEnum.LogLevel.Error, nameof(DashboardController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<List<CustomerUsageDto>>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpGet("customer-usage/{customerId:long}/daily-trend")]
    [ProducesResponseType(typeof(APIResponse<List<CustomerUsageDailyTrendDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<CustomerUsageDailyTrendDto>>>> GetCustomerDailyTrend(long customerId, [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<List<CustomerUsageDailyTrendDto>>("Unauthorized.", "Missing tenant claim."));
            }

            var result = await _usageQueryRepository.GetCustomerUsageDailyTrendAsync(new CustomerUsageDailyTrendQueryDto
            {
                TenantId = tenantId.Value,
                CustomerId = customerId,
                FromDate = fromDate,
                ToDate = toDate
            });

            return Ok(OkResponse(result));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to fetch customer usage daily trend.", AppEnum.LogLevel.Error, nameof(DashboardController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<List<CustomerUsageDailyTrendDto>>("An unexpected error occurred.", ex.Message));
        }
    }
}
