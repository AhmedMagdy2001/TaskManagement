namespace TaskManagement.Application.DTOs;

public record TaskItemDto(
    Guid Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime CreatedAt,
    Guid UserId);

public record CreateTaskDto(string Title, string Description, int Priority);
public record UpdateTaskStatusDto(int Status);