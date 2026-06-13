using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Persistence;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class Repository<T> : IRepository<T>
    where T : class
{
    protected readonly AppDbContext Context;

    public Repository(AppDbContext context)
    {
        Context = context;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);

        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        Context.Set<T>().Remove(entity);

        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        Context.Set<T>().Update(entity);

        await Context.SaveChangesAsync();
    }
}