import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { TaskStatus } from '../../../core/models/task.model';
import { StatusDialogComponent } from '../status-dialog/status-dialog.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

// Material imports
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

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
    MatIconModule,
    MatButtonModule,
    MatDialogModule // Add this
  ]
})
export class UserTasksComponent implements OnInit {
  tasks: any[] = [];
  isLoading = false;
  editingTask: any = null;
  displayedColumns: string[] = ['title', 'description', 'dueDate', 'status'];
  
  // Update statuses to match the backend enum (TaskItemStatus)
  statuses = [
    { value: 0, text: 'Pending' },
    { value: 1, text: 'In Progress' },
    { value: 2, text: 'Completed' }
  ];

  constructor(
    private taskService: TaskService,
    private authService: AuthService,
    private dialog: MatDialog // Add this
  ) {}

  ngOnInit(): void {
    this.loadUserTasks();
  }

  loadUserTasks(): void {
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

  getStatusText(statusValue: number): string {
    const status = this.statuses.find(s => s.value === statusValue);
    return status ? status.text : String(statusValue);
  }
  
  startEditingStatus(task: any): void {
    const dialogRef = this.dialog.open(StatusDialogComponent, {
      width: '300px',
      data: { currentStatus: task.status }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined) {
        this.updateTaskStatus(task, result);
      }
    });
  }
  
  stopEditingStatus(): void {
    this.editingTask = null;
  }

  updateTaskStatus(task: any, newStatus: number): void {
    this.taskService.updateTaskStatus(task.id, newStatus).subscribe({
      next: () => {
        console.log(`Task ${task.id} status updated to ${newStatus}`);
        task.status = newStatus;
      },
      error: (error) => {
        console.error('Error updating task status:', error);
      }
    });
  }
}