using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Domain.Enums;
using BCrypt.Net;

namespace TaskManagement.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash)) return null;

        try
        {
      
            var token = _tokenService.CreateToken(user);
            var profile = new UserProfileDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.CreatedAt);
            return new AuthResponseDto(token, profile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("********** JWT GENERATION FAILED **********");
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            Console.WriteLine("*******************************************");
            return null;
        }
    }

    public async Task<UserProfileDto> RegisterAsync(RegisterUserDto registerDto)
    {
        if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
            throw new Exception("User with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserProfileDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.CreatedAt);
    }

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserProfileDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.CreatedAt);
    }
}