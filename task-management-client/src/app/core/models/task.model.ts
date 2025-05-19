export enum TaskStatus {
    Pending = 0,
    InProgress = 1,
    Completed = 2
  }
  
  export interface Task {
    id: number;
    title: string;
    description: string;
    dueDate: Date;
    status: TaskStatus | string;
    assignedUserId: number;
    assignedUsername?: string;
  }
  
  export interface CreateTaskRequest {
    title: string;
    description: string;
    dueDate: Date;
    status: TaskStatus;
    assignedUserId: number;
  }
  
  export interface UpdateTaskRequest {
    title: string;
    description: string;
    dueDate: Date;
    status: TaskStatus;
    assignedUserId: number;
  }
  
  export interface UpdateTaskStatusRequest {
    status: TaskStatus;
  }