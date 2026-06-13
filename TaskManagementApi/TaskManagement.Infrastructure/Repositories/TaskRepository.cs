using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository
    : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<TaskItem>> GetByUserIdAsync(Guid userId)
    {
        return await Context.Tasks
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdWithUserAsync(Guid id)
    {
        return await Context.Tasks
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsTodayAsync(
    Guid userId,
    string title)
    {
        return await Context.Tasks.AnyAsync(x =>
            x.UserId == userId &&
            x.Title == title &&
            x.CreatedAt.Date == DateTime.UtcNow.Date);
    }
}