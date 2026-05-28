using Core.CommonModels;
using Core.DTOs.Customers;
using Core.Interfaces.Repositories;
using GenericServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppEnum = Core.Enums.Enum;

namespace APIMonitoringSystem.Controllers;

[Authorize]
[Route("api/v1/customers")]
public class CustomersController : BaseApiController
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILoggingService _loggingService;

    public CustomersController(ICustomerRepository customerRepository, ILoggingService loggingService)
    {
        _customerRepository = customerRepository;
        _loggingService = loggingService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<long>>> Create([FromBody] CreateCustomerRequestDto request)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            var userId = GetUserIdFromClaims();
            if (!tenantId.HasValue || !userId.HasValue)
            {
                return Unauthorized(FailResponse<long>("Unauthorized.", "Missing tenant or user claims."));
            }

            request.TenantId = tenantId.Value;
            request.CreatedByUserId = userId.Value;

            var customerId = await _customerRepository.CreateAsync(request);
            return Ok(OkResponse(customerId, "Customer created successfully."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to create customer.", AppEnum.LogLevel.Error, nameof(CustomersController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<long>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpPut("{customerId:long}")]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<bool>>> Update(long customerId, [FromBody] UpdateCustomerRequestDto request)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<bool>("Unauthorized.", "Missing tenant claim."));
            }

            request.TenantId = tenantId.Value;
            request.CustomerId = customerId;

            var updated = await _customerRepository.UpdateAsync(request);
            return Ok(OkResponse(updated, updated ? "Customer updated successfully." : "Customer not found."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to update customer.", AppEnum.LogLevel.Error, nameof(CustomersController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<bool>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpPatch("{customerId:long}/status")]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<bool>>> UpdateStatus(long customerId, [FromBody] UpdateCustomerStatusRequestDto request)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<bool>("Unauthorized.", "Missing tenant claim."));
            }

            request.TenantId = tenantId.Value;
            request.CustomerId = customerId;

            var updated = await _customerRepository.UpdateStatusAsync(request);
            return Ok(OkResponse(updated, updated ? "Customer status updated successfully." : "Customer not found."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to update customer status.", AppEnum.LogLevel.Error, nameof(CustomersController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<bool>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpGet("{customerId:long}")]
    [ProducesResponseType(typeof(APIResponse<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<CustomerDto>>> GetById(long customerId)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<CustomerDto>("Unauthorized.", "Missing tenant claim."));
            }

            var customer = await _customerRepository.GetByIdAsync(tenantId.Value, customerId);
            if (customer is null)
            {
                return NotFound(FailResponse<CustomerDto>("Customer not found."));
            }

            return Ok(OkResponse(customer));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to fetch customer by id.", AppEnum.LogLevel.Error, nameof(CustomersController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<CustomerDto>("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<PagedCustomersDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<PagedCustomersDto>>> List([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var tenantId = GetTenantIdFromClaims();
            if (!tenantId.HasValue)
            {
                return Unauthorized(FailResponse<PagedCustomersDto>("Unauthorized.", "Missing tenant claim."));
            }

            var result = await _customerRepository.ListAsync(new CustomerListQueryDto
            {
                TenantId = tenantId.Value,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(OkResponse(result));
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync("Failed to list customers.", AppEnum.LogLevel.Error, nameof(CustomersController), ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, FailResponse<PagedCustomersDto>("An unexpected error occurred.", ex.Message));
        }
    }
}
