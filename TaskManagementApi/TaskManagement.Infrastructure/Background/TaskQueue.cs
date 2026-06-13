using System.Collections.Concurrent;
using TaskManagement.Application.Interfaces.Background;

namespace TaskManagement.Infrastructure.Background;

public class TaskQueue : ITaskQueue
{
    private readonly ConcurrentQueue<Guid> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void Enqueue(Guid taskId)
    {
        _queue.Enqueue(taskId);

        _signal.Release();
    }

    public async Task<Guid> DequeueAsync(
        CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);

        _queue.TryDequeue(out var taskId);

        return taskId;
    }
}