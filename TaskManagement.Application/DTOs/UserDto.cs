namespace TaskManagement.Application.DTOs;

public record RegisterUserDto(string Name, string Email, string Password);
public record LoginDto(string Email, string Password);
public record UserProfileDto(Guid Id, string Name, string Email, string Role, DateTime CreatedAt);
public record AuthResponseDto(string Token, UserProfileDto User);