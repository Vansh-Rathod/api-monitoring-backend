using GenericServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AppEnum = Core.Enums.Enum;

namespace GenericServices.LoggingService;

public class LoggingService : ILoggingService
{
    private readonly IConfiguration _configuration;

    public LoggingService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task LogAsync(
        string message,
        AppEnum.LogLevel level = AppEnum.LogLevel.Info,
        string source = "Application",
        string? exception = null,
        Dictionary<string, object>? additionalData = null)
    {
        try
        {
            var folderPath = _configuration["LoggerSettings:LogFolderPath"];
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                folderPath = Path.Combine(AppContext.BaseDirectory, "logs");
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, $"log_{DateTime.UtcNow:dd-MM-yyyy}.txt");
            var logEntry = new
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow.ToString("o"),
                Level = level.ToString(),
                Source = source,
                Message = message,
                Exception = exception,
                AdditionalData = additionalData
            };

            var logContent = JsonConvert.SerializeObject(logEntry, Formatting.Indented);
            await File.AppendAllTextAsync(filePath, logContent + Environment.NewLine + new string('-', 80) + Environment.NewLine);
        }
        catch (Exception logException)
        {
            Console.WriteLine($"Logging failed: {logException.Message}");
        }
    }
}
