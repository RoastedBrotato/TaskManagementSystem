import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="user-list-container">
      <h3>User Management</h3>
      <button mat-raised-button color="primary" class="add-button" (click)="addUser()">
        <mat-icon>add</mat-icon> Add User
      </button>
      
      <table mat-table [dataSource]="users" class="mat-elevation-z2">
        <!-- Username Column -->
        <ng-container matColumnDef="username">
          <th mat-header-cell *matHeaderCellDef>Username</th>
          <td mat-cell *matCellDef="let user">{{user.username}}</td>
        </ng-container>
        
        <!-- Email Column -->
        <ng-container matColumnDef="email">
          <th mat-header-cell *matHeaderCellDef>Email</th>
          <td mat-cell *matCellDef="let user">{{user.email}}</td>
        </ng-container>
        
        <!-- Role Column -->
        <ng-container matColumnDef="role">
          <th mat-header-cell *matHeaderCellDef>Role</th>
          <td mat-cell *matCellDef="let user">{{user.role}}</td>
        </ng-container>
        
        <!-- Actions Column -->
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Actions</th>
          <td mat-cell *matCellDef="let user">
            <button mat-icon-button color="primary" (click)="editUser(user)">
              <mat-icon>edit</mat-icon>
            </button>
            <button mat-icon-button color="warn" (click)="deleteUser(user.id)">
              <mat-icon>delete</mat-icon>
            </button>
          </td>
        </ng-container>
        
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>
      
      <div *ngIf="users.length === 0" class="no-users">
        No users available.
      </div>
    </div>
  `,
  styles: [`
    .user-list-container {
      padding: 16px;
    }
    
    .add-button {
      margin-bottom: 16px;
    }
    
    table {
      width: 100%;
    }
    
    .no-users {
      margin-top: 20px;
      text-align: center;
      color: #777;
    }
  `]
})
export class UserListComponent implements OnInit {
  users: any[] = [];
  displayedColumns: string[] = ['username', 'email', 'role', 'actions'];
  
  constructor(private userService: UserService) {}
  
  ngOnInit(): void {
    this.loadAllUsers();
  }
  
  loadAllUsers(): void {
    this.userService.getUsers().subscribe(
      users => this.users = users,
      error => console.error('Error fetching users:', error)
    );
  }
  
  addUser(): void {
    // Implement add user functionality
    console.log('Add new user');
  }
  
  editUser(user: any): void {
    // Implement edit functionality
    console.log('Edit user:', user);
  }
  
  deleteUser(userId: number): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userService.deleteUser(userId).subscribe(
        () => {
          this.users = this.users.filter(user => user.id !== userId);
        },
        error => console.error('Error deleting user:', error)
      );
    }
  }
}