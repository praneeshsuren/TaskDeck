using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskDeck.Application.Interfaces;
using TaskDeck.Infrastructure.Persistence;
using TaskDeck.Infrastructure.Repositories;
using TaskDeck.Infrastructure.Services;

namespace TaskDeck.Infrastructure;

/// <summary>
/// Dependency injection extensions for the Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<TaskDeckDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(TaskDeckDbContext).Assembly.FullName)));

        // Add repositories
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Add services
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
