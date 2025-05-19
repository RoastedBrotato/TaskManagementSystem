import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

// Material imports
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';

// Imported components (assuming these are also standalone)
import { UserTasksComponent } from '../tasks/user-tasks/user-tasks.component';
import { TaskListComponent } from '../tasks/task-list/task-list.component';
import { UserListComponent } from '../users/user-list/user-list.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTabsModule,
    UserTasksComponent,
    TaskListComponent,
    UserListComponent
  ]
})
export class DashboardComponent implements OnInit {
  currentUser: any = null;
  isAdmin = false;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;
    this.isAdmin = this.authService.isAdmin();
  }
}