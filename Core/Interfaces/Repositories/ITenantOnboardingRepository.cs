using Core.DTOs.TenantOnboarding;

namespace Core.Interfaces.Repositories;

public interface ITenantOnboardingRepository
{
    Task<CreateTenantWithOwnerResultDto> CreateTenantWithOwnerAsync(CreateTenantWithOwnerRequestDto request);
}
