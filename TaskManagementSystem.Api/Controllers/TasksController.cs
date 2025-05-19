using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.Core.DTOs;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Services;

namespace TaskManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/Tasks
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            var taskDtos = tasks.Select(MapToTaskDto);
            return Ok(taskDtos);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            // Check if user is authorized to view this task
            if (!isAdmin && task.AssignedUserId != userId)
            {
                return Forbid();
            }

            return Ok(MapToTaskDto(task));
        }

        // GET: api/Tasks/user
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetUserTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            var taskDtos = tasks.Select(MapToTaskDto);
            return Ok(taskDtos);
        }

        // POST: api/Tasks
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskRequest request)
        {
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Status = request.Status,
                AssignedUserId = request.AssignedUserId
            };

            var createdTask = await _taskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, MapToTaskDto(createdTask));
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
        {
            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            // If not admin, can only update status of assigned tasks
            if (!isAdmin)
            {
                if (existingTask.AssignedUserId != userId)
                {
                    return Forbid();
                }

                // Only update status
                existingTask.Status = request.Status;
            }
            else
            {
                // Admin can update everything
                existingTask.Title = request.Title;
                existingTask.Description = request.Description;
                existingTask.DueDate = request.DueDate;
                existingTask.Status = request.Status;
                existingTask.AssignedUserId = request.AssignedUserId;
            }

            await _taskService.UpdateTaskAsync(existingTask);
            return NoContent();
        }

        // PUT: api/Tasks/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskStatusUpdateRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _taskService.UpdateTaskStatusAsync(id, request.Status, userId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }

        // Helper method to map TaskItem to TaskDto
        private TaskDto MapToTaskDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status.ToString(),
                AssignedUserId = task.AssignedUserId,
                AssignedUsername = task.AssignedUser?.Username
            };
        }
    }

    public class TaskStatusUpdateRequest
    {
        public TaskItemStatus Status { get; set; }
    }

    public class CreateTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskItemStatus Status { get; set; }
        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskItemStatus Status { get; set; }
        public int? AssignedUserId { get; set; }
    }
}