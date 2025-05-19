import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  Task, 
  CreateTaskRequest, 
  UpdateTaskRequest, 
  UpdateTaskStatusRequest,
  TaskStatus 
} from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private http: HttpClient) { }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(`${environment.apiUrl}/api/tasks`);
  }

  getTaskById(id: number): Observable<Task> {
    return this.http.get<Task>(`${environment.apiUrl}/api/tasks/${id}`);
  }

  getUserTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(`${environment.apiUrl}/api/tasks/user`);
  }

  createTask(task: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(`${environment.apiUrl}/api/tasks`, task);
  }

  updateTask(id: number, task: UpdateTaskRequest): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/api/tasks/${id}`, task);
  }

  updateTaskStatus(id: number, status: TaskStatus): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/api/tasks/${id}/status`, { status });
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/tasks/${id}`);
  }
}