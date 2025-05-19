using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Core.Services
{
    public interface ITaskService
    {
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task UpdateTaskAsync(TaskItem task);
        Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus status, int userId);
        Task DeleteTaskAsync(int id);
    }
}