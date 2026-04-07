using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    // Returns tasks sorted by priority, then creation date
    Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId);
    Task AddAsync(TaskItem task);
    void Update(TaskItem task);
    void Delete(TaskItem task);
    //  Prevent duplicate titles on the same day for the same user
    Task<bool> DoesTaskExistTodayAsync(Guid userId, string title);
    Task<bool> SaveChangesAsync();
}