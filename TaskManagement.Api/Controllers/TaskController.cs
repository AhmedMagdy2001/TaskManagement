using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService) => _taskService = taskService;

    [HttpGet]
    public async Task<IActionResult> GetMyTasks()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _taskService.GetAllTasksForUserAsync(userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var task = await _taskService.GetTaskByIdAsync(id, userId);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _taskService.CreateTaskAsync(userId, dto);
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TaskItemStatus status)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _taskService.UpdateStatusAsync(id, userId, status);
        if (!result) return BadRequest("Could not update task.");
        return NoContent();
    }
}