using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Task Configuration
        modelBuilder.Entity<TaskItem>(entity => {
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(t => t.UserId);
        });

        // Seed Admin User
     
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = Guid.Parse("B8E7D5A1-C2B3-4E5F-A6B7-C8D9E0F1A2B3"),
            Name = "System Admin",
            Email = "admin@example.com",
            PasswordHash = "$2a$11$wV8c.y7jYEamk6yjHnzB6uREzpo7a1Lgbci0BKGW3ZLI.xxtslClO",
            Role = UserRole.Admin,
            // FIX: Use a fixed date so the model stays consistent
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}