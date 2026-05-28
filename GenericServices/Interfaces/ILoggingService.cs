using AppEnum = Core.Enums.Enum;

namespace GenericServices.Interfaces;

public interface ILoggingService
{
    Task LogAsync(
        string message,
        AppEnum.LogLevel level = AppEnum.LogLevel.Info,
        string source = "Application",
        string? exception = null,
        Dictionary<string, object>? additionalData = null);
}
