using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;


    public TaskItem(string title, string description, Guid userId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        UserId = userId;
        Status = TaskItemStatus.Pending;
        Priority = TaskItemPriority.Medium;
        CreatedAt = DateTime.UtcNow;
    }
    //  constructor for migrations/queries for EF Core 
    public TaskItem() { }

}