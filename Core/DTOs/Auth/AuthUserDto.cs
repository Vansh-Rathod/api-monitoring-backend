namespace Core.DTOs.Auth;

public class AuthUserDto
{
    public long UserId { get; set; }
    public long TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Viewer";
    public bool IsActive { get; set; }
}
