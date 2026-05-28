using GenericServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices;

public static class DependencyInjection
{
    public static IServiceCollection AddGenericServices(this IServiceCollection services)
    {
        services.AddScoped<ILoggingService, LoggingService.LoggingService>();
        return services;
    }
}
