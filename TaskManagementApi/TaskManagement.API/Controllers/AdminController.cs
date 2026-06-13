using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _users;
    ILogger<AdminController> _logger;

    public AdminController(
        IUserRepository users, ILogger<AdminController> logger)
    {
        _users = users;
        _logger = logger;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(
            await _users.GetAllAsync());
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(
        CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    request.Password),
            Role =
                Enum.Parse<UserRole>(
                    request.Role)
        };

        await _users.AddAsync(user);

        _logger.LogInformation(
            "Admin created user {UserEmail}",
            user.Email);

        return Ok();
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(
        Guid id)
    {
        var user =
            await _users.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        await _users.DeleteAsync(user);
        _logger.LogInformation(
            "Admin deleted user {UserEmail}",
            user.Email);

        return NoContent();
    }
}