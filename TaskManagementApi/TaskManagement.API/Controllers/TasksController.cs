using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces.Background;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _tasks;
    private readonly ICacheService _cache;
    private readonly ITaskQueue _queue;
    ILogger<TasksController> _logger;

    public TasksController(
        ITaskRepository tasks,
        ICacheService cache,
        ITaskQueue queue,
        ILogger<TasksController> logger)
    {
        _tasks = tasks;
        _cache = cache;
        _queue = queue;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTaskRequest request)
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    System.Security.Claims
                        .ClaimTypes.NameIdentifier)!
                    .Value);

        var duplicate =
            await _tasks.ExistsTodayAsync(
                userId,
                request.Title);

        if (duplicate)
        {
            _logger.LogWarning(
                "Duplicate task attempt: {Title} by User {UserId}",
                request.Title,
                userId);
            return Conflict(new { Message = "A task with this title already exists for today." });
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            UserId = userId,
            Priority =
                Enum.Parse<TaskPriority>(
                    request.Priority),
            Status = TasksStatus.Pending
        };

        await _tasks.AddAsync(task);


        _queue.Enqueue(task.Id);

        _logger.LogInformation(
            "Task created: {TaskId} by User {UserId}",
            task.Id,
            userId);

        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    System.Security.Claims
                        .ClaimTypes.NameIdentifier)!
                    .Value);

        return Ok(
            await _tasks.GetByUserIdAsync(
                userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var cacheKey = $"task:{id}";

        var cached =
            await _cache.GetAsync<TaskDto>(
                cacheKey);

        if (cached != null)
            return Ok(cached);

        var task =
            await _tasks.GetByIdWithUserAsync(
                id);

        if (task == null)
            return NotFound();

        var userId =
            Guid.Parse(
                User.FindFirst(
                    System.Security.Claims
                        .ClaimTypes.NameIdentifier)!
                    .Value);

        if (task.UserId != userId)
            return Forbid();

        var dto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            UserId = task.UserId,
            CreatedAt = task.CreatedAt
        };

        await _cache.SetAsync(
            cacheKey,
            dto);

        return Ok(dto);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateTaskStatusRequest request)
    {
        var task =
            await _tasks.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        var userId =
            Guid.Parse(
                User.FindFirst(
                    System.Security.Claims
                        .ClaimTypes.NameIdentifier)!
                    .Value);

        if (task.UserId != userId)
            return Forbid();

        task.Status =
            Enum.Parse<TasksStatus>(
                request.Status);

        await _tasks.UpdateAsync(task);

        await _cache.RemoveAsync(
            $"task:{id}");

        _logger.LogInformation(
        "Task {TaskId} updated to {Status}",
        task.Id,
        task.Status);

        return Ok(task);
    }
}