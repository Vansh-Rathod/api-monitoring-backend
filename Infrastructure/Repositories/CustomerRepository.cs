using Core.DTOs.Customers;
using Core.Interfaces.Repositories;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Data;
using System.Data;
using AppEnum = Core.Enums.Enum;

namespace Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILoggingService _loggingService;

    public CustomerRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
    {
        _connectionFactory = connectionFactory;
        _loggingService = loggingService;
    }

    public async Task<long> CreateAsync(CreateCustomerRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@TenantId", request.TenantId);
            parameters.Add("@ExternalCustomerRef", request.ExternalCustomerRef);
            parameters.Add("@CustomerName", request.CustomerName);
            parameters.Add("@Email", request.Email);
            parameters.Add("@Phone", request.Phone);
            parameters.Add("@CreatedByUserId", request.CreatedByUserId);

            return await connection.QuerySingleAsync<long>(
                "dbo.usp_Customer_Create",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to create customer.",
                AppEnum.LogLevel.Error,
                nameof(CustomerRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = request.TenantId,
                    ["CustomerName"] = request.CustomerName
                });
            throw;
        }
    }

    public async Task<bool> UpdateAsync(UpdateCustomerRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var affectedRows = await connection.QuerySingleAsync<int>(
                "dbo.usp_Customer_Update",
                new
                {
                    request.TenantId,
                    request.CustomerId,
                    request.ExternalCustomerRef,
                    request.CustomerName,
                    request.Email,
                    request.Phone
                },
                commandType: CommandType.StoredProcedure);

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to update customer.",
                AppEnum.LogLevel.Error,
                nameof(CustomerRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = request.TenantId,
                    ["CustomerId"] = request.CustomerId
                });
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(UpdateCustomerStatusRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var affectedRows = await connection.QuerySingleAsync<int>(
                "dbo.usp_Customer_UpdateStatus",
                new { request.TenantId, request.CustomerId, request.Status },
                commandType: CommandType.StoredProcedure);

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to update customer status.",
                AppEnum.LogLevel.Error,
                nameof(CustomerRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = request.TenantId,
                    ["CustomerId"] = request.CustomerId,
                    ["Status"] = request.Status
                });
            throw;
        }
    }

    public async Task<CustomerDto?> GetByIdAsync(long tenantId, long customerId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<CustomerDto>(
                "dbo.usp_Customer_GetById",
                new { TenantId = tenantId, CustomerId = customerId },
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to fetch customer by id.",
                AppEnum.LogLevel.Error,
                nameof(CustomerRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = tenantId,
                    ["CustomerId"] = customerId
                });
            throw;
        }
    }

    public async Task<PagedCustomersDto> ListAsync(CustomerListQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                "dbo.usp_Customer_List",
                new { query.TenantId, query.PageNumber, query.PageSize },
                commandType: CommandType.StoredProcedure);

            var items = (await multi.ReadAsync<CustomerDto>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PagedCustomersDto
            {
                Items = items,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Failed to list customers.",
                AppEnum.LogLevel.Error,
                nameof(CustomerRepository),
                ex.ToString(),
                new Dictionary<string, object>
                {
                    ["TenantId"] = query.TenantId,
                    ["PageNumber"] = query.PageNumber,
                    ["PageSize"] = query.PageSize
                });
            throw;
        }
    }
}
