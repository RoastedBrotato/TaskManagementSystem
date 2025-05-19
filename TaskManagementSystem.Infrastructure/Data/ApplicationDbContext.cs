using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedUserId);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Add an admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Password = "admin123", // In production, use password hashing
                Email = "admin@example.com",
                Role = "Admin"
            });

            // Add a regular user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 2,
                Username = "user",
                Password = "user123", // In production, use password hashing
                Email = "user@example.com",
                Role = "User"
            });

            // Add tasks
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Complete project proposal",
                    Description = "Create a detailed project proposal document",
                    DueDate = DateTime.Now.AddDays(7),
                    Status = TaskStatus.Pending,
                    AssignedUserId = 1
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Review code changes",
                    Description = "Review pull request #42",
                    DueDate = DateTime.Now.AddDays(2),
                    Status = TaskStatus.InProgress,
                    AssignedUserId = 2
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Fix login bug",
                    Description = "Fix the authentication issue reported by QA",
                    DueDate = DateTime.Now.AddDays(1),
                    Status = TaskStatus.Pending,
                    AssignedUserId = 2
                }
            );
        }
    }
}