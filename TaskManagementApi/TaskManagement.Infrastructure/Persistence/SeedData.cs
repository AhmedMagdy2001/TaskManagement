using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        var adminExists = await context.Users
            .AnyAsync(x => x.Role == UserRole.Admin);

        if (adminExists)
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "System Admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin
        };

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}