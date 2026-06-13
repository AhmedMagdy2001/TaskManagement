using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}