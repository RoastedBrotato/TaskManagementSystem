import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TaskFormComponent } from '../task-form/task-form.component';
import { TaskStateService } from '../../../core/services/task-state.service';
import { Task, TaskStatus } from '../../../core/models/task.model';

// Material imports
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatTooltipModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule
  ]
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  users: any[] = [];
  isLoading = false;
  isEditingStatus = false;
  displayedColumns: string[] = ['title', 'description', 'dueDate', 'status', 'assignedUser', 'actions'];
  
  // Update status enum values to match your backend
  statuses = [
    { value: 0, text: 'Pending' },
    { value: 1, text: 'In Progress' },
    { value: 2, text: 'Completed' }
  ];

  constructor(
    private taskService: TaskService,
    private userService: UserService,
    private dialog: MatDialog,
    private taskState: TaskStateService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    
    // Subscribe to the shared task state
    this.taskState.tasks$.subscribe(tasks => {
      this.tasks = tasks;
    });
    
    this.taskState.loading$.subscribe(loading => {
      this.isLoading = loading;
    });
    
    // Initial load
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe({
      next: () => {
        // Tasks will be loaded via taskState subscription
      },
      error: (error) => {
        console.error('Error loading tasks:', error);
      }
    });
  }

  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
      },
      error: (error) => {
        console.error('Error loading users:', error);
      }
    });
  }

  getUserName(userId: number): string {
    if (!userId) return 'Unassigned';
    const user = this.users.find(u => u.id === userId);
    return user ? user.username : 'Unknown User';
  }

  getStatusText(statusValue: number): string {
    const status = this.statuses.find(s => s.value === statusValue);
    return status ? status.text : String(statusValue);
  }

  updateTaskStatus(task: Task, newStatus: number): void {
    // For the All Tasks view, use the full update endpoint so that admin can update status even if they're not the owner.
    const updateRequest = {
      title: task.title,
      description: task.description,
      dueDate: task.dueDate,
      status: newStatus,
      assignedUserId: task.assignedUserId
    };
    
    this.taskService.updateTask(task.id, updateRequest).subscribe({
      next: () => {
        console.log(`Task ${task.id} status updated to ${newStatus}`);
        task.status = newStatus; // update local value
        // Optionally exit editing mode here if using a per–task editing flag
      },
      error: (error) => {
        console.error('Error updating task status:', error);
      }
    });
  }

  openTaskForm(task?: Task): void {
    const dialogRef = this.dialog.open(TaskFormComponent, {
      width: '500px',
      data: { task, users: this.users }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (task) {
          // Update existing task
          this.taskService.updateTask(task.id, result).subscribe({
            next: () => {
              // No need to call loadTasks here as the taskState subscription will update the view
            },
            error: (error) => {
              console.error('Error updating task:', error);
            }
          });
        } else {
          // Create new task
          this.taskService.createTask(result).subscribe({
            next: () => {
              // No need to call loadTasks here as the taskState subscription will update the view
            },
            error: (error) => {
              console.error('Error creating task:', error);
            }
          });
        }
      }
    });
  }

  deleteTask(taskId: number): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(taskId).subscribe({
        next: () => {
          // No need to manually filter tasks - the taskState subscription will handle it
        },
        error: (error) => {
          console.error('Error deleting task:', error);
        }
      });
    }
  }
}