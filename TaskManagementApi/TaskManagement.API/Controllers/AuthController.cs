using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IJwtService _jwt;
    protected readonly AppDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository users,
        IJwtService jwt,
        AppDbContext context,
        ILogger<AuthController> logger)
    {
        _users = users;
        _jwt = jwt;
        _context = context;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var exists =
            await _users.GetByEmailAsync(
                request.Email);

        if (exists != null)
            return BadRequest(
                "Email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    request.Password),
            Role = UserRole.User
        };

        await _users.AddAsync(user);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
     LoginRequest request)
    {
        var user =
            await _users.GetByEmailAsync(
                request.Email);

        if (user == null)
        {
            _logger.LogWarning(
                "Login failed. User not found for email {Email}",
                request.Email);

            return Unauthorized();
        }

        var valid =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

        if (!valid)
        {
            _logger.LogWarning(
                "Login failed. Invalid password for email {Email}",
                request.Email);

            return Unauthorized();
        }

        var accessToken =
            _jwt.GenerateToken(user);

        var refreshTokenValue =
            _jwt.GenerateRefreshToken();

        var refreshToken =
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "User {UserId} ({Email}) logged in successfully",
            user.Id,
            user.Email);

        return Ok(
            new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                Email = user.Email,
                Role = user.Role.ToString()
            });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    RefreshTokenRequest request)
    {
        var token =
            await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    x => x.Token ==
                         request.RefreshToken);

        if (token == null)
            return NotFound();

        token.IsRevoked = true;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Logged out successfully."
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
    RefreshTokenRequest request)
    {
        var refreshToken =
            await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.Token ==
                         request.RefreshToken);

        if (refreshToken == null)
            return Unauthorized("Invalid refresh token.");

        if (refreshToken.IsRevoked)
            return Unauthorized("Refresh token revoked.");

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Refresh token expired.");

        var accessToken =
            _jwt.GenerateToken(
                refreshToken.User);

        return Ok(new
        {
            accessToken
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    System.Security.Claims
                        .ClaimTypes.NameIdentifier)!
                    .Value);

        var user =
            await _users.GetByIdAsync(userId);

        return Ok(user);
    }
}