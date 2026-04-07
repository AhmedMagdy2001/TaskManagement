using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService) => _userService = userService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await _userService.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);
        if (result == null) return Unauthorized("Invalid credentials");
        return Ok(result);
    }
}