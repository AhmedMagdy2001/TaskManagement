namespace TaskManagement.Application.Interfaces.Background;

public interface ITaskQueue
{
    void Enqueue(Guid taskId);

    Task<Guid> DequeueAsync(CancellationToken cancellationToken);
}