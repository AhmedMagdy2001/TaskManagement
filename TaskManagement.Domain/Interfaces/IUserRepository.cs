using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync(); //  Admin can view all users
    Task AddAsync(User user);
    void Update(User user);
    void Delete(User user); //  Admin can delete users
    Task<bool> SaveChangesAsync();
}