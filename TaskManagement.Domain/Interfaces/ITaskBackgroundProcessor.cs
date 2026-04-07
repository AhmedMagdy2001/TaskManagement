namespace TaskManagement.Domain.Interfaces;

public interface ITaskBackgroundProcessor
{
    void QueueTaskProcessing(Guid taskId);
}