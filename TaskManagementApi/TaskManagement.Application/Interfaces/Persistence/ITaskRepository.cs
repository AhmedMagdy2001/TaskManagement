using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Persistence;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<List<TaskItem>> GetByUserIdAsync(Guid userId);

    Task<TaskItem?> GetByIdWithUserAsync(Guid id);

    Task<bool> ExistsTodayAsync(
    Guid userId,
    string title);
}