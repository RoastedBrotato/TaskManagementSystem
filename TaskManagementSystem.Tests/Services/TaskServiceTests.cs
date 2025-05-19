using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Services;

namespace TaskManagementSystem.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _taskService = new TaskService(_mockTaskRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_UserOwnsTask_ReturnsTrue()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            TaskItemStatus newStatus = TaskItemStatus.Completed;

            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Status = TaskItemStatus.InProgress,
                AssignedUserId = userId // Task is assigned to the user
            };

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus, userId);

            // Assert
            Assert.True(result);
            Assert.Equal(newStatus, task.Status); // Status was updated
            
            // Verify repository calls
            _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(repo => repo.UpdateAsync(task), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_UserDoesNotOwnTask_ReturnsFalse()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            int differentUserId = 3;
            TaskItemStatus originalStatus = TaskItemStatus.InProgress;
            TaskItemStatus newStatus = TaskItemStatus.Completed;

            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Status = originalStatus,
                AssignedUserId = differentUserId // Task is assigned to a different user
            };

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus, userId);

            // Assert
            Assert.False(result);
            Assert.Equal(originalStatus, task.Status); // Status was not updated
            
            // Verify repository calls
            _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_TaskDoesNotExist_ReturnsFalse()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            TaskItemStatus newStatus = TaskItemStatus.Completed;

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus, userId);

            // Assert
            Assert.False(result);
            
            // Verify repository calls
            _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task GetTasksByUserIdAsync_ReturnsTasks()
        {
            // Arrange
            int userId = 1;
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", AssignedUserId = userId },
                new TaskItem { Id = 2, Title = "Task 2", AssignedUserId = userId }
            };

            _mockTaskRepository.Setup(repo => repo.GetTasksByUserIdAsync(userId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetTasksByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Id == 1);
            Assert.Contains(result, t => t.Id == 2);
            
            // Verify repository call
            _mockTaskRepository.Verify(repo => repo.GetTasksByUserIdAsync(userId), Times.Once);
        }
    }
}
