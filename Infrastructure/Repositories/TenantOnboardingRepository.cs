using Core.DTOs.TenantOnboarding;
using Core.Interfaces.Repositories;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Data;
using System.Data;
using AppEnum = Core.Enums.Enum;

namespace Infrastructure.Repositories;

public class TenantOnboardingRepository : ITenantOnboardingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILoggingService _loggingService;

    public TenantOnboardingRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
    {
        _connectionFactory = connectionFactory;
        _loggingService = loggingService;
    }

    public async Task<CreateTenantWithOwnerResultDto> CreateTenantWithOwnerAsync(CreateTenantWithOwnerRequestDto request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@TenantCode", request.TenantCode);
            parameters.Add("@TenantName", request.TenantName);
            parameters.Add("@PlanType", request.PlanType);
            parameters.Add("@OwnerEmail", request.OwnerEmail);
            parameters.Add("@OwnerFirstName", request.OwnerFirstName);
            parameters.Add("@OwnerLastName", request.OwnerLastName);
            parameters.Add("@PasswordHash", request.PasswordHash);

            var result = await connection.QuerySingleAsync<CreateTenantWithOwnerResultDto>(
                "dbo.usp_Tenant_CreateWithOwner",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                message: "Failed to create tenant with owner.",
                level: AppEnum.LogLevel.Error,
                source: nameof(TenantOnboardingRepository),
                exception: ex.ToString(),
                additionalData: new Dictionary<string, object>
                {
                    ["TenantCode"] = request.TenantCode,
                    ["OwnerEmail"] = request.OwnerEmail
                });

            throw;
        }
    }
}
