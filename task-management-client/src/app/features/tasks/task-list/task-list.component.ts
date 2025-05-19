import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TaskFormComponent } from '../task-form/task-form.component';

// Material imports
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

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
    MatTooltipModule
  ]
})
export class TaskListComponent implements OnInit {
  tasks: any[] = [];
  users: any[] = [];
  isLoading = false;
  displayedColumns: string[] = ['title', 'description', 'dueDate', 'status', 'assignedUser', 'actions'];
  
  statuses = [
    { value: 'Todo', text: 'To Do' },
    { value: 'InProgress', text: 'In Progress' },
    { value: 'Completed', text: 'Completed' }
  ];

  constructor(
    private taskService: TaskService,
    private userService: UserService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadUsers();
  }

  loadTasks(): void {
    this.isLoading = true;
    this.taskService.getTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading tasks:', error);
        this.isLoading = false;
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

  getStatusText(statusValue: string): string {
    const status = this.statuses.find(s => s.value === statusValue);
    return status ? status.text : statusValue;
  }

  openTaskForm(task?: any): void {
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
              this.loadTasks();
            },
            error: (error) => {
              console.error('Error updating task:', error);
            }
          });
        } else {
          // Create new task
          this.taskService.createTask(result).subscribe({
            next: () => {
              this.loadTasks();
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
          this.tasks = this.tasks.filter(task => task.id !== taskId);
        },
        error: (error) => {
          console.error('Error deleting task:', error);
        }
      });
    }
  }
}