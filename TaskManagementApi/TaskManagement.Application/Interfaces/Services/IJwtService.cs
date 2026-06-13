using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}