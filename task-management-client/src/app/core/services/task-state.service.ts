import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Task } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskStateService {
  private tasksSubject = new BehaviorSubject<Task[]>([]);
  public tasks$ = this.tasksSubject.asObservable();
  
  private loading = new BehaviorSubject<boolean>(false);
  public loading$ = this.loading.asObservable();

  constructor() { }

  setTasks(tasks: Task[]): void {
    console.log('Setting tasks in state service:', tasks);
    this.tasksSubject.next(tasks);
  }

  updateTask(updatedTask: Partial<Task>): void {
    console.log('Updating task in state service:', updatedTask);
    const currentTasks = this.tasksSubject.value;
    const updatedTasks = currentTasks.map(task => 
      task.id === updatedTask.id ? { ...task, ...updatedTask } : task
    );
    this.tasksSubject.next(updatedTasks);
  }

  updateTaskStatus(taskId: number, status: number): void {
    console.log(`Updating task ${taskId} status to ${status} in state service`);
    const currentTasks = this.tasksSubject.value;
    const updatedTasks = currentTasks.map(task => 
      task.id === taskId ? { ...task, status } : task
    );
    this.tasksSubject.next(updatedTasks);
  }

  addTask(task: Task): void {
    console.log('Adding task to state service:', task);
    const currentTasks = this.tasksSubject.value;
    this.tasksSubject.next([...currentTasks, task]);
  }

  deleteTask(taskId: number): void {
    console.log(`Deleting task ${taskId} from state service`);
    const currentTasks = this.tasksSubject.value;
    this.tasksSubject.next(currentTasks.filter(task => task.id !== taskId));
  }

  setLoading(isLoading: boolean): void {
    this.loading.next(isLoading);
  }

  getTasks(): Task[] {
    return this.tasksSubject.value;
  }
}