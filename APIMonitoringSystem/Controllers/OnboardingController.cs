using Core.CommonModels;
using Core.DTOs.Customers;
using Core.DTOs.TenantOnboarding;
using Core.Interfaces.Repositories;
using GenericServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppEnum = Core.Enums.Enum;

namespace APIMonitoringSystem.Controllers;

[Route("api/v1/onboarding")]
public class OnboardingController : BaseApiController
{
    private readonly ITenantOnboardingRepository _tenantOnboardingRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILoggingService _loggingService;

    public OnboardingController(
        ITenantOnboardingRepository tenantOnboardingRepository,
        ICustomerRepository customerRepository,
        ILoggingService loggingService)
    {
        _tenantOnboardingRepository = tenantOnboardingRepository;
        _customerRepository = customerRepository;
        _loggingService = loggingService;
    }

    [AllowAnonymous]
    [HttpPost("tenant")]
    [ProducesResponseType(typeof(APIResponse<CreateTenantWithOwnerResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<CreateTenantWithOwnerResultDto>>> CreateTenant([FromBody] CreateTenantWithOwnerRequestDto request)
    {
        try
        {
            var result = await _tenantOnboardingRepository.CreateTenantWithOwnerAsync(request);
            return Ok(OkResponse(result, "Tenant created successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Tenant onboarding failed.",
                AppEnum.LogLevel.Error,
                nameof(OnboardingController),
                ex.ToString());

            return StatusCode(StatusCodes.Status500InternalServerError,
                FailResponse<CreateTenantWithOwnerResultDto>("An unexpected error occurred.", ex.Message));
        }
    }

    [Authorize]
    [HttpPost("customer")]
    [ProducesResponseType(typeof(APIResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<long>>> CreateInitialCustomer([FromBody] CreateCustomerRequestDto request)
    {
        try
        {
            var tenantIdFromClaim = GetTenantIdFromClaims();
            var userIdFromClaim = GetUserIdFromClaims();
            if (!tenantIdFromClaim.HasValue || !userIdFromClaim.HasValue)
            {
                return Unauthorized(FailResponse<long>("Unauthorized.", "Missing tenant or user claims."));
            }

            request.TenantId = tenantIdFromClaim.Value;
            request.CreatedByUserId = userIdFromClaim.Value;

            var customerId = await _customerRepository.CreateAsync(request);
            return Ok(OkResponse(customerId, "Customer onboarded successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Initial customer onboarding failed.",
                AppEnum.LogLevel.Error,
                nameof(OnboardingController),
                ex.ToString());

            return StatusCode(StatusCodes.Status500InternalServerError,
                FailResponse<long>("An unexpected error occurred.", ex.Message));
        }
    }
}
