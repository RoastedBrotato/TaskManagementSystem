using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskItemStatus Status { get; set; }
        public int? AssignedUserId { get; set; }
        public User AssignedUser { get; set; }
    }

    public enum TaskItemStatus
    {
        Pending,
        InProgress,
        Completed
    }
}