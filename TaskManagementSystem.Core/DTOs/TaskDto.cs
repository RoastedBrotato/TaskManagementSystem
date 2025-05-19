using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Core.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int? AssignedUserId { get; set; }
        public string AssignedUsername { get; set; }
    }

    public class CreateTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        public TaskItemStatus Status { get; set; }
    }
}