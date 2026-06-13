using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TasksStatus Status { get; set; } =
        TasksStatus.Pending;

    public TaskPriority Priority { get; set; } =
        TaskPriority.Medium;

    public Guid UserId { get; set; }

    public User? User { get; set; }
}