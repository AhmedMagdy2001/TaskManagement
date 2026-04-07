using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;
    ITaskBackgroundProcessor _backgroundProcessor; //for background processing 
    private readonly IDistributedCache _cache; // For Redis integration 

    public TaskService(ITaskRepository taskRepository, IDistributedCache cache , ITaskBackgroundProcessor backgroundProcessor)
    {
        _taskRepository = taskRepository;
        _cache = cache;
        _backgroundProcessor = backgroundProcessor;
    }

    public async Task<IEnumerable<TaskItemDto>> GetAllTasksForUserAsync(Guid userId)
    {
       //  Sort tasks by priority first, then by creation date
        // This sorting is handled within our Repository implementation.
        var tasks = await _taskRepository.GetByUserIdAsync(userId);

        return tasks.Select(t => new TaskItemDto(
            t.Id, t.Title, t.Description, t.Status.ToString(),
            t.Priority.ToString(), t.CreatedAt, t.UserId));
    }

    public async Task<TaskItemDto?> GetTaskByIdAsync(Guid id, Guid userId)
    {
        string cacheKey = $"task_{id}";

        //  Check cache first
        var cachedTask = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedTask))
        {
            var cachedDto = JsonSerializer.Deserialize<TaskItemDto>(cachedTask);
            //  Ensure user only views their own tasks 
            if (cachedDto?.UserId == userId) return cachedDto;
        }

        var task = await _taskRepository.GetByIdAsync(id);

       // Task must exist and belong to the user 
        if (task == null || task.UserId != userId) return null;

        var dto = new TaskItemDto(
            task.Id, task.Title, task.Description, task.Status.ToString(),
            task.Priority.ToString(), task.CreatedAt, task.UserId);

       //  On first request, save to Redis
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return dto;
    }

    public async Task<TaskItemDto?> CreateTaskAsync(Guid userId, CreateTaskDto dto)
    {
        
        if (await _taskRepository.DoesTaskExistTodayAsync(userId, dto.Title))
        {
            throw new Exception("A task with this title already exists for today.");
        }

        // Use the constructor we defined in TaskItem
        var task = new TaskItem(dto.Title, dto.Description, userId)
        {
            Priority = (TaskItemPriority)dto.Priority
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        //intiate background worker (first and forget)
        _backgroundProcessor.QueueTaskProcessing(task.Id);

        return new TaskItemDto(
            task.Id, task.Title, task.Description, task.Status.ToString(),
            task.Priority.ToString(), task.CreatedAt, task.UserId);
    }

    public async Task<bool> UpdateStatusAsync(Guid taskId, Guid userId, TaskItemStatus newStatus)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

     
        if (task == null || task.UserId != userId) return false;

        task.Status = newStatus;
        _taskRepository.Update(task);

        var result = await _taskRepository.SaveChangesAsync();

        if (result)
        {
           
            await _cache.RemoveAsync($"task_{taskId}");
        }

        return result;
    }
}