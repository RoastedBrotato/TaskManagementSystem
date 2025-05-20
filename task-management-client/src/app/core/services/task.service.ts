import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  Task, 
  CreateTaskRequest, 
  UpdateTaskRequest, 
  TaskStatus 
} from '../models/task.model';
import { TaskStateService } from './task-state.service';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(
    private http: HttpClient,
    private taskState: TaskStateService
  ) { }

  getTasks(): Observable<Task[]> {
    this.taskState.setLoading(true);
    return this.http.get<Task[]>(`${environment.apiUrl}/api/tasks`)
      .pipe(
        tap(tasks => {
          this.taskState.setTasks(tasks);
          this.taskState.setLoading(false);
        })
      );
  }

  getTaskById(id: number): Observable<Task> {
    return this.http.get<Task>(`${environment.apiUrl}/api/tasks/${id}`);
  }

  getUserTasks(): Observable<Task[]> {
    this.taskState.setLoading(true);
    return this.http.get<Task[]>(`${environment.apiUrl}/api/tasks/user`)
      .pipe(
        tap(tasks => {
          this.taskState.setLoading(false);
        })
      );
  }

  createTask(task: CreateTaskRequest): Observable<Task> {
    const taskRequest = {
      ...task,
      status: Number(task.status)
    };
    return this.http.post<Task>(`${environment.apiUrl}/api/tasks`, taskRequest)
      .pipe(
        tap(newTask => {
          this.taskState.addTask(newTask);
        })
      );
  }

  updateTask(id: number, task: UpdateTaskRequest): Observable<void> {
    const taskRequest = {
      ...task,
      status: Number(task.status)
    };
    return this.http.put<void>(`${environment.apiUrl}/api/tasks/${id}`, taskRequest)
      .pipe(
        tap(() => {
          this.taskState.updateTask({ id, ...taskRequest });
        })
      );
  }

  updateTaskStatus(id: number, status: number): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/api/tasks/${id}/status`, { status: Number(status) })
      .pipe(
        tap(() => {
          console.log(`Updating task ${id} status to ${status} via service`);
          this.taskState.updateTaskStatus(id, status);
        })
      );
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/tasks/${id}`)
      .pipe(
        tap(() => {
          this.taskState.deleteTask(id);
        })
      );
  }
}