using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context) => _context = context;

    public async Task<TaskItem?> GetByIdAsync(Guid id) =>
        await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Tasks
            .Where(t => t.UserId == userId)
            // Implementation of sorting requirement
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DoesTaskExistTodayAsync(Guid userId, string title)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Tasks.AnyAsync(t =>
            t.UserId == userId &&
            t.Title.ToLower() == title.ToLower() &&
            t.CreatedAt.Date == today);
    }

    public async Task AddAsync(TaskItem task) => await _context.Tasks.AddAsync(task);
    public void Update(TaskItem task) => _context.Tasks.Update(task);
    public void Delete(TaskItem task) => _context.Tasks.Remove(task);
    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
}