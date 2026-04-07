using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Background;

// Implement the ITaskBackgroundProcessor interface (which you should create in Domain)
public class PostTaskCreationWorker : BackgroundService, ITaskBackgroundProcessor
{
    private readonly ILogger<PostTaskCreationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    // Thread-safe queue to hold task IDs waiting to be processed
    private readonly ConcurrentQueue<Guid> _taskQueue = new();

    public PostTaskCreationWorker(ILogger<PostTaskCreationWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    // This method is called by TaskService via the interface
    public void QueueTaskProcessing(Guid taskId)
    {
        _logger.LogInformation($"Task {taskId} added to background queue.");
        _taskQueue.Enqueue(taskId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Task Worker is starting and monitoring the queue.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Try to pull a task ID from the queue
            if (_taskQueue.TryDequeue(out var taskId))
            {
                try
                {
                    await ProcessTaskInternalAsync(taskId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing background task {taskId}");
                }
            }

            // Check the queue every 1 second
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessTaskInternalAsync(Guid taskId)
    {
        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();

        var task = await repo.GetByIdAsync(taskId);
        if (task != null)
        {
            _logger.LogInformation($"Background Processing: {task.Title}");

            // Appending text to prove it worked
            task.Description += " (Verified by Background Service)";

            await repo.SaveChangesAsync();
        }
    }
}