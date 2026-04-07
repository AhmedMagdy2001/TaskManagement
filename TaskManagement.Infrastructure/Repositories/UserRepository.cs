using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _context.Users.ToListAsync(); // Admin View users list [cite: 59]

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user); //  Admin Create users [cite: 57]

    public void Update(User user) => _context.Users.Update(user);

    public void Delete(User user) => _context.Users.Remove(user); //  Admin Delete users [cite: 58]

    public async Task<bool> SaveChangesAsync() =>
        await _context.SaveChangesAsync() > 0;
}