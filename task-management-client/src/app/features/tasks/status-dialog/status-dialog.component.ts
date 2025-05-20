import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { TaskStatus } from '../../../core/models/task.model';

@Component({
  selector: 'app-status-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatRadioModule, FormsModule],
  template: `
    <h2 mat-dialog-title>Update Status</h2>
    <mat-dialog-content>
      <div class="status-options">
        <mat-radio-group [(ngModel)]="selectedStatus">
          <mat-radio-button *ngFor="let status of statuses" [value]="status.value" class="status-option">
            <div [class]="'status-badge status-' + status.value">
              {{ status.text }}
            </div>
          </mat-radio-button>
        </mat-radio-group>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" [mat-dialog-close]="selectedStatus">Update</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .status-options {
      display: flex;
      flex-direction: column;
      gap: 12px;
      margin: 12px 0;
    }
    
    .status-option {
      margin-bottom: 8px;
      padding: 8px;
      border-radius: 4px;
    }
    
    .status-badge {
      display: inline-block;
      padding: 4px 10px;
      border-radius: 4px;
      font-weight: 500;
    }
    
    .status-0 {
      background-color: #f5f5f5;
      color: #616161;
    }
    
    .status-1 {
      background-color: #e3f2fd;
      color: #1976d2;
    }
    
    .status-2 {
      background-color: #e8f5e9;
      color: #388e3c;
    }
  `]
})
export class StatusDialogComponent {
  selectedStatus: number;
  statuses = [
    { value: 0, text: 'Pending' },
    { value: 1, text: 'In Progress' },
    { value: 2, text: 'Completed' }
  ];

  constructor(
    public dialogRef: MatDialogRef<StatusDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {currentStatus: number}
  ) {
    this.selectedStatus = data.currentStatus;
  }
}