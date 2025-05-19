import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TaskService } from '../../../core/services/task.service';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="task-list-container">
      <h3>All Tasks</h3>
      <button mat-raised-button color="primary" class="add-button" (click)="addTask()">
        <mat-icon>add</mat-icon> Add Task
      </button>
      
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
        
        <!-- Assigned To Column -->
        <ng-container matColumnDef="assignedTo">
          <th mat-header-cell *matHeaderCellDef>Assigned To</th>
          <td mat-cell *matCellDef="let task">{{task.assignedToUser?.username || 'Unassigned'}}</td>
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
        No tasks available.
      </div>
    </div>
  `,
  styles: [`
    .task-list-container {
      padding: 16px;
    }
    
    .add-button {
      margin-bottom: 16px;
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
export class TaskListComponent implements OnInit {
  tasks: any[] = [];
  displayedColumns: string[] = ['title', 'description', 'status', 'assignedTo', 'dueDate', 'actions'];
  
  constructor(private taskService: TaskService) {}
  
  ngOnInit(): void {
    this.loadAllTasks();
  }
  
  loadAllTasks(): void {
    this.taskService.getTasks().subscribe(
      tasks => this.tasks = tasks,
      error => console.error('Error fetching all tasks:', error)
    );
  }
  
  addTask(): void {
    // Implement add task functionality
    console.log('Add new task');
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