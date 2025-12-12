using TaskDeck.Application.Interfaces;

namespace TaskDeck.Api.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds API-specific services to the service collection
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Add any API-specific services here
        return services;
    }
}
