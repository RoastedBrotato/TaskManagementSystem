import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';

// Material imports
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-user-tasks',
  templateUrl: './user-tasks.component.html',
  styleUrls: ['./user-tasks.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatIconModule
  ]
})
export class UserTasksComponent implements OnInit {
  tasks: any[] = [];
  isLoading = false;
  displayedColumns: string[] = ['title', 'description', 'dueDate', 'status'];
  
  statuses = [
    { value: 'Todo', text: 'To Do' },
    { value: 'InProgress', text: 'In Progress' },
    { value: 'Completed', text: 'Completed' }
  ];

  constructor(
    private taskService: TaskService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadUserTasks();
  }

  loadUserTasks(): void {
    const currentUser = this.authService.currentUserValue;
    if (currentUser) {
      this.isLoading = true;
      this.taskService.getUserTasks().subscribe({
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
  }

  updateTaskStatus(task: any, newStatus: string): void {
    const updatedTask = { ...task, status: newStatus };
    this.taskService.updateTask(task.id, updatedTask).subscribe({
      next: () => {
        task.status = newStatus;
      },
      error: (error) => {
        console.error('Error updating task status:', error);
      }
    });
  }
}