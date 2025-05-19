import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-user-tasks',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="user-tasks-container">
      <h3>My Tasks</h3>
      
      <table mat-table [dataSource]="tasks" class="mat-elevation-z2">
        <!-- Title Column -->
        <ng-container matColumnDef="title">
          <th mat-header-cell *matHeaderCellDef>Title</th>
          <td mat-cell *matCellDef="let task">{{task.title}}</td>
        </ng-container>
        
        <!-- Description Column -->
        <ng-container matColumnDef="description">
          <th mat-header-cell *matHeaderCellDef>Description</th>
          <td mat-cell *matCellDef="let task">{{task.description}}</td>
        </ng-container>
        
        <!-- Status Column -->
        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let task">{{task.status}}</td>
        </ng-container>
        
        <!-- Due Date Column -->
        <ng-container matColumnDef="dueDate">
          <th mat-header-cell *matHeaderCellDef>Due Date</th>
          <td mat-cell *matCellDef="let task">{{task.dueDate | date}}</td>
        </ng-container>
        
        <!-- Actions Column -->
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Actions</th>
          <td mat-cell *matCellDef="let task">
            <button mat-icon-button color="primary" (click)="editTask(task)">
              <mat-icon>edit</mat-icon>
            </button>
            <button mat-icon-button color="warn" (click)="deleteTask(task.id)">
              <mat-icon>delete</mat-icon>
            </button>
          </td>
        </ng-container>
        
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>
      
      <div *ngIf="tasks.length === 0" class="no-tasks">
        You don't have any tasks yet.
      </div>
    </div>
  `,
  styles: [`
    .user-tasks-container {
      padding: 16px;
    }
    
    table {
      width: 100%;
    }
    
    .no-tasks {
      margin-top: 20px;
      text-align: center;
      color: #777;
    }
  `]
})
export class UserTasksComponent implements OnInit {
  tasks: any[] = [];
  displayedColumns: string[] = ['title', 'description', 'status', 'dueDate', 'actions'];
  
  constructor(
    private taskService: TaskService,
    private authService: AuthService
  ) {}
  
  ngOnInit(): void {
    this.loadUserTasks();
  }
  
  loadUserTasks(): void {
    const userId = this.authService.currentUserValue?.id;
    if (userId) {
      this.taskService.getUserTasks().subscribe(
        tasks => this.tasks = tasks,
        error => console.error('Error fetching user tasks:', error)
      );
    }
  }
  
  editTask(task: any): void {
    // Implement edit functionality
    console.log('Edit task:', task);
  }
  
  deleteTask(taskId: number): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(taskId).subscribe(
        () => {
          this.tasks = this.tasks.filter(task => task.id !== taskId);
        },
        error => console.error('Error deleting task:', error)
      );
    }
  }
}