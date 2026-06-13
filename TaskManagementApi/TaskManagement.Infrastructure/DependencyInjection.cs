using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Interfaces.Background;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Infrastructure.Background;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddInfrastructure(
            this IServiceCollection services)
    {
        services.AddScoped<IUserRepository,
            UserRepository>();

        services.AddScoped<ITaskRepository,
            TaskRepository>();

        services.AddScoped<IJwtService,
            JwtService>();

        services.AddScoped<ICacheService,
            RedisCacheService>();

        services.AddSingleton<ITaskQueue,
            TaskQueue>();

        services.AddHostedService<
            TaskProcessingWorker>();

        return services;
    }
}