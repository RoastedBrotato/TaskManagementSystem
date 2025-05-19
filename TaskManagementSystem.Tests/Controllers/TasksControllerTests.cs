using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Core.DTOs;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Services;
using UpdateTaskRequest = TaskManagementSystem.Api.Controllers.UpdateTaskRequest;
using CreateTaskRequest = TaskManagementSystem.Core.DTOs.CreateTaskRequest;

namespace TaskManagementSystem.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TasksController(_mockTaskService.Object);
        }

        private void SetupUserClaims(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetUserTasks_ReturnsTasksForCurrentUser()
        {
            // Arrange
            int userId = 1;
            SetupUserClaims(userId, "User");

            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Task 1",
                    Description = "Description 1",
                    Status = TaskItemStatus.Pending,
                    AssignedUserId = userId
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Task 2",
                    Description = "Description 2",
                    Status = TaskItemStatus.InProgress,
                    AssignedUserId = userId
                }
            };

            _mockTaskService.Setup(service => service.GetTasksByUserIdAsync(userId))
                .ReturnsAsync(tasks);

            // Act
            var actionResult = await _controller.GetUserTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);

            Assert.Equal(2, returnedTasks.Count());

            // Verify service call
            _mockTaskService.Verify(service => service.GetTasksByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetTasks_AdminUser_ReturnsAllTasks()
        {
            // Arrange
            SetupUserClaims(1, "Admin");

            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Task 1",
                    Description = "Description 1",
                    Status = TaskItemStatus.Pending,
                    AssignedUserId = 1
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Task 2",
                    Description = "Description 2",
                    Status = TaskItemStatus.InProgress,
                    AssignedUserId = 2
                }
            };

            _mockTaskService.Setup(service => service.GetAllTasksAsync())
                .ReturnsAsync(tasks);

            // Act
            var actionResult = await _controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);

            Assert.Equal(2, returnedTasks.Count());

            // Verify service call
            _mockTaskService.Verify(service => service.GetAllTasksAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTask_UserOwnsTask_ReturnsTask()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            SetupUserClaims(userId, "User");

            var task = new TaskItem
            {
                Id = taskId,
                Title = "Task 1",
                Description = "Description 1",
                Status = TaskItemStatus.Pending,
                AssignedUserId = userId // Task is assigned to the user
            };

            _mockTaskService.Setup(service => service.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var actionResult = await _controller.GetTask(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedTask = Assert.IsType<TaskDto>(okResult.Value);

            Assert.Equal(taskId, returnedTask.Id);
            Assert.Equal(task.Title, returnedTask.Title);

            // Verify service call
            _mockTaskService.Verify(service => service.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetTask_UserDoesNotOwnTask_ReturnsForbid()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            SetupUserClaims(userId, "User");

            var task = new TaskItem
            {
                Id = taskId,
                Title = "Task 1",
                Description = "Description 1",
                Status = TaskItemStatus.Pending,
                AssignedUserId = 3 // Task is assigned to a different user
            };

            _mockTaskService.Setup(service => service.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var actionResult = await _controller.GetTask(taskId);

            // Assert
            Assert.IsType<ForbidResult>(actionResult.Result);

            // Verify service call
            _mockTaskService.Verify(service => service.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetTask_AdminUser_ReturnsAnyTask()
        {
            // Arrange
            int taskId = 1;
            SetupUserClaims(1, "Admin");

            var task = new TaskItem
            {
                Id = taskId,
                Title = "Task 1",
                Description = "Description 1",
                Status = TaskItemStatus.Pending,
                AssignedUserId = 2 // Task is assigned to another user
            };

            _mockTaskService.Setup(service => service.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var actionResult = await _controller.GetTask(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedTask = Assert.IsType<TaskDto>(okResult.Value);

            Assert.Equal(taskId, returnedTask.Id);
            Assert.Equal(task.Title, returnedTask.Title);

            // Verify service call
            _mockTaskService.Verify(service => service.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task CreateTask_AdminUser_CreatesTask()
        {
            // Arrange
            SetupUserClaims(1, "Admin");

            var createRequest = new Api.Controllers.CreateTaskRequest
            {
                Title = "New Task",
                Description = "Description",
                DueDate = DateTime.Now.AddDays(7),
                AssignedUserId = 2
            };

            var createdTask = new TaskItem
            {
                Id = 1,
                Title = createRequest.Title,
                Description = createRequest.Description,
                DueDate = createRequest.DueDate,
                Status = TaskItemStatus.Pending,
                AssignedUserId = createRequest.AssignedUserId
            };

            _mockTaskService.Setup(service => service.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(createdTask);

            // Act
            var actionResult = await _controller.CreateTask(createRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnedTask = Assert.IsType<TaskDto>(createdAtActionResult.Value);

            Assert.Equal(createdTask.Id, returnedTask.Id);
            Assert.Equal(createRequest.Title, returnedTask.Title);
            Assert.Equal(createRequest.Description, returnedTask.Description);

            // Verify service call
            _mockTaskService.Verify(service => service.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_AdminUser_UpdatesAllFields()
        {
            // Arrange
            int taskId = 1;
            SetupUserClaims(1, "Admin");

            var updateRequest = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(14),
                Status = TaskItemStatus.InProgress, // Fixed: Use TaskItemStatus enum instead of string
                AssignedUserId = 3
            };

            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Original Task",
                Description = "Original Description",
                DueDate = DateTime.Now.AddDays(7),
                Status = TaskItemStatus.Pending,
                AssignedUserId = 2
            };

            _mockTaskService.Setup(service => service.GetTaskByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act
            var result = await _controller.UpdateTask(taskId, updateRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify service calls
            _mockTaskService.Verify(service => service.GetTaskByIdAsync(taskId), Times.Once);
            _mockTaskService.Verify(service => service.UpdateTaskAsync(It.Is<TaskItem>(t =>
                t.Title == updateRequest.Title &&
                t.Description == updateRequest.Description)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTask_RegularUser_OnlyUpdatesStatus()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            SetupUserClaims(userId, "User");

            var updateRequest = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(14),
                Status = TaskItemStatus.InProgress, // Fixed: Use TaskItemStatus enum instead of string
                AssignedUserId = 3
            };

            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Original Task",
                Description = "Original Description",
                DueDate = DateTime.Now.AddDays(7),
                Status = TaskItemStatus.Pending,
                AssignedUserId = userId // Task is assigned to the current user
            };

            _mockTaskService.Setup(service => service.GetTaskByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act
            var result = await _controller.UpdateTask(taskId, updateRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify service calls
            _mockTaskService.Verify(service => service.GetTaskByIdAsync(taskId), Times.Once);
            _mockTaskService.Verify(service => service.UpdateTaskAsync(It.Is<TaskItem>(t =>
                t.Title == existingTask.Title && // Original value preserved
                t.Description == existingTask.Description && // Original value preserved
                t.AssignedUserId == existingTask.AssignedUserId)), // Original value preserved
                Times.Once);
        }

        [Fact]
        public async Task DeleteTask_AdminUser_DeletesTask()
        {
            // Arrange
            int taskId = 1;
            SetupUserClaims(1, "Admin");

            var task = new TaskItem { Id = taskId, Title = "Test Task" };

            _mockTaskService.Setup(service => service.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify service calls
            _mockTaskService.Verify(service => service.GetTaskByIdAsync(taskId), Times.Once);
            _mockTaskService.Verify(service => service.DeleteTaskAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskStatus_UserOwnsTask_UpdatesStatus()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            SetupUserClaims(userId, "User");

            var statusUpdateRequest = new TaskStatusUpdateRequest
            {
                Status = TaskItemStatus.Completed
            };

            _mockTaskService.Setup(service => service.UpdateTaskStatusAsync(taskId, statusUpdateRequest.Status, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTaskStatus(taskId, statusUpdateRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify service call
            _mockTaskService.Verify(service => service.UpdateTaskStatusAsync(taskId, statusUpdateRequest.Status, userId), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskStatus_UserDoesNotOwnTask_ReturnsNotFound()
        {
            // Arrange
            int taskId = 1;
            int userId = 2;
            SetupUserClaims(userId, "User");

            var statusUpdateRequest = new TaskStatusUpdateRequest
            {
                Status = TaskItemStatus.Completed
            };

            _mockTaskService.Setup(service => service.UpdateTaskStatusAsync(taskId, statusUpdateRequest.Status, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateTaskStatus(taskId, statusUpdateRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            // Verify service call
            _mockTaskService.Verify(service => service.UpdateTaskStatusAsync(taskId, statusUpdateRequest.Status, userId), Times.Once);
        }
    }
}