namespace Core.DTOs.TenantOnboarding;

public class CreateTenantWithOwnerRequestDto
{
    public string TenantCode { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string? OwnerFirstName { get; set; }
    public string? OwnerLastName { get; set; }
    public string? PasswordHash { get; set; }
}
