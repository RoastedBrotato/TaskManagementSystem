import { Component, OnInit, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { TaskStatus } from '../../../core/models/task.model';

// Material imports
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule
  ]
})
export class TaskFormComponent implements OnInit {
  taskForm!: FormGroup;
  isEditMode = false;
  
  statuses = [
    { value: 0, text: 'Pending' }, // Match backend enum values
    { value: 1, text: 'In Progress' },
    { value: 2, text: 'Completed' }
  ];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<TaskFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { task?: any, users: any[] }
  ) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data.task;
    
    // Initial value for status should be numeric to match backend enum
    const initialStatus = this.isEditMode 
      ? this.getStatusValue(this.data.task?.status) 
      : 0; // Default to Pending
    
    this.taskForm = this.fb.group({
      title: [this.data.task?.title || '', [Validators.required, Validators.maxLength(100)]],
      description: [this.data.task?.description || '', [Validators.required, Validators.maxLength(500)]],
      dueDate: [this.data.task?.dueDate ? new Date(this.data.task.dueDate) : new Date(), Validators.required],
      status: [initialStatus, Validators.required],
      assignedUserId: [this.data.task?.assignedUserId || null]
    });
  }

  // Convert string status to numeric enum value
  getStatusValue(statusString: string): number {
    if (typeof statusString === 'number') return statusString;
    
    switch(statusString) {
      case 'Pending': return TaskStatus.Pending;
      case 'InProgress': return TaskStatus.InProgress;
      case 'Completed': return TaskStatus.Completed;
      default: return TaskStatus.Pending;
    }
  }

  onSubmit(): void {
    if (this.taskForm.valid) {
      const taskData = {...this.taskForm.value};
      
      // Ensure status is numeric to match backend enum
      taskData.status = Number(taskData.status);
      
      // Format date for API
      if (taskData.dueDate instanceof Date) {
        taskData.dueDate = taskData.dueDate.toISOString();
      }
      
      this.dialogRef.close(taskData);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}