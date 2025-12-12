using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace TaskDeck.Application;

/// <summary>
/// Dependency injection extensions for the Application layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the service collection
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // Add FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
