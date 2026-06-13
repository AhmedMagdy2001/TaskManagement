using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces.Background;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Background;

public class TaskProcessingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITaskQueue _queue;
    private readonly ILogger<TaskProcessingWorker> _logger;
    private readonly IDistributedCache _cache;

    public TaskProcessingWorker(
        IServiceProvider serviceProvider,
        ITaskQueue queue,
        ILogger<TaskProcessingWorker> logger,
        IDistributedCache cache
        )

    {
        _serviceProvider = serviceProvider;
        _queue = queue;
        _logger = logger;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            await Task.Delay(15000, stoppingToken);


            var taskId =
                await _queue.DequeueAsync(stoppingToken);

            using var scope =
                _serviceProvider.CreateScope();

            var context =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            var task =
                await context.Tasks
                    .FirstOrDefaultAsync(
                        x => x.Id == taskId,
                        stoppingToken);

            if (task == null)
                continue;

            _logger.LogInformation(
                "Processing task {TaskId}",
                taskId);



            task.Status = TasksStatus.InProgress;

            await context.SaveChangesAsync(stoppingToken);

            await _cache.RemoveAsync($"task:{taskId}");

            _logger.LogInformation(
                "Task {TaskId} processed",
                taskId);
        }
    }
}